using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsReaderSPA.Models;
using CodeHollow.FeedReader;
using System.Threading;

/* 
 * RSSClient is a provider that 
 * fetches RSS feed from a URL
 * We are using a separate class from FeedReader
 * to ensure it is wrapped in a multi threaded environment
 * that doesn't block the application
 * 
 * Todo: Unit Testing is needed for the background service
 **/
namespace NewsReaderSPA.Provider
{
    public class NewsClient
    {
        public static NewsFeed newsFeed;    // NewsFeed
        public static int _interval;        // Sleep time between each WebGet call in second
        private static string _URL;
        private DateTime _lastPublishDate;
        
        public NewsClient(string URL, int interval)
        {
            // Assign local variables
            _interval = interval;
            _URL = URL;
            _lastPublishDate = new DateTime();
            newsFeed = new NewsFeed();

            // It is mandatory to preload the instance of NewsClient
            // A) It will ensure the URL is valid and contains a valid NewsFeed
            // B) We will use the data to preload our WebSocket instance so Immediately after launch
            //      websocket can receive subscriptions.

            FetchData();
        }

        public async void Start()
        {
            // await must be issued to function for multithreading
            await StartInterval();
        }

        // Fetch content in new thread
        private async Task StartInterval()
        {
            await Task.Run(() =>
            {
                

                // Todo: We will need `Termination Signal` here
                while (true)
                {
                    Thread.Sleep(_interval * 1000);
                    FetchData();
                }
            });
        }

        /*
         * 
         * FetchData() May be called from constructor or from the threaded process
         * to fetch data and fill our news feed
         * 
         */

        private async void FetchData( )
        {
            try
            {
                var feedReader = FeedReader.ReadAsync(_URL);
                var feedResult = feedReader.Result;

                // If there are more than one Item in the result and the items are newer than last check 
                if (feedResult.Items.Count() > 0 && feedResult.LastUpdatedDate > _lastPublishDate)
                {
                    foreach (FeedItem feedItem in feedResult.Items)
                    {
                        // Only add items that are newer last Publishdate
                        if (feedItem.PublishingDate > _lastPublishDate)
                        {
                            NewsItem newsItem = new NewsItem
                            {
                                GUID = feedItem.Id,                 // Looks like GUID is casted into Id (more testing needed)
                                Title = feedItem.Title,
                                Description = feedItem.Description,
                                Link = feedItem.Link,
                                PublicationDate = (DateTime)feedItem.PublishingDate
                            };

                            newsFeed.AddItem(newsItem);
                        }

                    }
                    // Reset LastPublishdate
                    _lastPublishDate = (DateTime) feedResult.LastUpdatedDate;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Failed to read URL: {_URL} , Error: {ex.Message}");
            }
        }
    }
}
