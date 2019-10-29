setInterval(displayLastData, 300000);

function displayLastData(){
	$.get("api/Data/LastData", function(full_data){
		var data_dic = JSON.parse(full_data); 
		for(data of data_dic){	
			var station_id = data['StationId'];
			var sensor_id = data['SensorId'];
			var value = parseFloat(data['Value']).toFixed(2);
			var p = document.getElementById("Sta"+station_id+"Sens"+sensor_id);
			if(p != null){
				var tipo = data['Type'];
				var unit = "?";
				if(tipo.toUpperCase().includes("TEMP")){
					unit = "°C";
				}else if (tipo.toUpperCase().includes("HUM")){
					unit = "%";
				}
				p.innerHTML = value +" "+ unit;
			}
		}
	});
}
function getDataSensor(){
  for(var stationId in stations){
      $.get('api/Station/'+stationId+'/Sensor/', function(data){
				var dataDic = JSON.parse(data);
					for(sensor of dataDic){
						var idStation = sensor['StationId'];
						var id = sensor['Id'];
						var typeSensor = sensor['Type'];
						var locationSensor = sensor['Location'];
						var contentS = stations[idStation]["content"];
						
						if(typeSensor.toUpperCase().includes("HUM")){
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-tint" style="color: #527cfb" ></i> Ambiente: </p><p class="valueHum" id="humedadId"> <i class="fa fa-circle-o-notch fa-spin"></i> </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("humedadId", "Sta"+idStation+"Sens"+id);
						}
						else if(typeSensor.toUpperCase().includes("TEMP") && (locationSensor.toUpperCase().includes("AMB") || locationSensor.toUpperCase().includes("ENV"))){
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="color: #424084;"></i> Ambiente: </p><p class="valueTempAmb" id="tempAmbId"> <i class="fa fa-circle-o-notch fa-spin"></i> </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("tempAmbId", "Sta"+idStation+"Sens"+id);
						}else{
							stations[idStation]["content"] = contentS + '<p class="sensor-title"><i class="fa fa-thermometer" style="color: #ff7800;"></i> Estación: </p><p class="valueTempDisp" id="tempDispId"> <i class="fa fa-circle-o-notch fa-spin"></i> </p>';
							stations[idStation]["content"] = stations[idStation]["content"].replace("tempDispId", "Sta"+idStation+"Sens"+id);
						}
						stations[idStation]["content"] = stations[idStation]["content"] +'</div>'+
																		'</div>';
						
						stations[idStation]["sensorsId"].push(id);
					}
        initMap();
      });
    }  
      
  }

function initMap() {
	var centerCoordinatesBosque = {lat: -2.15437, lng: -79.963035};
	var estilos =[
		{
			featureType: "poi",
			elementType: "labels",
			stylers: [
					{ visibility: "off" }
			]
		},
		{
			featureType: "transit",
			elementType: "labels",
			stylers: [
					{ visibility: "off" }
			]
		}
	];
	var map = new google.maps.Map(document.getElementById('map'), {
		zoom: 17,
		center: centerCoordinatesBosque,
		styles: estilos		
	});
	map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
	for(var stationId in stations){
		var coordenadas = {lat: parseFloat(stations[stationId]["lat"]), lng: parseFloat(stations[stationId]["long"])};
		var contentString = stations[stationId]["content"];
		var infowindow = new google.maps.InfoWindow({
			content: contentString
		});
		var imageURL = 'http://maps.google.com/mapfiles/kml/paddle/orange-circle.png';		
		var image = {
			url: imageURL,
			scaledSize: new google.maps.Size(30, 30)
		};
		var marker = new google.maps.Marker({
			position: coordenadas,
			map: map,
			icon: image
		});
		//infowindow.open(map,marker);
		var currentInfoWindow = null;
		google.maps.event.addListener(marker,'click', (function(marker,contentString,infowindow){ 
			return function() {
				if (currentInfoWindow != null) { 
					currentInfoWindow.close(); 
				}
				infowindow.open(map, marker); 
				currentInfoWindow = infowindow;
				displayLastData();
			};
		})(marker,contentString,infowindow));	
	}             
}