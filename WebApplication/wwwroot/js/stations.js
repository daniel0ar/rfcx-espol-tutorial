$(document).ready(function(){
    stations_input_changed = []
    $.ajax({
        url : 'api/Station/',
        type: 'GET',
        success : getStationsList
    })  

    $("#station_modal input.form-control").change(function () {
        var input_id = $(this).attr("id");
        if(!stations_input_changed.includes(input_id)) {
            switch(input_id) {
                case "latitude":
                    if(stations_input_changed.includes("longitude")) {
                        return;
                    }
                    break;
                case "longitude":
                    if(stations_input_changed.includes("latitude")) {
                        return;
                    }
                    break;
            }
            stations_input_changed.push(input_id);
        }
    });

    $("input").val("");

    $('#station_modal').on('hidden.bs.modal', function (e) {
        $("form input").val("");
        $("h4#modal_label").html("Nueva Estación");
        var inputs = $(".form-group");
        for(i of inputs) {
            $(i).removeClass("has-error"); 
        }
        stations_input_changed.length = 0;
    });

    $('#alert_modal').on('hidden.bs.modal', function (e) {
        $("input#st_id").val("");
    });

});

function getStationsList(data) {
    var data_dic = JSON.parse(data);
    for(station of data_dic){
        var station_id = station['Id'];
        var station_name = station['Name'];
        var content = contentStation(station_id,station_name);

        $(content).insertBefore(".plus-station");
    }
    $.ajax({
        url : 'api/Sensor/',
        type: 'GET',
        success : getSensorsList
    })
}

function contentStation(station_id, station_name){
    var content = `
        <div class="station col-lg-4 col-md-4 col-sm-4 col-xs-12">
         <div class="title row">
         <div class="col-lg-8 col-md-7 col-sm-7 col-xs-7"><h4>${station_name}</h4></div>
         <div class="col-lg-4 col-md-5 col-sm-5 col-xs-5 header">
          <a class="icon_station" href="/StationView?stationName=${station_name}&stationId=${station_id}">
          <i class="material-icons fa fa-line-chart" id="barra"></i></a>
          <i class="material-icons icon_station" style="font-size:23px; color:#2874A6;" onclick="fillStationModal('${station_id}');">edit</i>
          <i class="material-icons icon_station" style="font-size:23px; color:#CB4335;" onclick="showAlertModal('${station_id}');">delete</i></div>
        </div><div class="station_body" id="station${station_id}"></div></div>`;
    var content_=`
    <div class="col-lg-4 col-md-6 col-sm-12 col-xs-12">
        <div class="card" style ="margin-bottom:20px;">
            <div class="card-header">
                <div class="row">		
                <div class="col-md-2 col-lg-2 col-sm-2 col-xs-2"><div class="pulse-animation"></div></div>
                <div class="col-md-6 col-lg-5 col-sm-6 col-xs-6"><h5 class="card-title">${station_name}</h5></div>
                <div class="col-md-4 col-lg-5 col-sm-4 col-xs-4">
                <a href="/StationView?stationName=${station_name}&stationId=${station_id}"><i class="fa fa-line-chart icon_station"></i></a>
                <i class='fa fa-pencil icon_station' onclick="fillStationModal('${station_id}');"></i>
                <i class='fa fa-trash icon_station' onclick="showAlertModal('${station_id}');"></i>
                </div>
                </div>			
            </div>
            <div class="card-body" id="station${station_id}">			   		   
            </div>
            <div class = "last_record">
                <p><small class="text-muted">Last record 3 mins ago</small></p>
            </div>
        </div>
	</div>
    `;
    return content_;
}
function getSensorsList(data) {
    var data_dic = JSON.parse(data); 
    for(sensor of data_dic){
        var sensor_id = sensor['Id'];
        var station_id = sensor['StationId'];
        var sensor_type = sensor['Type'];
        var sensor_location = sensor['Location'];
        var icon_type = getIconType(sensor_type);
        var icon_id = getIconId(sensor_type, sensor_location);
        var station_body = $("div.card-body#station"+station_id);
        var content = contentSensor(icon_id,icon_type, sensor_type, sensor_location, sensor_id);
        $(station_body).append(content);
    }
    getLastData();
}

