using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace NewsReaderSPA.WebSocketConf
{
    /*
     * WebSocket Configuration Objects
     * 
     */

    public class wsConfig
    {

        // accessible variable for BufferSize (4kbytes)
        public static int BufferSize = 4 * 1024;

        // define static object to call without instantiation
        public static WebSocketOptions GetOptions()
        {
            WebSocketOptions MyWebSocketOptions = new WebSocketOptions()
            {
                ReceiveBufferSize = BufferSize,
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            return MyWebSocketOptions;
        }
    }
}
