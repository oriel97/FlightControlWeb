var ip = "http://localhost";
var port = "44364"; // !!!!!!!!!!!!!!!changeable?

function run() {
	get_flights_service();
	var intervalID = window.setInterval(get_flights_service, 3000);
}

function get_flights_service() {
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
	var url = "https://localhost:44364/api/Flights?relative_to=2019-05-08T08:51:58Z&sync_all";
	$.get(url, function (data) {
		var arr = data;
		$("#internal_list").empty();
		$("#external_list").empty();
		clearMarkers();
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
			add_onclick(id);


			var image = {
				url: "planeicon.png",
				// This marker is 20 pixels wide by 32 pixels high.
				size: new google.maps.Size(40, 40),
				// The origin for this image is (0, 0).
				origin: new google.maps.Point(0, 0),
				// The anchor for this image is the base of the flagpole at (0, 32).
				anchor: new google.maps.Point(0, 32)
			};

			addMarker({
				coords: { lat: f.get_latitude(), lng: f.get_longitude() },
				iconImage: image,
				animation: google.maps.Animation.BOUNCE,
				id: f.get_flight_id(),
			});
		}
		if (clicked != null) {
			var elem = document.getElementById(clicked);
			elem.style.border = "thin solid #0000ff";
		}
	});

}

class Flight {
	constructor(obj) {
		Object.assign(this, obj)
	}
	get_flight_id() { return this.flight_id; }
	get_longitude() { return this.longitude; }
	get_latitude() { return this.latitude; }
	get_passengers() { return this.passengers; }
	get_company_name() { return this.company_name; }
	get_date_time() { return this.date_time; }
	get_is_external() { return this.is_external; }
}