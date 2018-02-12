using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace crypto_analyse_newsletter
{
    public class FeedParser
    {
        
        /// <summary>
        /// Parses an RSS feed and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseRss(string url,string proxyServer)
        {
            try
            {
                WebClient wc = new WebClient();
                if (proxyServer != null)
                {
                    wc.Proxy = new WebProxy(proxyServer);
                }
                MemoryStream ms = new MemoryStream(wc.DownloadData(url));
                XmlTextReader rdr = new XmlTextReader(ms);
                XDocument doc = XDocument.Load(rdr);
                // RSS/Channel/item
                var entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                              select new Item
                              {
                                  Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                  Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                  PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                  Title = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                  Guid = Hash(item.Elements().First(i => i.Name.LocalName == "link").Value)
                              };
                
                return entries.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Item>();
            }
        }

        string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
        /// <summary>
        /// Parses an RDF feed and returns a <see cref="IList&amp;lt;Item&amp;gt;"/>.
        /// </summary>
   
        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }
    }
}
