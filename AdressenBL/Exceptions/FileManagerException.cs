using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Exceptions
{
    public class FileManagerException : Exception
    {
        public FileManagerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
