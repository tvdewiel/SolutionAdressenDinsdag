using AdressenBL.Exceptions;
using AdressenBL.Interfaces;
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

        public void CheckZipFile(string zipFileName,List<string> fileNames)
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
            }
            catch (ZipFileManagerException) { throw; }
            catch (Exception ex) { throw new FileManagerException($"CheckZipFile - {ex.Message}", ex); }
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
    }
}
