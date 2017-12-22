using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace scripts
{
    public class LigaMagicDownloader
    {        
        public Dictionary<int, string> Results = new Dictionary<int, string>();
        private int start;
        private int end;
        private string file;        

        public LigaMagicDownloader(int start, int end, string file)
        {
            this.start = start;
            this.end = end;
            this.file = file;
        }

        public void Download()
        {
            for (int i = start; i <= end; i++)
            {                
                RequestData(i, string.Format(@"http://chucktcg.com.br/?view=ecom/item&cardP={0}", i));                
            }            
        }

        private void RequestData(int index, string uri)
        {
            try
            {

            var client = new WebClient();
            string data = client.DownloadString(uri);
            int start = data.IndexOf("<head><title>");
            int end = data.IndexOf("</title>");
            string r = data.Substring(start + 13, end - start - 25);
            if (r.Contains("Break"))
            {
                r.Replace("Break", "BREAK");
            }
            lock (this.Results)
            {
                using (StreamWriter sw = new StreamWriter(file, true))
                {
                    sw.WriteLine(string.Format("{0};{1}", index, r));
                }
            }
            Console.WriteLine(string.Format("{0};{1}", index, r));
            Results.Add(index, r);            
            }
            catch
            {

            }
        }

    }
}
