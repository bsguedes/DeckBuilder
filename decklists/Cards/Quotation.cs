using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decklists
{
    [DataContract]
    public class Quotation
    {
        public Quotation(uint card, uint provider, long timestamp, string value)
        {
            this.CardID = card;
            this.ProviderID = provider;
            this.Timestamp = timestamp;
            Value = value;
            this.UniqueID = (uint)string.Format("{0}|{1}|{2}|{3}", card, provider, timestamp, value).GetHashCode();
        }

        [DataMember(Name = "id")]
        public ulong UniqueID { get; private set; }

        [DataMember(Name = "card")]
        public uint CardID { get; private set; }        

        [DataMember(Name = "provider")]
        public uint ProviderID { get; private set; }

        [DataMember(Name = "value")]
        public string Value { get; private set; }

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; private set; }
       
        public Card Card => Static.Database.Instance.Cards.First(x => x.UniqueID == CardID);

        public override string ToString()
        {
            DateTime ts = DateTime.FromFileTimeUtc(this.Timestamp);
            return string.Format("{0} - {1} ({2})", Static.Database.Instance.Providers.First(x => x.ID == this.ProviderID), this.Value, ts.Date.ToShortDateString());
        }
    }
}
