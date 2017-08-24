using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace scripts
{
    public class LigaMagicDownloader
    {
        readonly int MAXIMUM_CARDS = 4500;

        public Dictionary<int, string> Results = new Dictionary<int, string>();

        public LigaMagicDownloader()
        {

        }

        public void Download()
        {
            for (int i = 4200; i < MAXIMUM_CARDS; i++)
            {
                RequestData(i, string.Format(@"http://chucktcg.com.br/?view=ecom/item&cardP={0}", i));
            }
        }

        private async void RequestData(int index, string uri)
        {
            var client = new WebClient();
            string data = await client.DownloadStringTaskAsync(uri);
            int start = data.IndexOf("<head><title>");
            int end = data.IndexOf("</title>");
            string r = data.Substring(start + 13, end - start - 25);
            lock (this.Results)
            {
                using (StreamWriter sw = new StreamWriter("list.csv", true))
                {
                    sw.WriteLine(string.Format("{0};{1}", index, r));
                }
            }
            Console.WriteLine(string.Format("{0};{1}", index, r));
            Results.Add(index, r);
        }

    }
}
