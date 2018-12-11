using System;

namespace NewsReaderSPA.Models
{
    public class NewsItem
    {
        public string GUID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
