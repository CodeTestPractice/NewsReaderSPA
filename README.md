
## News Reader SPA

News Reader is a single page application that reads RSS feed and display using realtime protocol 
and display the latest news to the user.

News feed is fetched live without refreshing the page or any Ajax call, the items are added to the list on-the-fly
the latest news will be added to the table in Descending order, (being the latest news to the top of the list)

Note: While assignment has mentioned that RSS client is mind the example was given in Atom format,
		so for the sake of this example the application will Atom format,

		RSS format can be parsed into the 

Application logic and models are follows: 

Roadmap:

A) Single Page with fixed static RSS Url
B) User would be able to set/save the desired URL 
C) Read all RSS/Atom version
D) Push notification to user's browser with WebPush method upon receiving new item
F) Prefetch render news to the client's browser

## Back-end:

A)	RSS Client (Provider) 
		that downloads the RSS feed on interval, given RSS runs on Web 
		Http technology it doesn't have push notifications. therefore the web client
		must download the feed on interval and store the feed locally.

	Start()
		Runs with Async/Await to a parallel thread and reads the RSS feed URL
		sleeps for an interval of X seconds, here we assume 30 seconds as static value,
		this may be adjusted further


	ResourceManagement:
		Todo:
		Service should stop as soon as no user is connected to web service,
		we don't want waste resources 
		and service will start again when there is a Socket open.

	

B) NewsFeed (Model)
	Feed is a object that feeds the data to the based on Push notification to user-agents
	Users are tapping to to RSS or Atom Feed model based on Websocket Technology. 
	the benefit is that it consume less resources, it is faster and any news object that arrives to the feed is 
	pushed/broadcast to all useragent that are listening the URL.

	Channel.title
	Channel.description
	Channel.link
	Channel.lastBuildDate
	Channel.pubDate

Atom Example:
```
<title>NU - Algemeen</title>
<link>https://www.nu.nl/algemeen</link>
<description>Het laatste nieuws het eerst op NU.nl</description>
<atom:link href="https://www.nu.nl/rss/Algemeen" rel="self"></atom:link>
<language>nl-nl</language>
<copyright>Copyright (c) 2018, NU</copyright>
<lastBuildDate>Mon, 10 Dec 2018 15:43:53 +0100</lastBuildDate>
<ttl>60</ttl>
<atom:logo>https://www.nu.nl/algemeenstatic/img/atoms/images/logos/rss-logo-250x40.png</atom:logo>
```

Rss Example:
````
<?xml version="1.0" encoding="UTF-8" ?>
<rss version="2.0">
<channel>
	<title>RSS Title</title>
	<description>This is an example of an RSS feed</description>
	<link>http://www.example.com/main.html</link>
	<lastBuildDate>Mon, 06 Sep 2010 00:01:00 +0000 </lastBuildDate>
	<pubDate>Sun, 06 Sep 2009 16:20:00 +0000</pubDate>
	<ttl>1800</ttl>
</channel>
```
	AddItems(RSSItem item): 
		1. First it will sort the Object Items based on the PublicationDate in Ascending order (Oldest first and newest last)
		2. Iterate through all itmes and will check if item.PublicationDate is greater than LatestPublicationDate item
		3. 

	LatestPublicationDate: Publication date of latest item in the local store, 

	

C) News Item Object:
	RSS Object that consists of below params:
	1) GUID 
	2) Title
	3) Description
	4) Link RSS,Atom : link
	5) PublicationDate

	Atom Example:
	<item>
		<title>Brits kabinet houdt spoedoverleg, stelt Brexit-stemming mogelijk uit</title>
		<link>https://www.nu.nl/brexit/5621312/brits-kabinet-houdt-spoedoverleg-stelt-brexit-stemming-mogelijk.html</link>
		<description>De Britse premier Theresa May houdt momenteel een telefonisch spoedoverleg met haar kabinet. Diverse Britse media melden dat de stemming over het terugtredingsakkoord morgen wordt uitgesteld.</description>
		<pubDate>Mon, 10 Dec 2018 14:52:47 +0100</pubDate>
		<guid isPermaLink="false">https://www.nu.nl/-/5621312/</guid>
		<enclosure url="https://media.nu.nl/m/7g8x5wrayaj7_sqr256.jpg/brits-kabinet-houdt-spoedoverleg-stelt-brexit-stemming-mogelijk.jpg" length="0" type="image/jpeg"></enclosure>
		<category>Economie</category>
		<category>Algemeen</category>
		<category>Brexit</category>
		<dc:creator>NU.nl</dc:creator>
		<dc:rights>copyright photo: Hollandse Hoogte</dc:rights>
	</item>

	RSS Example:
	<item>
		<title>Example entry</title>
		<description>Here is some text containing an interesting description.</description>
		<link>http://www.example.com/blog/post/1</link>
		<guid isPermaLink="false">7bd204c6-1655-4c27-aeee-53f933c5395f</guid>
		<pubDate>Sun, 06 Sep 2009 16:20:00 +0000</pubDate>
	</item>

	Reference: https://en.wikipedia.org/wiki/RSS

D) Websocket service:
	1) will open ws://URL/feed subscribe
	2) Upon adding a new item to the NewsFeed it will ad
	3) data output are JSON format for easy and accurate parsing at client

## Front-end:

It uses the Javascript Websocket model upon loading the page and subscribe to 
ws://URL/feed 

- Open loading the first page, the table already consist the preloaded data
	Todo:
	It will be much faster to take advantage of BigPipe and load the index.html
	in static mode without preloaded data and only add the items whenever there is a new
- It will use DataTables to display the news in multi-page format
- Datatables's api will assist in adding items to the beginning of the list.

	