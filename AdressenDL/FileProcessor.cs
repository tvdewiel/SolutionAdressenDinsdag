using AdressenBL.Interfaces;
using AdressenBL.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenDL
{
    public class FileProcessor : IFileProcessor
    {
        public void ClearFolder(string folderName)
        {
            DirectoryInfo dir=new DirectoryInfo(folderName);
            foreach(FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }
            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

        public List<string> GetFileNamesConfigInfoFromZip(string fileName, string configName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(fileName))
            {
                var entry = archive.GetEntry(configName);
                if (entry != null)
                {
                    List<string> data = new();
                    // Open a stream to read the file content
                    using (Stream entryStream = entry.Open())
                    using (StreamReader reader = new StreamReader(entryStream))
                    {
                        // Read and process the content of the file
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            data.Add(line);
                        }
                        return data;
                    }
                }
                else throw new FileNotFoundException($"{configName} not found");
            }
        }

        public List<string> GetFileNamesFromZip(string zipFileName)
        {
            if (!File.Exists(zipFileName)) throw new FileNotFoundException($"{zipFileName} not found");
            using(var zipFile = ZipFile.OpenRead(zipFileName))
            {
                return zipFile.Entries.Select(x=>x.FullName).ToList();
            }
        }

        public bool IsFolderEmpty(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);
            return (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0);
        }

        public List<Provincie> ReadFiles(Dictionary<string, string> filesMap, string dir)
        {
            Dictionary<int,Provincie> provinces = new Dictionary<int,Provincie>();
            Dictionary<int,Gemeente> municipalities=new Dictionary<int,Gemeente>();
            Dictionary<int,string> streetNames= new Dictionary<int,string>();
            HashSet<int> provincieIds = new HashSet<int>();
            //lees provincieIds
            using (StreamReader p = new StreamReader(Path.Combine(dir, filesMap["provinces"])))
            {
                string[] ids = p.ReadLine().Trim().Split(",");
                foreach(string id in ids) { provincieIds.Add(Int32.Parse(id)); }
            }
            //lees provincieInfo
            using(StreamReader gp= new StreamReader(Path.Combine(dir, filesMap["link_Province_MunicipalityNames"])))
            {
                string line;
                gp.ReadLine();
                while((line = gp.ReadLine()) != null)
                {
                    string[] ss=line.Trim().Split(";");
                    int municipalityId = Int32.Parse(ss[0]);
                    int provinceId = Int32.Parse(ss[1]);
                    string languageCode = ss[2];
                    string provinceName= ss[3];
                    if (provincieIds.Contains(provinceId) && languageCode == "nl")
                    {
                        if (!provinces.ContainsKey(provinceId))
                        {
                            provinces.Add(provinceId, new Provincie(provinceId, provinceName));
                        }
                        if (!provinces[provinceId].HeeftGemeente(municipalityId))
                        {
                            provinces[provinceId].VoegGemeenteToe(new Gemeente(municipalityId));
                            municipalities.Add(municipalityId, provinces[provinceId].GeefGemeente(municipalityId));
                        }
                    }
                }                
            }
            //lees gemeentenaam
            using (StreamReader g = new StreamReader(Path.Combine(dir, filesMap["municiplaityNames"])))
            {
                string line;
                g.ReadLine();
                while((line = g.ReadLine()) != null)
                {
                    string[] ss= line.Trim().Split(";");
                    int municipalityId= Int32.Parse(ss[1]);
                    string languageCode= ss[2];
                    string municipalityName= ss[3];
                    if (languageCode == "nl")
                    {
                        if (municipalities.ContainsKey(municipalityId)) municipalities[municipalityId].Naam = municipalityName;
                    }
                }
            }
            //lees straatnamen
            using(StreamReader s=new StreamReader(Path.Combine(dir, filesMap["streetNames"])))
            {
                string line;
                s.ReadLine();
                while((line = s.ReadLine()) != null)
                {
                    string[] ss= line.Trim().Split(";");
                    int streetnameId= Int32.Parse(ss[0]);
                    string streetname= ss[1];
                    streetNames.Add(streetnameId, streetname);
                }
            }
            //koppel straten aan gemeente
            using(StreamReader sg=new StreamReader(Path.Combine(dir, filesMap["link_StreetName_Municipality"])))
            {
                string line;
                sg.ReadLine();
                while ((line = sg.ReadLine()) != null)
                {
                    string[] ss= line.Trim().Split(";");
                    int municipalityId= Int32.Parse(ss[1]);
                    int streetnameId = Int32.Parse(ss[0]);
                    if (municipalities.ContainsKey((municipalityId)) && streetNames.ContainsKey(streetnameId))
                    {
                        municipalities[municipalityId].VoegStraatnaamToe(streetNames[streetnameId]);
                    }
                }
            }
            return provinces.Values.ToList();
        }

        public void UnZip(string zipFileName, string unzipFolder)
        {
           ZipFile.ExtractToDirectory(zipFileName, unzipFolder);
        }

        public void WriteResults(string unzipFolder, List<Provincie> provincies)
        {
            DirectoryInfo di = new DirectoryInfo(unzipFolder);
            foreach(Provincie province in provincies)
            {
                di.CreateSubdirectory(province.Naam);
                foreach(Gemeente gemeente in province.GeefGemeentes())
                {
                    WriteMunicipality(Path.Combine(unzipFolder,province.Naam,gemeente.Naam+".txt"),gemeente);
                }
            }
        }
        private void WriteMunicipality(string fileName,Gemeente gemeente)
        {
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                foreach(string streetName in gemeente.GeefStraatNamen())
                {
                    sw.WriteLine(streetName);
                }
            }
        }
    }
}
