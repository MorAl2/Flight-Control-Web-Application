let flag = 0;
let port = "localhost";
let ip = "55997";

let defaultResponse = {
    status: Number
};

defaultResponse.status = 200;

function load() {
    //Executes the Get command for all active flights once every 3 seconds
    setInterval(getFunc, 1500);
    //Settings for the drag area
    let dropArea = document.getElementById('drop-area');
    let f = document.getElementById('form');
    // Defines a function that handles drag
    dropArea.addEventListener('drop', handleDrop, false)
        ;['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropArea.addEventListener(eventName, preventDefaults, false)
        })
    // Cancels the default browser behavior
    function preventDefaults(e) {
        e.preventDefault()
        e.stopPropagation()
    }
    ['dragenter', 'dragover'].forEach(eventName => {
        dropArea.addEventListener(eventName, highlight, false)
    });
    ['dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, unhighlight, false)
    });
    // Formats the object when there is a file dragged into it
    function highlight(e) {
        dropArea.classList.add('highlight');
        f.style.display = "none";
    }
    // Cancels the design if there is no file dragged into it
    function unhighlight(e) {
        dropArea.classList.remove('highlight');
    }
}
// A function that handles a file drag event
function handleDrop(e) {
    document.getElementById('form').style.display = "block";
    let dt = e.dataTransfer
    let files = dt.files
    // A function that uploads each file separately
    handleFiles(files)
}

function handleFiles(files) {

    ([...files]).forEach(uploadFile)
}
// A function that uploads a file to the server using the Post command
function uploadFile(file) {
    let reader = new FileReader();
    // Function to execute after reading the file as text
    reader.onload = function (event) {
        //Address to send by post command
        let postOptions = preparePost(event.target.result);
        //Executes the post command with the relevant parameters
        try {
            fetch("http://" + port + ":" + ip + "/api/FlightPlan", postOptions)
                .then(Response => handleHTTPErrors(Response))
                // catch error of http status
                .catch(e => {
                    handleErrorsOthers(e);
                });
        }
        // fetch failed 
        catch (err) { handleErrors(err) }
    };
    // A function that reads the file as text
    reader.readAsText(file); 
}
//A function that creates the parameters of the post request
function preparePost(todo) {
    let ContentType = 'application/json;charset=utf-8';
    return {
        "method": "POST",
        "mode": 'cors',
        "credentials": 'same-origin',
        "headers": {
            'Content-Type': ContentType,
        },
        "body": todo
    }
}

//activate the HTTP GET Request every second, and sync all the servers.
function getFunc() {
    //console.log("inside getFunc");
    const time = new Date;
    //Converts the current time to the required format in the exercise
    let timeToSend = time.toISOString().split('.')[0] + "Z"
    timeToSend = "http://" + port + ":" + ip + "/api/Flights?relative_to="
        + timeToSend + "&sync_all";
    async function getFlightsAsync() {
        //console.log("inside getFlightsAsync");
        //Executes the GET command and saves the result
        try {
            let response = await fetch(timeToSend);
            if (!response.ok) {
                handleHTTPErrors(response);
            } else {
                $(".myAlert-getSyncALL").hide();
                // Convert the result to a Jason object
                let data = await response.json()
                createMarkerFromFlight(data); //added by Gal
                refreshMap();
                return data;
            }

            }
            catch(err){
            handleErrors(err);
        }
    }
    //A function that checks if the information is empty
    //and sends it to another function that fills the table
    getFlightsAsync()
        .then(data => {
            if (data && data.length) {
                populateFlights(data)
            }
        });
 }

function populateFlights(json) {
    //console.log("inside populateFlights");
    //createMarkerFromFlight(json);
    // Gets a pointer to the body of the table
    let FlightsBody = document.querySelector("#FlightsTable > tbody");
    //A flag intended to check if a click was made
    let press = 0;
    //Empty the table from the existing content
     while (FlightsBody.firstChild) {
        //Check if the current row in the table is highlighted
         if (press == 0 && FlightsBody.firstChild.firstChild != null)
         {
             press = checkPress(FlightsBody.firstChild);            
         }
         //Delete the current row
        FlightsBody.removeChild(FlightsBody.firstChild);
    }
     
    //Produces the table rows again
    json.forEach((row) => {
        createRow(row, FlightsBody, press);
    });
}
//A function in which a row checks to see if it is highlighted
//- then returns its ID, otherwise it returns - 0
function checkPress(row) {
   // console.log("inside checkPress");
    if (row.style.backgroundColor == "rgb(108, 122, 137)") {
        return row.firstChild.textContent;
    }
    return 0;
}

