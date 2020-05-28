let flag = 0;

function load() {
    //מבצע את הפקודה גט עבור כל הטיסות הפעילות אחת ל- 3 שניות
    setInterval(getFunc, 8000);
    //הגדרות עבור האזור גרירה
    let dropArea = document.getElementById('drop-area');
    let t = document.getElementById('FlightsTable');
    // מגדיר פונקציה לטיפול באירוע גרירה 
    dropArea.addEventListener('drop', handleDrop, false)
        ;['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropArea.addEventListener(eventName, preventDefaults, false)
        })

    // מבטל את ההתנהגות ברירת מחדל של הדפדפן
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
    // מעצב את האובייקט כאשר יש קובץ שנגרר לתוכו
    function highlight(e) {
        dropArea.classList.add('highlight');
        t.style.display = "none";
    }
    // מבטל את העיצוב במידה ואין קובץ שנגרר לתוכו
    function unhighlight(e) {
        dropArea.classList.remove('highlight');
       // t.style.display = "block";
    }
}
// פונקציה המטפלת באירוע גרירת קובץ
function handleDrop(e) {
    document.getElementById('FlightsTable').style.display = "block";
    let dt = e.dataTransfer
    let files = dt.files
    // שליחת לפונקציה שמעלה כל קובץ בנפרד
    handleFiles(files)
}

