# Flight-Control-Web-Application
REST And Bootstrap Based Web Application

Using C# to build and maintain the server side, and JS and HTML to maintin the client side off the application, we builed an app that represent real time sky state in terms of on-air flights.

## Server Side
We Recieved flight and servers data as Json file, parse it and validate it.

Beside to that, we kept real time state of both flights and different servers in our DB.

Each Flight recieved an identifier number (i.e EA1432) in order to follow it.

Based on REST api, we maintained the connection with the client, and for every GET request we returned both flight and flight plans, from our local server and from distanced servers we capt connection with, for every flight that was on air at the moment of the Request.

## Client Side
We Built the client side's logic using JS, and the visual part of the app using bootstrap, css and html.

On the client side we allowed a drag and drop zone for fetching json files that contains flight details.

On the next step, we validated it and sent it to our local server in order to save it, and displayed the details on the screen - via line with some specific detaild such as flight number, airline company and if its on our server or not, and via plain logo that is shown in the real location on the map.

Each flight flight plan contains track represented by sergments, and with press on either the line or the logo, the track is shown at same time with the line and the plain are marked.

We used Async and Await for fetching data from the server, and Eslint tools to maintin coding style. 


![alt text](https://github.com/MorAl2/Flight-Control-Web-Application/blob/master/IndexPhoto.jpg?raw=true)