function contentSensor(icon_id,icon_type, sensor_type, sensor_location, sensor_id){
    var content = `
    <div class="row">
        <div class="col-lg-1 col-md-1 col-sm-1 col-xs-1 body">
            <i id="${icon_id}" class="fa ${icon_type}"></i>
        </div>
        <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8 body text">
            <p>${sensor_type}  ${sensor_location}</p>
        </div>
        <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3 body">
            <p id="sensor${sensor_id}"></p>
        </div>
    </div>
    `;
    return content;
}
function getLastData() {
    $.ajax({
        url : 'api/Data/LastData',
        type: 'GET',
        success : function(full_data){
            var data_dic = JSON.parse(full_data); 
            for(data of data_dic){
                var station_id = data['StationId'];
                var sensor_id = data['SensorId'];
                var value = parseFloat(data['Value']).toFixed(2);
                var unit = getUnit(data['Type']);
                var s = $("div.card-body#station"+station_id+" p#sensor"+sensor_id);
                s.html(value + " " + unit);
            }
        }
    })
    updateStationsHeight();
}

setInterval(getLastData, 300000);

function getUnit(sensor_type) {
    var type = sensor_type.toLowerCase();
    if(type.includes("hum")) {
        return "%";
    } else if(type.includes("temp")) {
        return "°C";
    } else {
        return "?";
    }
}

function getIconType(sensor_type) {
    if(sensor_type.toLowerCase().includes("temp")) {
        return "fa-thermometer";
    } else if(sensor_type.toLowerCase().includes("hum")) {
        return "fa-tint";
    }
}

function getIconId(sensor_type, sensor_location) {
    if(sensor_type.toLowerCase().includes("hum")) {
        return "hum";
    } else if((sensor_type.toLowerCase().includes("temp") && sensor_location.toLowerCase().includes("env")) ||
                (sensor_type.toLowerCase().includes("temp") && sensor_location.toLowerCase().includes("amb"))) {
        return "temp_env";
    } else if((sensor_type.toLowerCase().includes("temp") && sensor_location.toLowerCase().includes("sta")) ||
                (sensor_type.toLowerCase().includes("temp") && sensor_location.toLowerCase().includes("esta"))) {
        return "temp_station";
    }
}

function saveStation() {
    if(validateForm()) {
        var id = $("input#db_id").val();
        if(id == "") {
            saveNewStation();
        } else {
            updateStation(id);
        }
        window.location.reload();
    }
}

function validateForm() {
    var result = true;
    var inputs = $(".form-group");
    for(i of inputs) {
        $(i).removeClass("has-error"); 
    }
    var name = $("#form #name");
    var game_station = $("#form #game_station");
    var api_key = $("#form #api_key");
    var latitude = $("#form #latitude");
    var longitude = $("#form #longitude");
    if(name.val() == "") {
        name.parent(".form-group").addClass("has-error");
        result = false;
    } 
    if(api_key.val() == "") {
        api_key.parent(".form-group").addClass("has-error");
        result = false;
    } 
    if(game_station.val() <= 0) {
        game_station.parent(".form-group").addClass("has-error");
        result = false;   
    } 
    if(!($.isNumeric(latitude.val()) && (latitude.val() >= -90) && (latitude.val() <= 90))) {
        latitude.parent(".form-group").addClass("has-error");
        result = false;   
    } 
    if(!($.isNumeric(longitude.val()) && (longitude.val() >= -180) && (longitude.val() <= 180))) {
        longitude.parent(".form-group").addClass("has-error");
        result = false;   
    }
    return result;
}

