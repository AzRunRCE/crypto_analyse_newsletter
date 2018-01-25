using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace crypto_analyse_newsletter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Crypto-Analyse Push service starting..");
            FeedParser feedParser = new FeedParser();

            try
            {
                string ArticleKnowed = GetArticlesHash();
                IList<Item> items = feedParser.Parse("https://crypto-analyse.org/category/accueil/analyses/feed/", FeedType.RSS).Where(x => ArticleKnowed.Contains(x.Guid) == false).ToList();
                foreach (var itm in items)
                {
                    using (WebClient wc = new WebClient())
                    {
                        string htmlString = wc.DownloadString(itm.Link);
                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        htmlDocument.LoadHtml(htmlString);
                        var text = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='off-canvas-body']/div[4]/div/div/div/div[1]/div/article").InnerHtml;
                        var msg = new MailMessage("quentin.martinez@outlook.com", "azrunsoft@gmail.com", itm.Title, text);
                        msg.To.Add("quentin.martinez@outlook.com");
                        msg.To.Add("bc@chunkz.net");
                        msg.IsBodyHtml = true;
                        var login = GetLogin();
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(login.Split(':')[0], login.Split(':')[1])
                        };
                        smtp.Send(msg);
                        Console.WriteLine(itm.Title + " Email Sended Successfully");
                        System.IO.File.AppendAllText("log.txt", itm.Title + " Email Sended Successfully");
                    }

                    AddArticle(itm.Guid);
                }
                Console.Clear();
                Console.WriteLine("Scanned at " + DateTime.Now.ToString());
                System.IO.File.AppendAllText("log.txt", "Scanned at " + DateTime.Now.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.IO.File.AppendAllText("log.txt", ex.Message);
            }
        }

        static bool AddArticle(string hash)
        {
            try
            {
                System.IO.File.AppendAllText("db.txt", hash + Environment.NewLine);
                return true;
            }
            catch
            {
                return false;
            }
        }
        static string GetArticlesHash()
        {
            if (!System.IO.File.Exists("db.txt"))
            {
                return "";
            }
            return System.IO.File.ReadAllText("db.txt");
        }
        static string GetLogin()
        {
            if (!System.IO.File.Exists("login.txt"))
            {
                return "email@gmail.com:pass";
            }
            return System.IO.File.ReadAllText("login.txt");
        }
    }
}
