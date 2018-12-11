
# News Reader SPA

News Reader is a single page application that reads news feed and display to user in a table,
data feed is updated live using realtime protocol, this means the user will not reload the page, 
advantage of RealTime WebSocket over Ajax or RESTapi is that all users will receive the new 
instantly without making costly and slow API calls to backend.

Stream of news feed is fetched in live without refreshing the page or any Ajax call, the items 
are added to the list on-the-fly the latest news will be added to the table in Descending order,
(being the latest news to the top of the list).

Given the limited time available for this project, "Todo" tags are placed which can be extracted 
and completed at later time.
`Visual Studio` > `View menu` > `Task List`

Demo is available at below link:
[http://newsreader.gpn.mx/](http://newsreader.gpn.mx/)

### Run with Docker:
Use the following commands to build and run your Docker image with bash  

```
./rebuild.sh

# or just run below for detail:
docker stop newsreader
docker rm newsreader
sed -i "s/ws:\/\/localhost\:57293/ws\:\/\/newsreader.gpn.mx/g" wwwroot/js/site.js
docker build -t newsreader .
docker run -d -p 5000:80 --name newsreader newsreader
```

When behind Apache set below directive:
```
ProxyPass / http://127.0.0.1:5000/
ProxyPassReverse / http://127.0.0.1:5000/

ProxyPass /feed/ ws://127.0.0.1:5000/feed/  retry=0
ProxyPassReverse /feed/ ws://127.0.0.1:5000/feed/  retry=0

# Set the response header to the captured value if there was a match
<IfModule mod_headers.c>
    Header set Access-Control-Allow-Origin "*"
</IfModule>

# This section is important to be enclosed with Location/xml and double quote
<Location "/feed">
    ProxyPass "ws://127.0.0.1:5000/feed"
</Location>
```

### Roadmap:

A) Single Page with fixed static RSS Url

B) User would be able to set/save the desired URL 

C) Read all RSS/Atom version

D) Push notification to user's browser with WebPush method upon receiving new item

F) Prefetch render news to the client's browser

G) Resource Management would clear RSS Client

## Back-end:

### NewsClient (Background Service) 
that downloads the RSS/Atom  feed on interval, given RSS runs on Web 
Http technology it doesn't have push notifications. therefore the web client
must download the feed on interval and store the feed locally.

Upon loading application in server it will initialize latest news from feed URL, it will
then download the feed and iterate through items and check for new feed.

It will only begin iteration when `Feed`.`lastBuildDate` is greater than `LocalStorage`.`lastBuildDate`

`Start()`
	Runs with Async/Await to a parallel thread and reads the RSS feed URL
	sleeps for an interval of X seconds, here we assume 30 seconds as static value,
	this may be adjusted further. it will instantiate a single instance of `NewsFeed` model 
	and will issue 

`StartInterval()` : called by Start to open a child thread to ensure Async criteria is met.

`NewsFeed`: Local storage of news item that is an array of NewsItem object.

`_interval`: in seconds (Sleep time between each WebGet call).

`_URL`: Sleep time between each WebGet call in second.


Challenge: Most Newsfeed are not in order of published but rather in order 
of appearence at homePage, so latest news is not necessary the last item in news,
therefore in NewsFeed.AddItem() we change the order to our desire.

`ResourceManagement`:
Service should stop as soon as no user is connected to web service,
we don't want waste resources 
and service will start again when there is a Socket open.

	

### NewsFeed (Model)
NewsFeed is a object that feeds the data to the based on Push notification to user-agents
Users are tapping to to RSS or Atom Feed model based on Websocket Technology. 
the benefit is that it consume less resources, it is faster and any news object that arrives 
to the feed is pushed/broadcast to all useragent that are listening the URL.

```
public class NewsFeed {
    public static List<NewsItem> ListNewsItems = new List<NewsItem>();

	// NewsFeed general parameters
	public string Title;
	public string Description;
	public string Link;
	public string LastBuildDate;
	public string pubDate;
	
	...
	...
}
```


`Feed`

#### `AddItems(RSSItem item)`: 
1. First it will sort the Object Items based on the PublicationDate in Ascending order (Oldest first and newest last)
2. Iterate through all itmes and will check if `item.PublicationDate` is greater than `LatestPublicationDate` and add them.
3. Items smaller than the Last Duplicate `GUID` will be skipped

#### `LatestPublicationDate`:
Publication date of latest item in the local storage

