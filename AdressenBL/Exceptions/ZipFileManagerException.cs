using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Exceptions
{
    public class ZipFileManagerException : Exception
    {
        public ZipFileManagerException(string? message) : base(message)
        {
        }

        public ZipFileManagerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
