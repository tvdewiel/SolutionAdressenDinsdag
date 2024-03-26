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
    }
}
