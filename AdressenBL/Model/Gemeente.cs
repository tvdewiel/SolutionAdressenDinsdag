using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Model
{
    public class Gemeente
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        private SortedSet<string> straatNamen =new SortedSet<string>();
        public IReadOnlyList<string> GeefStraatNamen()=> straatNamen.ToList();
        public void VoegStraatnaamToe(string straatnaam)
        {
            straatNamen.Add(straatnaam);
        }
    }
}
