#!/bin/bash
docker stop newsreader
docker rm newsreader
sed -i "s/ws:\/\/localhost\:57293/ws\:\/\/newsreader.gpn.mx/g" wwwroot/js/site.js
docker build -t newsreader .
docker run -d -p 5000:80 --name newsreader newsreader