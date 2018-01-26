using System;
using System.Collections.Generic;
using System.Text;

namespace crypto_analyse_newsletter
{
    /// <summary>
    /// Represents a feed item.
    /// </summary>
    public class Item
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public string Guid { get; set; }

        public Item()
        {
            Link = "";
            Title = "";
            Content = "";
            PublishDate = DateTime.Today;
            
        }
    }
}
