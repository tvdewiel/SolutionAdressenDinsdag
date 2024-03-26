using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Interfaces
{
    public interface IFileProcessor
    {
        List<string> GetFileNamesFromZip(string zipFile);
        List<string> GetFileNamesConfigInfoFromZip(string zipFile,string configFile);
    }
}