function createRow(row, FlightsBody, press) {
   // console.log("inside createRow");
    //Definition of delete button
    let img = document.createElement('img');
    img.src = "delete.png";
    img.id="delImg";
    img.height = 30;
    img.width = 25;
    img.align = "left";
    //Define a new row in the table
    let tr = document.createElement("tr");
    //Set cell for row in table
    let tdID = document.createElement("td");
    //Placing the appropriate value in each cell that has a row in the table
    tdID.textContent = row.flight_id;
    let tdAirline = document.createElement("td");
    tdAirline.textContent = row.company_name;
    let tdExternal = document.createElement("td");
    tdExternal.textContent = row.is_external;
    tdExternal.textContent = tdExternal.textContent.charAt(0).toUpperCase()
        + tdExternal.textContent.slice(1);
    let tdDelete = document.createElement("td");
    tdDelete.textContent = ""; 
    //If the flight is not external, we will add the delete button for it
    if (row.is_external == false) {
        tdDelete = img;
    }
    //Check if the current row ID is the same as the previously submitted row ID -
    //If so, highlight it again
    if (press != 0 && row.flight_id == press) tr.style.backgroundColor = "rgb(108, 122, 137)";
    //Insert the cells into the new row
    tr.appendChild(tdID);
    tr.appendChild(tdAirline);
    tr.appendChild(tdExternal);
    tr.appendChild(tdDelete);
    //Define handling of a click event
    tr.onclick = function () {
        clickRow(event) 
    };
    FlightsBody.appendChild(tr);
}

function clickRow(event) {
    //console.log("inside clickRow");
    let FlightsBody = document.querySelector("#FlightsTable > tbody");
   // console.log("FlightsBody = " + FlightsBody);
    //Saves the ID of the clicked row
    let CellClickID = event.srcElement.parentElement.firstChild.textContent;
    let src = event.srcElement;
    //Checking for a click (image of the delete button)
    if (src instanceof HTMLImageElement) {
        //  A call to a function that wants to delete from the server
        deleteFlight(CellClickID);
        removeTrack(CellClickID);
        removeMarkerById(CellClickID);
        //  Delete the row from the table
        FlightsBody.removeChild(event.srcElement.parentElement);
        //Deletes flight information if needed
        DeleteUpdateFlightDetailsFromClient(event.srcElement.parentElement);
        //If clicked on another part of the line
    } else {
        if (flag == 0) {
            //removeTrack(data["flight_id"]); //added by Gal
            resetMarkers();
            //generateTrackFromId(data["flight_id"], data); //added by Gal
        }
        //Highlight the line
        pressRow(CellClickID)
        // Fetch flight details with GET request + ID
        getFlightPlanByID(CellClickID);

    }
}
function deleteFlight(data) {
    //console.log("inside deleteFlight");
    let url = "http://" + port + ":" + ip + "/api/Flights/" + data;
    async function getFlightsAsync() {
        //Requests to delete the relevant flight from the server
        let response = await fetch(url, { method: 'DELETE' });
        let data = await response.text()
        return data;
    }
    getFlightsAsync()
        .then(data => {
            //console.log(data)
        })
}

function pressRow(CellClickID) {
    let FlightsBody = document.querySelector("#FlightsTable > tbody");
    let i = 0;
    let row;
    //Go over the table rows, remove an existing highlight,
    //and perform a new highlight to the relevant row
    for (i; i < FlightsBody.childElementCount; i++) {
        row = FlightsBody.rows[i];
        if (row.style.backgroundColor == "rgb(108, 122, 137)") {
            row.style.backgroundColor = "";
        }
        if (row.firstChild.innerText == CellClickID) {
            row.style.backgroundColor = "rgb(108, 122, 137)";
        }
    }
}