### NewsItem (Object):
RSS Object that consists of below params:
```
namespace NewsReaderSPA.Models{
    public class NewsItem{
        public static string GUID;
        public static string Title;
        public static string Description;
        public static string Link;
        public static DateTime PublicationDate;
    }
}
```

Further: https://en.wikipedia.org/wiki/RSS

## Websocket Controller:
Sends initial data to Subscriber and receive callback from `NewsClient` each time there is a new `NewsItem` received from URL
1) Will open `ws://URL/feed` for useragent to subscribe
2) At `OnOpen()` event it will feed the subscriber with data
3) `SendAsync()` uses the method to send initial data to new subscriber
4) `SendAll(List<NewsItem> newsItem)` would broadcast a news item to all useragents
`WebSocketUserList` Manage WebSocket subscriber in `List<WebSocket>`
`_newsClient` access to static `NewsFeed`

## Front-end:

It uses the Javascript Websocket model upon loading the page and subscribe to 
ws://URL/feed 

### Components:
- Bootstrap
- jQuery
- DataTables
- WebSocket


### Workflow:
- Open loading the first page, the table already consist the preloaded data
	Todo:
	It will be much faster to take advantage of BigPipe and load the index.html
	in static mode without preloaded data and only add the items whenever there is a new
- It will use DataTables to display the news in multi-page format
- Datatables's api will assist in adding items to the beginning of the list.

	
## Appendix

Atom Example:
```
<?xml
version="1.0" encoding="utf-8"?>
<rss xmlns:atom="http://www.w3.org/2005/Atom" version="2.0" xmlns:media="http://search.yahoo.com/mrss/" xmlns:dc="http://purl.org/dc/elements/1.1/">
    <channel>
        <title>NU - Algemeen</title>
        <link>https://www.nu.nl/algemeen</link>
        <description>Het laatste nieuws het eerst op NU.nl</description>
        <atom:link href="https://www.nu.nl/rss/Algemeen" rel="self"></atom:link>
        <language>nl-nl</language>
        <copyright>Copyright (c) 2018, NU</copyright>
        <lastBuildDate>Mon, 10 Dec 2018 18:24:33 +0100</lastBuildDate>
        <ttl>60</ttl>
        <atom:logo>https://www.nu.nl/algemeenstatic/img/atoms/images/logos/rss-logo-250x40.png</atom:logo>
        <item>
            <title>Britse premier May stelt cruciale Brexit-stemming voorlopig uit</title>
            <link>https://www.nu.nl/brexit/5621771/britse-premier-may-stelt-cruciale-brexit-stemming-voorlopig.html</link>
            <description>De stemming over de Brexit-deal zal morgen toch niet plaatsvinden. Het kabinet van Theresa May stelt de stemming voorlopig uit. Een nieuwe datum is nog niet bekend.</description>
            <pubDate>Mon, 10 Dec 2018 18:02:06 +0100</pubDate>
            <guid isPermaLink="false">https://www.nu.nl/-/5621771/</guid>
            <enclosure url="https://media.nu.nl/m/20dxbkbazlfv_sqr256.jpg/britse-premier-may-stelt-cruciale-brexit-stemming-voorlopig.jpg" length="0" type="image/jpeg"></enclosure>
            <category>Algemeen</category>
            <category>Brexit</category>
            <dc:creator>NU.nl/Thomas Moerman</dc:creator>
            <dc:rights>copyright photo: AFP</dc:rights>
        </item>
    </channel>
</rss>
```


RSS Example:
```
<?xml version="1.0" encoding="UTF-8" ?>
<rss version="2.0">
<channel>
	<title>RSS Title</title>
	<description>This is an example of an RSS feed</description>
	<link>http://www.example.com/main.html</link>
	<lastBuildDate>Mon, 06 Sep 2010 00:01:00 +0000 </lastBuildDate>
	<pubDate>Sun, 06 Sep 2009 16:20:00 +0000</pubDate>
	<ttl>1800</ttl>
	<item>
		<title>Example entry</title>
		<description>Here is some text containing an interesting description.</description>
		<link>http://www.example.com/blog/post/1</link>
		<guid isPermaLink="false">7bd204c6-1655-4c27-aeee-53f933c5395f</guid>
		<pubDate>Sun, 06 Sep 2009 16:20:00 +0000</pubDate>
	</item>
</channel>
</rss>
```