using AdressenBL.Interfaces;
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

        public void UnZip(string zipFileName, string unzipFolder)
        {
           ZipFile.ExtractToDirectory(zipFileName, unzipFolder);
        }
    }
}