function getFlightPlanByID(id) {
    //console.log("inside getFlightPlanByID");
    let url = "http://" + port + ":" + ip + "/api/FlightPlan/" + id;
    async function getFlightsAsync() {
        try {
            //Execute the get request
            let response = await fetch(url);
            if (!response.ok) {
                handleHTTPErrors(response);
            } else {
                //Convert the answer to the Jason object
                let data = await response.json()
                return data;
            }
        } catch (err) {
            handleErrorsOthers(err);
        }
    }
    getFlightsAsync()
        .then(data => {
            //Update flight details
            updateFlightDetailsFromServer(data, id)
        })
        .catch(e => {
            handleErrorsOthers(e.message);
        })
}
function updateFlightDetailsFromServer(data, flightID) {
    //console.log("inside updateFlightDetailsFromServer");
    if (flag == 0) {
        generateTrackFromId(flightID, data);
    }    
    let landingTime = calculateLandingTime(data["initial_location"]["date_time"], data);
    document.getElementById("comp").innerHTML = data.company_name;
    document.getElementById("initLong").innerHTML = "(" + data.initial_location.longitude + ", ";  
    document.getElementById("initLatit").innerHTML = data.initial_location.latitude + ")";
    document.getElementById("EndTime").innerHTML = landingTime.toISOString();
    document.getElementById("StartTime").innerHTML = data.initial_location.date_time;
    document.getElementById("pass").innerHTML = data.passengers;
    document.getElementById("finalLong").innerHTML = "("
        + data.segments[data.segments.length - 1].longitude + ", ";
    document.getElementById("finalLatit").innerHTML =
        data.segments[data.segments.length - 1].latitude + ")";
    flag = 0
}
function DeleteUpdateFlightDetailsFromClient(data) {
    //console.log("inside DeleteUpdateFlightDetailsFromClient");
    //Checks whether the line we want to delete is highlighted - then deletes flight information
    if (data.style.backgroundColor == "rgb(108, 122, 137)") {
        document.getElementById("comp").innerHTML = "";
        document.getElementById("initLong").innerHTML = "";
        document.getElementById("initLatit").innerHTML = "";
        document.getElementById("EndTime").innerHTML = "";
        document.getElementById("StartTime").innerHTML = "";
        document.getElementById("pass").innerHTML = "";
        document.getElementById("finalLong").innerHTML = "";
        document.getElementById("finalLatit").innerHTML = "";
    }
}

function handleErrors(error) {
    let errorMsg = ". Please click on the map to reload or try again."
    if (error == "TypeError: Failed to fetch") {
        document.getElementById("getSyncALL").innerHTML = "The Server is disconnected!";
    } else {
        document.getElementById("getSyncALL").innerHTML = error + errorMsg;
    }
        $(".myAlert-getSyncALL").display = "block";
        $(".myAlert-getSyncALL").show();
        $(".myAlert-getSyncALL").offset().top;
}

function handleErrorsOthers(error) {
    let errorMsg = ". Please click on the map to reload or try again."
    if (error == "TypeError: Failed to fetch") {
        document.getElementById("getSyncALL").innerHTML = "The Server is disconnected!";
        $(".myAlert-getSyncALL").display = "block";
        $(".myAlert-getSyncALL").show();
        $(".myAlert-getSyncALL").offset().top;
    } else {
        document.getElementById("others").innerHTML = error + errorMsg;
        $(".myAlert-other").display = "block";
        $(".myAlert-other").show();
        $(".myAlert-other").offset().top;
    }

}

function handleHTTPErrors(error) {

    // remove the error in case of good response status.  
    if (error.status >= 200 && error.status <= 299) {
        $(".myAlert-other").hide();
    }
    // throw relvant status error if needed.
    if (error.status >= 300 && error.status <= 399) {
        console.log(error);
        throw Error("Redirects Error - " + error.status);
    }
    if (error.status >= 401 && error.status <= 499) {
        console.log(error);
        throw Error("Client errors - " + error.status);
    }
    if (error.status >= 500 && error.status <= 599) {
        console.log(error);
        throw Error("Server errors - " + error.status);
    }
    if (error.status == 400) {
        console.log(error);
        throw Error("Error - " + error.statusText);
    }
}

//map decleration.
let map = L.map('map').setView([0, 0], 1);
map.setZoom(6);
map.panTo(new L.LatLng(33.244, 31.12));

