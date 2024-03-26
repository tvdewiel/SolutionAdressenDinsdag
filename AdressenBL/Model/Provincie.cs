using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdressenBL.Model
{
    public class Provincie
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        private Dictionary<int,Gemeente> gemeentes =new Dictionary<int,Gemeente>();
        public IReadOnlyList<Gemeente> GeefGemeentes()=>gemeentes.Values.ToList();
        public void VoegGemeenteToe(Gemeente gemeente)
        {
            gemeentes.TryAdd(gemeente.Id, gemeente);
        }
    }
}
