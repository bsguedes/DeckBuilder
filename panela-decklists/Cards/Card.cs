using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Cards
{
    public class Card
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public Collection Collection { get; private set; }

        public Card( int index, string name, Collection collection )
        {            
            this.Index = index;
            this.Name = name;
            this.Collection = collection;
        }

        public override string ToString()
        {
            return string.Format( "{0} - {1}", this.Index, this.Name );
        }
    }
}