//icons
let pressedIcon = new L.Icon({
    iconUrl: 'planex64.png',
    iconSize: [45, 61],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [61, 61]
});

let startingIcon = new L.Icon({
    iconUrl: 'planex64.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});


let markers = [] //list of all existing markers.
let polylines = new Set() //list of polylines added.
let trackObj = { //a track of a single flight.
    id: String,
    track: []
};

//reset the track.
trackObj.id = "0";
trackObj.track.length = 0;
let polyline = null; //will be the track layer.

let updateMarker = 0; //when getting a new flight - check if it's already exist.



L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=VW6elJ2db2FGep8QI3fc', {
    attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy;'
        + 'MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy;'
        + 'OpenStreetMap contributors</a>'
}).addTo(map);
map.on('click', resetMarkers)

// create marker object from given locatin, flight id and starting time.
function renderMarker(location, id, time) { 
    //change the location of the given marker if it's already exist.
    markers.forEach(function (marker) { 
        if (marker.id === id) {
            marker.setLatLng(location).update();
            updateMarker = 1;
            marker.onAir = 1;
        }
    })
    if (updateMarker == 0) { //if we didn't find this marker, we create a new one.
        let marker = L.marker(location, { icon: startingIcon });
        let d = new Date(time);
        marker.id = id;
        marker.startingLocation = location;
        marker.startingTime = d;
        marker.onAir = 1;
        marker.addEventListener('click', selectMarker);
        markers.push(marker);
    }
    updateMarker = 0;
}

function addAllToMap() { //add all flights to the map layers.
    //console.log("inside addAllToMap");
    let i = 0;
    for (i; i < markers.length; i++) {
        map.addLayer(markers[i]);
    }
}

function selectMarker() { //resize the marker and displays its track.
    //console.log("inside selectMarker");
    let thisFlightId = this.id;
    let i = 0;
    for (i; i < markers.length; i++) {
        if (markers[i].id === thisFlightId) {
            markers[i].setIcon(pressedIcon);
            renderTrack(markers[i]);
        } else {
            markers[i].setIcon(startingIcon);
        }
    }
}

function resetMarkers() { //reset size for all markers clear the track when map is clicked.
    let i = 0
    for (i; i < markers.length; i++) {
        markers[i].setIcon(startingIcon);
    }
    trackObj.id = "0"; //reset track id
    trackObj.track.length = 0; //reset the track
    if (polyline != null) {
        map.removeLayer(polyline);
        polyline = null;
    }

    map.eachLayer(function (layer) {
        if (polylines.has(layer["_leaflet_id"])) {
            polylines.delete(layer["_leaflet_id"]);
            map.removeLayer(layer);
        }
    });
    //remove The Track itself
    resetDetails();
    handleHTTPErrors(defaultResponse);
}

function renderTrack(marker) { //function for displaing the flight track
    if (trackObj.track.length != 0) { //we have some track on the map.
        map.eachLayer(function (layer) {
            if (polylines.has(layer["_leaflet_id"])) {
                //console.log("find object with id = " + layer["_leaflet_id"])
                polylines.delete(layer["_leaflet_id"]);
                map.removeLayer(layer);
            } else {
                //console.log("didn't find object with id = " + layer["_leaflet_id"])
            }
        });

        //remove the current track
        trackObj.id = "0";
        trackObj.track.length = 0;
        map.removeLayer(polyline); 
        polyline = null;

    } //no truck is beeing shown currently.
        let wantedId = marker.id;
        resetDetails(wantedId);
        let path = "http://" + port + ":" + ip + "/api/FlightPlan/" + wantedId;
        flightPlanGetter(path);
        async function flightPlanGetter(wantedPath) {
            try {
                let response = await fetch(wantedPath);
                if (!response.ok) {
                    handleHTTPErrors(response);
                } else {
                    let data = await response.json();
                    fillTrack(marker, data);
                }
            } catch (err) {
                handleErrorsOthers(err);
            }
        }
    
}