function saveNewStation() {
    var name = $("input#name").val();
    var game_station = $("input#game_station").val();
    var lat = $("input#latitude").val();
    var long = $("input#longitude").val();
    var and_ver = $("input#android_version").val();
    var ser_ver = $("input#services_version").val();
    var api_k = $("input#api_key").val();
    var data = JSON.stringify({ "Name": name, "GameStation": game_station, "Latitude": lat, 
    "Longitude": long, "AndroidVersion": and_ver, "ServicesVersion": ser_ver, "APIKey": api_k});
    $.ajax({
        type: 'POST',
        url: 'api/Station',
        dataType: 'json',
        async: false,
        data: data,
        contentType: 'application/json'
    });
}

function updateStation(id) {
    for(st of stations_input_changed) {
        var obj = {};
        var api_url = getApiUrl(st);
        var db_name = getDbName(st);
        var value = $("input#" + st).val();
        obj[db_name] = value;
        if(db_name == "Latitude") {
            var long = $("input#longitude").val();
            var data = JSON.stringify({ "Latitude": value, "Longitude":long });
        } else if(db_name == "Longitude") {
            var lat = $("input#latitude").val();
            var data = JSON.stringify({ "Latitude": lat, "Longitude":value });
        } else {
            var data = JSON.stringify(obj);
        }
        $.ajax({
            type: 'PATCH',
            url: 'api/Station/' + id + '/' + api_url,
            dataType: 'json',
            async: false,
            data: data,
            contentType: 'application/json'
        });
    }
}

function getApiUrl(st) {
    switch(st) {
        case "api_key":
            return "APIKey";
        case "name":
            return "Name";
        case "game_station":
            return "GameStation";
        case "latitude":
            return "Coordinates";
        case "longitude":
            return "Coordinates";
        case "android_version":
            return "AndroidV";
        case "services_version":
            return "ServicesV";
    }
}

function getDbName(st) {
    switch(st) {
        case "api_key":
            return "APIKey";
        case "name":
            return "Name";
        case "game_station":
            return "GameStation";
        case "latitude":
            return "Latitude";
        case "longitude":
            return "Longitude";
        case "android_version":
            return "AndroidVersion";
        case "services_version":
            return "ServicesVersion";
    }
}

function showAlertModal(id) {
    $("input#st_id").val(id);
    $("#alert_modal").modal("show");
}

function deleteStation() {
    var id = $("input#st_id").val();
    $.ajax({
        url : 'api/Station/'+id,
        type: 'DELETE',
        async: false
    });
    window.location.reload();
}

function fillStationModal(id){
    $.ajax({
        url : 'api/Station/'+id,
        type: 'GET',
        async: false,
        success : function(data){
            var data_dic = JSON.parse(data);
            $("input#name").val(data_dic["Name"]);
            $("input#game_station").val(parseInt(data_dic["GameStation"]));
            $("input#api_key").val(data_dic["APIKey"]);
            $("input#latitude").val(data_dic["Latitude"]);
            $("input#longitude").val(data_dic["Longitude"]);
            $("input#android_version").val(data_dic["AndroidVersion"]);
            $("input#services_version").val(data_dic["ServicesVersion"]);
            $("input#db_id").val(data_dic["Id"]);
            $("h4#modal_label").html("Editar Estación");
            $("#station_modal").modal("show");
        }
    })
}

function updateStationsHeight(){
    var body_maximum_height = 0;
    var bodies = $(".station_body").get();
    var title_height = $(".title").get()[0];
    var d = $(title_height).height();
    for(b of bodies) {
        if($(b).height() > body_maximum_height) {
            body_maximum_height = $(b).height();
        }
    }
    for(b of bodies) {
        if((body_maximum_height + d + 13) < 85) {
            $(b).height(72 - d);
        } else {
            $(b).height(body_maximum_height);
        }
    }
    if((body_maximum_height + d + 13) < 85) {
        $(".new_station_button").height(85);
    } else {
        $(".new_station_button").height(body_maximum_height + d + 13);
    }    
}

function closeModal(id){
    if(id == 1) {
        $("#station_modal").modal("hide");
    } else {
        $("#alert_modal").modal("hide");
    }
}