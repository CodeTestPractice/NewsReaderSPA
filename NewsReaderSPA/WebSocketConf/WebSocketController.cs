using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using NewsReaderSPA.Models;
using NewsReaderSPA.Provider;

namespace NewsReaderSPA.WebSocketConf
{
    public class WebSocketController
    {
        public static List<WebSocket> WebSocketUserList = new List<System.Net.WebSockets.WebSocket>();
        private static NewsClient _newsClient = new NewsClient();

        public WebSocketController(NewsClient newsClient)
        {
            _newsClient = newsClient;
        }
        // async function to initiate new thread to avoid blocking
        public async Task ListenAcceptAsync(HttpContext context)
        {
            
            // register the context as a websocket handle
            WebSocket WebSocketHandle = await context.WebSockets.AcceptWebSocketAsync();

            // Add WebSocketHandle to List<>
            WebSocketUserList.Add( WebSocketHandle );

            // pass received data to 
            await ReceiveAsync(WebSocketHandle);
        }

        // Below method opens a new thread and read the message buffer of desired size
        // puts it into a string only if this is String/Text format, it will skip if it receives
        // a binary or empty message
        public async Task ReceiveAsync(WebSocket WebSocketHandle)
        {
            // Receive buffer in byte format
            byte[] buff;

            // Receive data until WebSocketState is Open
            while (WebSocketHandle.State == WebSocketState.Open)
            {
                // Set buffer size from wsconfig's static setting
                buff = new byte[wsConfig.BufferSize];

                // New Useragent has subscribed for news,
                // Sending initial payload.
                await SendAsync(WebSocketHandle, NewsClient.newsFeed.getListNewsItems());

                // Read result from websocket in new thread and pass the CancellationToken to 'none'
                WebSocketReceiveResult wsReceiveResult = await WebSocketHandle.ReceiveAsync(
                    new ArraySegment<byte>(array: buff, offset: 0, count: buff.Length),  // get chunk of bytes with data using ArraySegment
                    CancellationToken.None
                );

                // Check if receive bytes are not empty
                if (wsReceiveResult != null)
                {
                    // Filter Text format from Binary format.
                    switch (wsReceiveResult.MessageType)
                    {
                        // We have received Text Message
                        case WebSocketMessageType.Text:
                        // User is not suppose to send message to server
                        // this is a one way communication
                        // therefore a sign the client app is tampered
                        // Todo: user IP must be blocked
                        // We have received Close Connection request
                        case WebSocketMessageType.Close:
                            // exit funciton

                            return;

                        default:
                            break;


                    }
                }
            }
        }

        // Send a reply to WebSocket Connection
        // It receives handle to connection and a text message that needs to be send.
        // it will convert the message into UTF8 String and then to Binary Bytes to send over
        // TCP WebSocket connection.
        public async Task SendAsync(WebSocket WebSocketHandle, List<NewsItem> newsItem)
        {
            // Send a list of NewsItem to client
            string newsItemString = JsonConvert.SerializeObject(newsItem);
            byte[] buff = Encoding.UTF8.GetBytes(newsItemString);


            // open a thread and initiate message send
            await WebSocketHandle.SendAsync(
                new ArraySegment<byte>(array: buff,
                                       offset: 0,
                                       count: buff.Length), // get chunk of bytes with data using ArraySegment
                messageType: WebSocketMessageType.Text,     // The actual text message
                endOfMessage: true,                         // Wether message ends here or more packets are to come.
                cancellationToken: CancellationToken.None   // With no cancellation Token
            );
        }

        // Broadcasting set of new news item to all clients
        public async Task SendAll(List<NewsItem> newsItem)
        {
            // Send a list of NewsItem to client
            string newsItemString = JsonConvert.SerializeObject(newsItem);
            byte[] buff = Encoding.UTF8.GetBytes(newsItemString);

            foreach (var WebSocketHandle in WebSocketUserList)
            {
                // Todo: list.RemoveAt(index); Delete disconnected user from the pool : 
                if (WebSocketHandle.State == WebSocketState.Open)
                {
                    // Todo: Replace broadcast with SignalR broadcast
                    // open a thread and initiate message send
                    await WebSocketHandle.SendAsync(
                        new ArraySegment<byte>(array: buff,
                                               offset: 0,
                                               count: buff.Length), // get chunk of bytes with data using ArraySegment
                        messageType: WebSocketMessageType.Text,     // The actual text message
                        endOfMessage: true,                         // Wether message ends here or more packets are to come.
                        cancellationToken: CancellationToken.None   // With no cancellation Token
                    );
                }
                
            }
            
        }
    }
}