function handleFiles(files) {

    ([...files]).forEach(uploadFile)
}
// פונקציה שמעלה קובץ לשרת באמצעות פוסט
function uploadFile(file) {
    var reader = new FileReader();
    let jsonText;
    let ContentType = 'application/json;charset=utf-8';
    // פונקציה לביצוע אחרי סיום קריאת הקובץ כטקסט 
    reader.onload = function (event) {
        //הקובץ טקסט שקראתי
        jsonText = event.target.result
        //כתובת לשליחה בפוסט
        let url = "http://localhost:55997/api/FlightPlan"
        let postOptions = preparePost(jsonText);
        //פונקציה היוצרת את הפרמטרים של בקשת הפוסט
        function preparePost(todo) {
            let todoAsSTR = JSON.stringify(todo);
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
        //מבצע את פקודת הפוסט עם הפרמטרים הרלוונטיים
        fetch(url, postOptions)
            .then(Response => Response.text())
            .then(Response => console.log("response: " + Response))
    };
    // פונקציה לביצוע קריאת הקובץ כטקסט
    reader.readAsText(file); 
}
function getFunc() {
    const time = new Date;
    //המרת הזמן הנוכחי לפורמט הנדרש בתרגיל
    let timeToSend = time.toISOString().split('.')[0] + "Z"
    timeToSend = "http://localhost:55997/api/Flights?relative_to=" + timeToSend +"&sync_all";
    async function getFlightsAsync() {
        //מבצע את הפעולה גט ושומר את התוצאה
        let response = await fetch(timeToSend);
        // המרת התוצאה לאוביקט גייסון
        let data = await response.json()
        createMarkerFromFlight(data); //added by Gal
        refreshMap();
        return data;
    }
    //פונקציה שבודקת אם המידע ריק ושולחת אותה לפונקציה נוספת שתמלא את הטבלה
    getFlightsAsync()
        .then(data => {
            if (data && data.length) {
                populateFlights(data)
            }
            })
 }

function populateFlights(json) {
    //createMarkerFromFlight(json);
    // מקבלת מצביע לגוף של הטבלה
    let FlightsBody = document.querySelector("#FlightsTable > tbody");
    //דגל שמטרתו לבדוק אם התבצעה לחיצה
    let press = 0;
    //ריקון הטבלה מהתוכן הקיים
     while (FlightsBody.firstChild) {
        //בדיקה אם השורה הנוכחית בטבלה מודגשת
         if (press == 0 && FlightsBody.firstChild.firstChild != null)
         {
             press = checkPress(FlightsBody.firstChild);            
         }
        //מחיקת השורה הנוכחית
        FlightsBody.removeChild(FlightsBody.firstChild);
    }
     
    //מייצר את שורות הטבלה מחדש
    json.forEach((row) => {
        createRow(row, FlightsBody,press);
    });
}
//פונקציה שבהנתן שורה בודקת אם היא מודגשת- אם כן מחזירה את המזהה שלה, אחרת-0
function checkPress(row) {
    if (row.style.backgroundColor == "rgb(108, 122, 137)") {
        return row.firstChild.textContent;
    }
    return 0;
}

function createRow(row, FlightsBody,press)
{
    //הגדרת אייקון המחיקה
    let img = document.createElement('img');
    img.src = "delete.png";
    img.id="delImg";
    img.height = 30;
    img.width = 25;
    img.align = "left";
    //הגדרת שורה חדשה בטבלה
    const tr = document.createElement("tr");
    //הגדרת תא
    const tdID = document.createElement("td");
    //השמת הערך המתאים בכל תא שיש בשורה בטבלה
    tdID.textContent = row.flight_id;
    const tdAirline = document.createElement("td");
    tdAirline.textContent = row.company_name;
    const tdExternal = document.createElement("td");
    tdExternal.textContent = row.is_external;
    tdExternal.textContent = tdExternal.textContent.charAt(0).toUpperCase() + tdExternal.textContent.slice(1);
    //אם אכן הטיסה פנימית- נוסיף עבורה את אייקון המחיקה
    if (row.is_external == false) {
        tdDelete = img;
    }
    //בדיקה האם המזהה של השורה הוא המזהה של השורה המודגשת בעבר- ואם כן מדגיש אותה שוב
    if (press != 0 && row.flight_id == press) tr.style.backgroundColor = "#6C7A89";
    //הכנסת התאים לשורה החדשה
    tr.appendChild(tdID);
    tr.appendChild(tdAirline);
    tr.appendChild(tdExternal);
    tr.appendChild(tdDelete);
    //הגדרת טיפול באירוע לחיצה
    tr.onclick = function () {
        //שמירת של המזהה של השורה שנלחצה
        let CellClickID = event.srcElement.parentElement.firstChild.textContent;
        let src = event.srcElement;
        //(בדיקה האם הלחיצה התבצעה על תמונה (של אייקון המחיקה
        if (src instanceof HTMLImageElement) {
            //קריאה לפונקציה שמבקשת לבצע מחיקה מהשרת
            deleteFlight(CellClickID);
            removeTrack(CellClickID);
            removeMarkerById(CellClickID);
            //מחיקת השורה מהטבלה
            FlightsBody.removeChild(event.srcElement.parentElement);
            //מוחק את פרטי הטיסה במידת הצורך
            DeleteUpdateFlightDetailsFromClient(event.srcElement.parentElement);
        //אם הלחיצה התבצעה על חלק אחר של השורה 
        } else {
            if (flag == 0) {
                //removeTrack(data["flight_id"]); //added by Gal
                resetMarkers();
                //generateTrackFromId(data["flight_id"], data); //added by Gal
            }
            //הדגשת השורה
            pressRow(CellClickID)
            // הבאת פרטי הטיסה באמצעות בקשת גט+מזהה
            getFlightPlanByID(CellClickID);
            
        }
    };
    FlightsBody.appendChild(tr);
}

function deleteFlight(data) {
    url = "http://localhost:55997/api/Flights/" + data;
    async function getFlightsAsync() {
        //מבצע בקשה למחיקה של הטיסה הרלוונטית מהשרת 
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
    var i = 0;
    let row;
    //מעבר על שורות הטבלה, הסרת הדגשה קיימת וביצוע הדגשה חדשה לשורה הרלוונטית
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
    url = "http://localhost:55997/api/FlightPlan/" + id;
    async function getFlightsAsync() {
        //ביצוע בקשת הגט
        let response = await fetch(url);
        //המרת התשובה אובייקט גייסון
        let data = await response.json()
        return data;
    }
    getFlightsAsync()
        .then(data => {
            //עידכון פרטי הטיסה
            updateFlightDetailsFromServer(data)
        })
}
function updateFlightDetailsFromServer(data) {
    if (flag == 0) {
        //removeTrack(data["flight_id"]); //added by Gal
        //resetMarkers();
        generateTrackFromId(data["flight_id"], data); //added by Gal
    }    
    let landingTime = calculateLandingTime(data["initial_location"]["date_time"], data);
    document.getElementById("comp").innerHTML = data.company_name;
    document.getElementById("initLong").innerHTML = "(" + data.initial_location.longitude + ", ";
    document.getElementById("initLatit").innerHTML = data.initial_location.latitude + ")";
    document.getElementById("EndTime").innerHTML = landingTime.toISOString();
    document.getElementById("StartTime").innerHTML = data.initial_location.date_time;
    document.getElementById("pass").innerHTML = data.passengers;
    document.getElementById("finalLong").innerHTML = "(" + data.segments[data.segments.length - 1].longitude + ", ";
    document.getElementById("finalLatit").innerHTML = data.segments[data.segments.length - 1].latitude + ")";
    flag = 0
}
function DeleteUpdateFlightDetailsFromClient(data) {
    //בודק אם השורה שברצוננו למחוק מודגשת- אם כן- מוחק את פרטי הטיסה
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



//הקוד של גל


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
let trackObj = { //a track of a single flight.
    id: String,
    track: []
};

//reset the track.
trackObj.id = "0";
trackObj.track.length = 0;
let polyline; //will be the track layer.

let updateMarker = 0; //when getting a new flight - check if it's already exist.



L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=VW6elJ2db2FGep8QI3fc', {
    attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
}).addTo(map);
map.on('click', resetMarkers)

function renderMarker(location, id, time) { // create marker object from given locatin, flight id and starting time.
    markers.forEach(function (marker) { //change the location of the given marker if it's already exist.
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
    for (i = 0; i < markers.length; i++) {
        map.addLayer(markers[i])
    }
}

function selectMarker() { //resize the marker and displays its track.
    let thisFlightId = this.id;
    for (i = 0; i < markers.length; i++) {
        if (markers[i].id === thisFlightId) {
            markers[i].setIcon(pressedIcon);
            renderTrack(markers[i]);
        } else {
            markers[i].setIcon(startingIcon);
        }
    }
}

function resetMarkers() { //reset size for all markers clear the track when map is clicked.
    //TODO - clear flight details and unpress it in the table.
    for (i = 0; i < markers.length; i++) {
        markers[i].setIcon(startingIcon);
    }
    if (trackObj.id !== "0") {
        trackObj.id = "0"; //reset track id
        trackObj.track.length = 0; //reset the track
        map.removeLayer(polyline); //remove The Track itself
    }
    resetDetails(); //***************************************************************
}

function renderTrack(marker) { //function for displaing the flight track
    if (trackObj.track.length != 0) { //we have some track on the map.
        if (trackObj.id !== marker.id) { //different track then the pressed one.
            trackObj.id = "0";
            trackObj.track.length = 0;
            map.removeLayer(polyline); //remove the current track
            let wantedId = marker.id;
            resetDetails(wantedId);
            let path = "http://localhost:55997/api/FlightPlan/" + wantedId;
            flightPlanGetter(path);
            async function flightPlanGetter(wantedPath) {
                let response = await fetch(wantedPath).catch(handleErr);
                let data = await response.json();
                fillTrack(marker, data);
            }
        }
    } else { //no truck is beeing shown currently.
        let wantedId = marker.id;
        resetDetails(wantedId);
        let path = "http://localhost:55997/api/FlightPlan/" + wantedId;
        flightPlanGetter(path);
        async function flightPlanGetter(wantedPath) {
            let response = await fetch(wantedPath).catch(handleErr);
            let data = await response.json();
            fillTrack(marker, data);
        }
    }
}

function fillTrack(flight, flightPlan) { //drawing the track on the map.    
    flag = 1;
    trackObj.id = flight.id;
    trackObj.track.push(flight.startingLocation);
    for (i = 0; i < flightPlan["segments"].length; i++) {
        let lon = parseFloat(flightPlan["segments"][i]["longitude"]);
        let lat = parseFloat(flightPlan["segments"][i]["latitude"]);
        trackObj.track.push([lat, lon]);
    }
    polyline = L.polyline(trackObj.track, { color: '#69ff33' });
    map.addLayer(polyline);
    pressRow(trackObj.id, flightPlan); //****************************************************************************
    updateFlightDetailsFromServer(flightPlan); //********************************************************
}


function calculateLandingTime(dateAsString, flightPlan) {
    let landing = new Date(dateAsString);
    let totalTimeSpend = 0;
    for (i = 0; i < flightPlan["segments"].length; i++) {
        totalTimeSpend += parseInt(flightPlan["segments"][i]["timespan_seconds"]);
    }
    landing.setSeconds(landing.getSeconds() + totalTimeSpend);
    return landing;
}

//the next function "createMarkerFromFlight" recieve *Flight* as data!
function createMarkerFromFlight(data) { //gets flight object and extract it's details for renderMarker function.

    for (i = 0; i < markers.length; i++) {
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
    if (trackObj.id === idToCheck) {
        trackObj.id = "0"; //reset track id
        trackObj.track.length = 0; //reset the track
        map.removeLayer(polyline); //remove The Track itself
    }
}

function handleErr(err) { //handle errors from the json file
    console.warn(err);
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
    //resetDetails(fId);
    //display the relevent marker.
    for (i = 0; i < markers.length; i++) {
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

}

//deleting marker when his relevant line is pressed.
function removeMarkerById(wantedId) {
    for (i = 0; i < markers.length; i++) {
        if (markers[i].id === wantedId) {
            map.removeLayer(markers[i]);
            markers.splice(i, 1);
            break;
        }
    }
}

function resetDetails() {
    // מוחק שורה מסומנת מהטבלה
    pressRow("");
    // מעדכן את פרטי הטיסה המוצגים 
    document.getElementById("comp").innerHTML = "";
    document.getElementById("initLong").innerHTML = "";
    document.getElementById("initLatit").innerHTML = "";
    document.getElementById("EndTime").innerHTML = "";
    document.getElementById("StartTime").innerHTML = "";
    document.getElementById("pass").innerHTML = "";
    document.getElementById("finalLong").innerHTML = "";
    document.getElementById("finalLatit").innerHTML = "";
}