using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Model
{
    public class Statistieken
    {
        public Dictionary<string, int> Provincies { get; set; } = new();
        public Dictionary<(string,string),int> Gemeentes {  get; set; } = new(); 
    }
}
