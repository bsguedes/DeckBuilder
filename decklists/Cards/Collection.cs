using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decklists
{
    [DataContract]
    public class Collection
    {
        [DataMember(Name = "id")]
        public uint UniqueID { get; private set; }

        [DataMember(Name = "abbr")]
        public string Abbreviation { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "max")]
        public int MaxValue { get; private set; }

        [DataMember(Name = "data")]
        public Dictionary<string, string> Modifiers { get; private set; }

        public Collection( uint id, string abrv, string name, int maxValue, Dictionary<string, string> data )
        {
            this.UniqueID = id;
            this.Abbreviation = abrv;
            this.Name = name;
            this.MaxValue = maxValue;
            this.Modifiers = data ?? new Dictionary<string, string>();            
        }
        
    }
}
