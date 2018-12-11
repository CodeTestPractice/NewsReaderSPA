using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsReaderSPA.Models
{
    public class NewsFeed
    {
        public static List<NewsItem> ListNewsItems { get; set; } = new List<NewsItem>();

        // NewsFeed general parameters
        public string Title;
        public string Description;
        public string Link;
        public DateTime LastBuildDate;
        public string pubDate;

        public NewsFeed()
        {
            //ListNewsItems = new List<NewsItem>();
        }

        // add a single news item to Feed
        public void AddItem(NewsItem newsItem)
        {
            if (ListNewsItems.Count()>0 && newsItem.PublicationDate > ListNewsItems[0].PublicationDate)
            {
                // Add item to beginning of the list if this is a new item 
                // compared to last item
                ListNewsItems.Insert(0, newsItem);
            }
            else
            {
                // add item to the end of the list if this is older item
                ListNewsItems.Add(newsItem);
            }
            
        }
        
        public List<NewsItem> getListNewsItems()
        {
            return ListNewsItems;
        }

    }
}
