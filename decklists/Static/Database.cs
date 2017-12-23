using Decklists.Providers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Decklists.Static
{
    [DataContract]
    public class Database
    {
        public List<Collection> Collections { get; private set; }
        public List<Card> Cards { get; private set; }
        public List<Quotation> Quotations { get; private set; }
        public List<ProviderDescriptor> Providers { get; private set; }

        private Database()
        {
            this.Collections = new List<Collection>();
            this.Cards = new List<Card>();
            this.Quotations = new List<Quotation>();
            this.Providers = new Provider().ProviderList.ToList();
        }

        private static Database _instance;
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Database();
                }
                return _instance;
            }
        }

        public void ReloadFromJSON()
        {
            Collection[] db_collections = JsonConvert.DeserializeObject<Collection[]>(File.ReadAllText("dbcollections.json"));
            this.Collections = db_collections.OrderByDescending(x => x.UniqueID).ToList();

            Card[] db_cards = JsonConvert.DeserializeObject<Card[]>(File.ReadAllText("dbcards.json"));
            this.Cards = db_cards.ToList();

            Quotation[] db_quotations = JsonConvert.DeserializeObject<Quotation[]>(File.ReadAllText("dbquotations.json"));
            this.Quotations = db_quotations.ToList();
        }

        public void SaveToJSON()
        {
            string json = JsonConvert.SerializeObject(this.Quotations, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter("dbquotations.json", false))
            {
                sw.WriteLine(json);
            }

            json = JsonConvert.SerializeObject(this.Cards, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter("dbcards.json", false))
            {
                sw.WriteLine(json);
            }

            json = JsonConvert.SerializeObject(this.Collections, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter("dbcollections.json", false))
            {
                sw.WriteLine(json);
            }
        }

        public void AddCollection(Collection collection)
        {
            if (this.Collections.FirstOrDefault(x => x.UniqueID == collection.UniqueID) == null)
            {
                this.Collections.Add(collection);
            }
        }

        internal void AddCard(Card card)
        {
            if (this.Cards.FirstOrDefault(x => x.UniqueID == card.UniqueID) == null)
            {
                this.Cards.Add(card);
            }
        }

        internal void AddQuotation(Quotation q)
        {
            this.Quotations.Add(q);
        }
    }
}
