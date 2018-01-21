using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Decklists
{
    [DataContract]
    public class Card
    {
        [DataMember(Name = "id")]
        public uint UniqueID { get; private set; }

        [DataMember(Name = "index")]
        public int Index { get; private set; }

        [DataMember(Name = "str_index")]
        public string StringIndex { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "col")]
        public uint CollectionID { get; private set; }

        [DataMember(Name = "modifiers")]
        public Dictionary<string, string> Modifiers { get; set; }

        [JsonIgnore]
        public Collection Collection => Static.Database.Instance.Collections.First(x => x.UniqueID == CollectionID);

        public Card(int index, string str_index, string name, uint col, Dictionary<string, string> modifiers)
        {
            this.Index = index;
            this.StringIndex = str_index;
            this.Name = name;
            this.CollectionID = col;
            this.Modifiers = modifiers ?? new Dictionary<string, string>();
            this.UniqueID = (uint)string.Format("{0}|{1}|{2}", index, name, col).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.StringIndex, this.Name);
        }
    }
}
