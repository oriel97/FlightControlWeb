 /*  var onMarkerClick = function(e){
            alert("You clicked on marker with customId: " +this.options.myCustomId);   
        }
        	L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=dwAzStO5BTv3oNPggfCv', {
        			attribution:'<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'}).addTo(map);
setTimeout(function () { map.invalidateSize() }, 10);
map.on('click', function () {
    var x = document.getElementsByTagName("li");
    for (var k = 0; k < x.length; k++) {
        var attr1 = document.createAttribute("class");
        var h1 = document.getElementById(x[k].id);
        h1.setAttributeNode(attr1);
        var b = x[k].id;
        markerFlightsDict[String(b)].setIcon(myIcon);
    }
});*/

var map = L.map('map', { minZoom: 3, }).setView([33, 31], 5);
mapLink =
	'<a href="http://openstreetmap.org">OpenStreetMap</a>';
L.tileLayer(
	'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
	attribution: '&copy; ' + mapLink + ' Contributors',
	maxZoom: 18,
}).addTo(map);
map.on('click', function () {
	var x = document.getElementsByTagName("li");
	for (var k = 0; k < x.length; k++) {
		var attr1 = document.createAttribute("class");
		var h1 = document.getElementById(x[k].id);
		h1.setAttributeNode(attr1);
		var b = x[k].id;
		markerFlightsDict[String(b)].setIcon(myIcon);
	}
});