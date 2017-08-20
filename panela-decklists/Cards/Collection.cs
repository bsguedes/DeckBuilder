using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Cards
{
    public class Collection
    {
        public string Abbreviation { get; private set; }
        public string Name { get; private set; }
        public int MaxValue { get; private set; }

        public Collection( string abrv, string name, int maxValue )
        {
            this.Cards = new List<Card>();
            this.Abbreviation = abrv;
            this.Name = name;
            this.MaxValue = maxValue;
        }

        public List<Card> Cards { get; set; }



    }
}