function fillTrack(flight, flightPlan) { //drawing the track on the map.   
    //console.log("inside fillTrack");
    flag = 1;
    trackObj.id = flight.id;
    trackObj.track.push(flight.startingLocation);
    let i = 0;
    for (i; i < flightPlan["segments"].length; i++) {
        let lon = parseFloat(flightPlan["segments"][i]["longitude"]);
        let lat = parseFloat(flightPlan["segments"][i]["latitude"]);
        trackObj.track.push([lat, lon]);
    }
    polyline = L.polyline(trackObj.track, { color: '#69ff33' });
    polylines.add(polyline._leaflet_id);
    map.addLayer(polyline);
    polylines.add(polyline._leaflet_id);
    pressRow(trackObj.id);
    updateFlightDetailsFromServer(flightPlan, trackObj.id); 
}


function calculateLandingTime(dateAsString, flightPlan) {
    //console.log("inside calculateLandingTime");
    let landing = new Date(dateAsString);
    let totalTimeSpend = 0;
    let i = 0;
    for (i; i < flightPlan["segments"].length; i++) {
        totalTimeSpend += parseInt(flightPlan["segments"][i]["timespan_seconds"]);                 
    }
    landing.setSeconds(landing.getSeconds() + totalTimeSpend);
    return landing;
}

//the next function "createMarkerFromFlight" recieve *Flight* as data
//gets flight object and extract it's details for renderMarker function.!
function createMarkerFromFlight(data) { 
    let i = 0;
    for (i; i < markers.length; i++) {
        markers[i].onAir = 0;
    }
    for (i = 0; i < data.length; i++) {
        let lon = parseFloat(data[i]["longitude"]);
        let lat = parseFloat(data[i]["latitude"]);
        renderMarker([lat, lon], data[i]["flight_id"], data[i]["date_time"]);
    }

    //removing landed flights 
    for (i = 0; i < markers.length; i++) {
        if (markers[i].onAir == 0) {
            removeTrack(markers[i].id);
            map.removeLayer(markers[i]);
            markers.splice(i, 1);
        }
    }
}

function removeTrack(idToCheck) { //clear track if it's flight has been deleted.
    //console.log("inside removeTrack");
    if (trackObj.id === idToCheck) {
        trackObj.id = "0"; //reset track id
        trackObj.track.length = 0; //reset the track
        map.removeLayer(polyline); //remove The Track itself
        polyline = null;
    }
}


function refreshMap() {//keeps the map updated.
    if (markers.length != 0) {
        addAllToMap();
    }
}

//the following function called when line is pressed.
//fId - the id of the line we pressed on.
//flightPlan - the relevent flight plan
function generateTrackFromId(fId, flightPlan) {

    //display the relevent marker.
    let i = 0;
    for (i; i < markers.length; i++) {
        // 
        if (markers[i].id === fId) {
            markers[i].setIcon(pressedIcon);
        } else {
            markers[i].setIcon(startingIcon);
        }
    }

    //finding the relevant marker
    let thisFlight;
    for (i = 0; i < markers.length; i++) {
        if (markers[i].id === fId) {
            thisFlight = markers[i];
            break;
        }
    }

    trackObj.id = fId;
    trackObj.track.push(thisFlight.startingLocation);

    //runs over the segments for adding them to the track and calculate time in the air.
    for (i = 0; i < flightPlan["segments"].length; i++) {
        let lon = parseFloat(flightPlan["segments"][i]["longitude"]);
        let lat = parseFloat(flightPlan["segments"][i]["latitude"]);
        trackObj.track.push([lat, lon]);
    }

    polyline = L.polyline(trackObj.track, { color: '#69ff33' });
    map.addLayer(polyline);//drawing the track.
    polylines.add(polyline._leaflet_id);
}

//deleting marker when his relevant line is pressed.
function removeMarkerById(wantedId) {
    let i = 0;
    for (i; i < markers.length; i++) {
        if (markers[i].id === wantedId) {
            map.removeLayer(markers[i]);
            markers.splice(i, 1);
            break;
        }
    }
}

function resetDetails() {
    // Deleting a highlighted row from the table
    pressRow("");
    // Updating flight information shown
    document.getElementById("comp").innerHTML = "";
    document.getElementById("initLong").innerHTML = "";
    document.getElementById("initLatit").innerHTML = "";
    document.getElementById("EndTime").innerHTML = "";
    document.getElementById("StartTime").innerHTML = "";
    document.getElementById("pass").innerHTML = "";
    document.getElementById("finalLong").innerHTML = "";
    document.getElementById("finalLatit").innerHTML = "";
}