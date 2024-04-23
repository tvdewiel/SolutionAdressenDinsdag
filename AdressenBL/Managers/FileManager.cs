using AdressenBL.Exceptions;
using AdressenBL.Interfaces;
using AdressenBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Managers
{
    public class FileManager
    {
        private IFileProcessor processor;

        public FileManager(IFileProcessor processor)
        {
            this.processor = processor;
        }

        public bool IsFolderEmpty(string folderName)
        {
            try
            {
                return processor.IsFolderEmpty(folderName);
            }
            catch(Exception ex) { throw new FileManagerException($"IsFolderEmpty - {ex.Message}"); }
        }

        public Dictionary<string,string> CheckZipFile(string zipFileName,List<string> fileNames)
        {
            try
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                List<string> configEntries = new List<string>() {
                "streetNames",
                "municipalityNames",
                "link_StreetName_Municipality",
                "link_Province_MunicipalityNames",
                "provinces"
                };
                Dictionary<string, string> errors = new();
                if (!fileNames.Contains("FileNamesConfig.txt"))
                    throw new ZipFileManagerException("FileNamesConfig.txt is missing");
                var data = processor.GetFileNamesConfigInfoFromZip(zipFileName, "FileNamesConfig.txt");
                foreach (var line in data)
                {
                    string[] parts = line.Split(':');
                    map.Add(parts[0].Trim(), parts[1].Trim().Replace('\"'.ToString(), string.Empty));
                }
                //controleer of de 5 entries in config staan
                foreach (string entry in configEntries)
                {
                    if (!map.ContainsKey(entry)) //error
                        errors.Add(entry, "missing in config");

                }
                //controleer of de 5 bestanden in zip zitten
                foreach (string file in map.Values)
                {
                    if (!fileNames.Contains(file)) errors.Add(file, "missing in zip");
                }
                if (errors.Count > 0)
                {
                    ZipFileManagerException ex = new ZipFileManagerException("Files missing");
                    foreach (var e in errors)
                    {
                        ex.Data.Add(e.Key, e.Value);
                    }
                    throw ex;
                }
                return map;
            }
            catch (ZipFileManagerException) { throw; }
            catch (Exception ex) { throw new FileManagerException($"CheckZipFile - {ex.Message}", ex); }
        }

        public void CleanFolder(string folderName)
        {
            try
            {
                processor.ClearFolder(folderName);
            }
            catch (Exception ex) { throw new FileManagerException($"CleanFolder - {ex.Message}"); }
        }

        public List<string> GetFilesFromZip(string zipFile)
        {
            try
            {
                return processor.GetFileNamesFromZip(zipFile);
            }
            catch(Exception ex)
            {
                throw new FileManagerException($"GetFilesFromZip - {ex.Message}", ex);
            }
        }

        public List<string> ProcessZip(string zipFileName, string unzipFolder)
        {
            List<string> messages=new List<string>();
            try
            {
                //unzip
                processor.UnZip(zipFileName, unzipFolder);
                //maken obj (lezen bestanden)
                List<string> files=GetFilesFromZip(zipFileName);
                List<Provincie> provincies=processor.ReadFiles(CheckZipFile(zipFileName,files), unzipFolder);
                //schrijven folders/bestanden
                processor.WriteResults(unzipFolder, provincies);
                //statistieken maken
                var stats = CalculateStatistics(provincies);
                messages.AddRange(stats.Provincies.Select(x=>$"{x.Key} : {x.Value} gemeenten").ToList());
                messages.AddRange(stats.Gemeentes.Select(x=>$"{x.Key} : {x.Value} straten").ToList());
                return messages;
            }
            catch(Exception ex) { throw new FileManagerException($"ProcessZip - {ex.Message}"); }
        }

        private Statistieken CalculateStatistics(List<Provincie> provincies)
        {
            var statistieken=new Statistieken();
            statistieken.Provincies = provincies.ToDictionary(g => g.Naam, g => g.GeefGemeentes().Count());
            statistieken.Gemeentes=provincies
                .SelectMany(list=>list.GeefGemeentes(),(p,m)=>new {name=(p.Naam,m.Naam),count=m.GeefStraatNamen().Count})
                .OrderBy(g => g.name)
                .ToDictionary(x=>x.name,x=>x.count);
            return statistieken;
        }
    }
}
