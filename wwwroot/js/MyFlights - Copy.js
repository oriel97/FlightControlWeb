var markerFlightsDict = {}
function createAirplanesIcons() {
	var myIcon = L.icon({
		iconUrl: 'blue.png',
		iconSize: [40, 40],
		// iconAnchor: [1, 24],
		popupAnchor: [-3, -76],
	});

	var myIcon2 = L.icon({
		iconUrl: 'red.png',
		iconSize: [50, 50],
		// iconAnchor: [1, 24],
		popupAnchor: [-3, -76],
	});
	return [myIcon, myIcon2];
}
function CloseButtonClicked() {

	var close = document.getElementsByClassName("close");
	var i;

	for (i = 0; i < close.length; ++i) {
		close[i].onclick = function () {
			var x = document.getElementsByTagName("li");
			for (var k = 0; k < x.length; k++) {
				var attr1 = document.createAttribute("class");
				var b = x[k].id;
				markerFlightsDict[String(b)].setIcon(myIcon);
			}
			var div = this.parentElement;
			if (div.isPressed == "false") {
				map.removeLayer(markerFlightsDict[String(div.id)]);
				div.style.display = "none";
				event.cancelBubble = true;
			}
			else {
				div.isPressed = "false";
				div.classList.toggle('checked');
				event.cancelBubble = true;

			}
		}
	}
}
function Clicked(data, i,marker, myIcon, myIcon2) {
	var x = document.getElementsByTagName("li");
	for (var k = 0; k < x.length; k++) {
		var attr1 = document.createAttribute("class");
		var h1 = document.getElementById(x[k].id);
		h1.setAttributeNode(attr1);
		var b = x[k].id;
		markerFlightsDict[String(b)].setIcon(myIcon);
	}
	marker.setIcon(myIcon2);
	document.getElementById("FlightDet").innerHTML = "Flight ID: " + data[i].flight_id +
		", Company Name:" + data[i].company_name;
	var attr = document.createAttribute("class");
	var h = document.getElementById(data[i].flight_id);
	h.setAttributeNode(attr);
	h.isPressed = "true";
	h.classList.toggle('checked');
}

function Airplanes(data){
	for (let i = 0; i < data.length; i++) {
		let lon = data[i]["longitude"];
		let lat = data[i]["latitude"];
		if (data[i]["flight_id"] in markerFlightsDict) {
			markerFlightsDict[data[i]["flight_id"]].setLatLng([lat, lon]);
		} else {
			var airplanes = createAirplanesIcons();
			var myIcon = airplanes[0];
			var myIcon2 = airplanes[1];


			let marker = L.marker([lat, lon], { icon: myIcon });
			marker.addTo(map);
			marker._leaflet_id = data[i].flight_id;
			map.addLayer(marker);
			marker.addEventListener("click", () => {
				Clicked(data, i, marker, myIcon, myIcon2);
				
			});
			markerFlightsDict[data[i]["flight_id"]] = marker;
		}
	}
	return [myIcon, myIcon2];
}
function createElements(data,ul) {
	for (let i = 0; i < data.length; i++) {
		var li = document.createElement("li");
		li.appendChild(document.createTextNode("Flight" + data[i].flight_id));
		var span = document.createElement("SPAN");
		var txt = document.createTextNode("\u00D7");
		span.className = "close";
		span.appendChild(txt);
		li.isPressed = "false";
		li.setAttribute("id", data[i].flight_id);
		ul.appendChild(li);
		li.appendChild(span);
		li.addEventListener("click", () => {
			Clicked(data, i, markerFlightsDict[data[i]["flight_id"]], myIcon, myIcon2);
		});
	}
}
function createFlightsList() {
	var xmlhttp = new XMLHttpRequest();

	var d = new Date();
	var hrs = d.getUTCHours();
	var mins = d.getUTCMinutes();
	var secs = d.getUTCSeconds();
	if (hrs < 10) {
		hrs = "0" + hrs.toString();
	} else { hrs = hrs.toString(); }
	if (mins < 10) {
		mins = "0" + mins.toString();
	} else { mins = mins.toString(); }
	if (secs < 10) {
		secs = "0" + secs.toString();
	} else { secs = secs.toString(); }

	var date = d.getUTCFullYear() + "-" + (d.getUTCMonth() + 1) + "-" + d.getUTCDate() + "T" + hrs + ":" + mins + ":" + secs + "Z";
	//var url = ip + ":" + port + "/api/Flights?relative_to=" + date + "&sync_all";
	var url = "https://localhost:44364/api/Flights?relative_to=2018-09-18T05:18:24Z&sync_all";
	$.get(url, function (data1) {
		arr = data1;
		for (var i in arr) {
			var f = new Flight(arr[i]);
			//put flight in list
			var id = f.get_flight_id();
			if (f.get_is_external() == true) {
				$("<li id='" + id + "'><div class='text'>" + id + "</div></li>").appendTo("#external_list");
			}
			else {
				$("<li id='" + id + "'><div class='text'>" + id + "</div><button class='x' onclick=\"delete_from_list('" + id + "')\">X</button></li>").appendTo("#internal_list");
			}
		}
		return Flights;
	});
	
/*	let Flights =	`[{
																 "flight_id": "[1111]",
																 "longitude": 33.244,
																 "latitude": 31.12,
																 "passengers": 216,
																 "company_name": "SwissAir",
																 "date_time": "2020-12-26T23:56:21Z",
																 "is_external": false
																},
																{
																 "flight_id": "[2222]",
																 "longitude": 33.50,
																 "latitude": 31.66,
																 "passengers": 216,
																 "company_name": "SwissAir",
																 "date_time": "2020-12-26T23:56:21Z",
																 "is_external": false
																}]`*/

}
function DrawIcons(data) {
	

	var arr = Airplanes(data);
	myIcon = arr[0];
	myIcon2 = arr[1];
	var ul = document.getElementById("nodeName");
	createElements(data,ul);

}
// Add a "checked" symbol when clicking on a list item
function AddCheckedSymbol() {
	var list = document.querySelector('ul');
	list.addEventListener('click', function (ev) {
		var checkList = document.getElementsByTagName("LI");
		var i;
		// Check if there is a "checked" symbol
		for (i = 0; i < checkList.length; i++) {
			if (checkList[i].isPressed == "true") {
				checkList[i].isPressed = "false";
				//checkList[i].classList.toggle('checked');
			}
		}
		// Add a "checked" symbol to the current item
		if (ev.target.tagName === 'LI') {
			if (ev.target.isPressed == "false") {
				ev.target.isPressed = "true";

			}
			else
				ev.target.isPressed = "false";
			//ev.target.classList.toggle('checked');

		}
	}, false);
}
AddCheckedSymbol();
CloseButtonClicked();
let Flights=createFlightsList();
var closebtns = document.querySelectorAll(".close");
Array.from(closebtns).forEach(item => {
	item.addEventListener("click", () => {
		item.parentElement.style.display = "none";
	});
});
DrawIcons(JSON.parse(Flights));
CloseButtonClicked();
