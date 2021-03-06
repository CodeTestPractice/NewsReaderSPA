﻿var WebSocketHandle;

// Connect Browser to WebSocket Server
function Connect() {

    // create WebSocket URI address
    var uri = "ws://newsreader.gpn.mx/feed";

    // instantiate a WebSocket object and set handle
    WebSocketHandle = new WebSocket(uri);

    // (OnOpen) Upon Connecting to server
    // Trigger below function : add log to MessageArea div
    WebSocketHandle.onopen = function (DataObject) {
        // Get MessageArea DOM div object
        var messageArea = document.getElementById("messageArea");

        // Append a log : Connected to Message Area
        console.log( "Connected to : " + DataObject.currentTarget.url);
    }

    // (OnClose) Upon Connecting to server
    // Trigger below function : add log to MessageArea div
    WebSocketHandle.onclose = function (DataObject) {
        // Append a log
        console.log("Disconnected from : " + DataObject.currentTarget.url );
    }

    // (OnMessage) Upon receiving NewsItem and add latest news item to top of the list.
    WebSocketHandle.onmessage = function (DataObject) {
        // Get MessageArea DOM div object
        var NewsTable = $('#NewsTable').DataTable();

        var message = JSON.parse(DataObject.data);
    
        // Get and append data from WebSocketHandle
        message.forEach(function (item) {
            var NewsTable = $('#NewsTable').DataTable();;
            NewsTable.row.add([item.Title,
            item.Description,
            "<a href='"+item.Link+"'>Link</a>",
            item.PublicationDate,
            item.GUID
            ]).draw(false);;
        });
    }
}


// Initialize DataTable and add
$(document).ready(function () {
    Connect();
    var t = $('#NewsTable').DataTable({
        "iDisplayLength": 5,
        "lengthMenu": [1, 3, 5, 10, 25, 50, 75, 100],
        "ordering": false,
        "columns": [
            { "width": "30%" },
            { "width": "40%" },
            { "width": "50px" },
            { "width": "150px" },
            { "width": "100px" }
        ]
    });
});
