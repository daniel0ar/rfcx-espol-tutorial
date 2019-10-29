var stationId = parseInt($("#stationId").text());
var stationNameSensor=$("#stationName").text();
var dataPoints = [];
var listDataSensor = [];
var dataSensor = {};
var listValuesMinMaxAvg = [];

//get location and type of actual sensor
function getDataSensor(idSensor){
    $.getJSON('api/Station/'+stationId+'/Sensor/'+idSensor, function(data){
        dataSensor['location'] = data['Location'];
        dataSensor['type'] = data['Type'];
        dataSensor['id'] = data['Id'];
        if(data['Type'].toLowerCase().startsWith("hum")){
            dataSensor['unit'] = "%";
        }else{
            dataSensor['unit'] = "CELCIUS";
        }
    });
}
//get the dates from inputs and make the query
function getDates(id){
    var idButton = id.substring(7,id.length);
    var nameStart = "start"+idButton;
    var nameFinish = "finish"+idButton;
    var nameSelect = "selectBox"+idButton+"2";
    var start = moment($("input[name="+nameStart+"]").val());  
    var finish = moment($("input[name="+nameFinish+"]").val()); 
    var selectFilter = $("#"+nameSelect)[0];
    var selectValue = selectFilter.options[selectFilter.selectedIndex].value;
    filterByRange(start, finish, selectValue);

}
function getLegends(){
    var location = dataSensor["location"];
    var type = dataSensor["type"];
    var titleVertical = "Temperatura °C";
    var minValId = "minValue"+type+"_"+location;
    var maxValId = "maxValue"+type+"_"+location;
    var avgValId = "avgValue"+type+"_"+location;
    var chartId = "chart_"+type+"_"+location;
    if(type.toLowerCase().startsWith("hum") && location.toLowerCase().startsWith("env")){
        var colorP = "#424084";
        var titleVertical = "Humedad %";
    }
    else if(location.toLowerCase().startsWith("temp") && location.toLowerCase().startsWith("env")){
        var colorP = "orange";
    }
    else if(location.toLowerCase().startsWith("temp") && location.toLowerCase().startsWith("sta")){
        var colorP = "LightSeaGreen";
    }
    return [colorP, titleVertical, minValId, maxValId, avgValId, chartId]
}
function changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV){
    $("#"+minValId).text(minV);
    $("#"+maxValId).text(maxV);
    $("#"+avgValId).text(avgV);
}


function displayChartInd(divId, titleVertical, colorL, data, format, contentTool){
    var chartMon = new CanvasJS.Chart(divId, {
        animationEnabled: true,
        zoomEnabled: true,
        height: 320,
        theme: "light2",
        toolTip:{   
			content: contentTool   
		},
        axisX:{      
            valueFormatString: format
        },
        axisY: {
            title: titleVertical,
            titleFontSize: 18
        },
        data: [{
            type: "line",
            lineColor: colorL,
            dataPoints: data
        }]
    });


    chartMon.render();
    listDataSensor = dataPoints.slice();
    dataPoints = [];
    
}

function addDataHours(data){
    data = JSON.parse(data);
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    if(data!=null && data.length!=0){
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
        for(points of data){
            var time = parseInt(points['Timestamp']);
            var value = parseFloat(points['Value']);
            sumV = sumV + value;
            if(value<minV){
                minV = value;
            }if(value>maxV){
                maxV = value;
            }
            var date = new Date(time*1000);
            var hours = date.getHours()+":"+(date.getMinutes()<10?'0':'') + date.getMinutes();
            dataPoints.push({
                x: date,
                y: value,
                hour: hours,
                color: colorP
            });
        }
        var lengthChart = dataPoints.length;
        var avgV = (sumV/lengthChart).toFixed(2);
        if (isNaN(avgV)){
            avgV = 0;
        }
        listValuesMinMaxAvg = [maxV, minV, avgV.replace(".",",")];
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
        
    }else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    console.log(dataPoints);
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "DDD/D HH:mm", "<strong> {hour}</strong>: {y}");

}

function addDataDays(data){
    data = JSON.parse(data);
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    if(data!=null && data.length!=0){
        //Boxes min, max, avg
        var minV = 5000; var maxV = 0; var sumV = 0;
        for(points of data){
            var time = parseInt(points['Timestamp']);
            var value = parseFloat(points['Value']);
            sumV = sumV + value;
            if(value<minV){
                minV = value;
            }if(value>maxV){
                maxV = value;
            }
            var date = new Date(time*1000);
            dataPoints.push({
                x: date,
                y: value,
                color: colorP
            });
        }
        var lengthChart = dataPoints.length;
        var avgV = (sumV/lengthChart).toFixed(2);
        if (isNaN(avgV)){
            avgV = 0;
        }
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
        
    }else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "DD-DDD", "<strong> {x} </strong>:{y}");

}

function addDataOneDay(data, status){
    var colorP = getLegends()[0], titleVertical = getLegends()[1];
    var minValId = getLegends()[2], maxValId= getLegends()[3];
    var avgValId= getLegends()[4], chartId= getLegends()[5];
    console.log(getLegends);
    data = JSON.parse(data);
    if(data!=null && data.length!=0){
            //Boxes min, max, avg
            var minV = 5000; var maxV = 0; var sumV = 0;
            for(points of data){
                var time = parseInt(points['Timestamp']);
                var value = parseFloat(points['Value']);
                sumV = sumV + value;
                if(value<minV){
                    minV = value;
                }if(value>maxV){
                    maxV = value;
                }
                var date = new Date(time*1000);
                var hours = date.getHours()+":"+(date.getMinutes()<10?'0':'') + date.getMinutes();
                dataPoints.push({
                    x: date,
                    y: value,
                    hour: hours,
                    color: colorP
                });
            }
            var lengthChart = dataPoints.length;
            var avgV = (sumV/lengthChart).toFixed(2);
            if (isNaN(avgV)){
                avgV = 0;
            }
            changeValuesMinMaxAvg(minValId, maxValId, avgValId, minV, maxV, avgV);
    }
    else{
        changeValuesMinMaxAvg(minValId, maxValId, avgValId, 0, 0, 0);
        
    }
    displayChartInd(chartId, titleVertical, colorP, dataPoints, "hh:mm TT", "<strong> {hour}</strong> {y}");

}
//Filter by date ranges
function filterByRange(start, finish, selectedValue){
    var idSensor = parseInt(dataSensor['id']);
    if(selectedValue=="hora"){
        var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+start.unix()+"&EndTimestamp="+finish.unix()+"&Filter=Hours&FilterValue=1";
        $.get(query, addDataHours);
    }
    else if(selectedValue=="12horas"){
        //filterByTwelveHours(start, finish);
        var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+start.unix()+"&EndTimestamp="+finish.unix()+"&Filter=Hours&FilterValue=12";
        $.get(query, addDataHours);
    }
    else if(selectedValue=="dia"){
        //filterByDay(start, finish);
        var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+start.unix()+"&EndTimestamp="+finish.unix()+"&Filter=Days&FilterValue=1";
        $.get(query, addDataDays);
    }
    else if(selectedValue=="semana"){
        //filterByWeek(start, finish);
        var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+start.unix()+"&EndTimestamp="+finish.unix()+"&Filter=Weeks&FilterValue=1";
        $.get(query, addDataDays);
    }else{
        //filterByMonth(start, finish);
        var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+start.unix()+"&EndTimestamp="+finish.unix()+"&Filter=Months&FilterValue=1";
        $.get(query, addDataDays);
    }
}

function filterByWeek(dateStart=0, dateFinish = new Date()){
    var idSensor = parseInt(dataSensor['id']);
    var finish = moment(dateFinish);
    var finishTimestamp = finish.unix();
    if(dateStart == 0){
        var startTimestamp = finish.clone().subtract(1,'week').unix();
    }else{
        var startTimestamp = dateStart.unix();
    }
    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimestamp+"&EndTimestamp="+finishTimestamp+"&Filter=Hours&FilterValue=6";
    $.get(query, addDataDays);
}
function filterByHour(dateStart=0, dateFinish = new Date()){
    var idSensor = parseInt(dataSensor['id']);
    var finish = moment(dateFinish);
    var finishTimestamp = finish.unix();
    if(dateStart == 0){
        var startTimestamp = finish.clone().subtract(1,'hour').unix();
    }else{
        var startTimestamp = dateStart.unix();
    }

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp?StartTimestamp="+startTimestamp+"&EndTimestamp="+finishTimestamp;
    $.get(query, addDataOneDay);
}

function filterByTwelveHours(dateStart=0, dateFinish = new Date()){
    var idSensor = parseInt(dataSensor['id']);
    var finish = moment(dateFinish);
    var finishTimestamp = finish.unix();
    if(dateStart == 0){
        var startTimestamp = finish.clone().subtract(12,'hours').unix();
    }else{
        var startTimestamp = dateStart.unix();
    }

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimestamp+"&EndTimestamp="+finishTimestamp+"&Filter=Hours&FilterValue=1";
    $.get(query, addDataOneDay);
}

function filterByDay(dateStart=0, dateFinish = new Date()){
    var idSensor = parseInt(dataSensor['id']);
    var finish = moment(dateFinish);
    var finishTimestamp = finish.unix();
    if(dateStart == 0){
        var startTimestamp = finish.clone().subtract(1,'day').unix();
    }else{
        var startTimestamp = dateStart.unix();
    }

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimestamp+"&EndTimestamp="+finishTimestamp+"&Filter=Hours&FilterValue=1";
    $.get(query, addDataOneDay);
}
function filterByMonth(dateStart=0, dateFinish = new Date()){
    var idSensor = parseInt(dataSensor['id']);
    var finish = moment(dateFinish);
    var finishTimestamp = finish.unix();
    if(dateStart == 0){
        var startTimestamp = finish.clone().subtract(1,'month').unix();
    }else{
        var startTimestamp = dateStart.unix();
    }

    var query = "api/Station/"+stationId+"/Sensor/"+idSensor+"/DataTimestamp/Filter?StartTimestamp="+startTimestamp+"&EndTimestamp="+finishTimestamp+"&Filter=Days&FilterValue=1";
    $.get(query, addDataDays);
}

function changeFunc(id) {
    var selectBox = document.getElementById(id);
    var selectedValue = selectBox.options[selectBox.selectedIndex].value;
    if(id.endsWith("2")){
        var nameButton = "filter_"+id.substring(9,id.length-1);
        var filterButton =  document.getElementById(nameButton); 
        filterButton.disabled = false; 
    }else{
        if(selectedValue=="hora"){
            filterByHour();
        }
        else if(selectedValue=="12horas"){
            filterByTwelveHours();
        }
        else if(selectedValue=="dia"){
            filterByDay();
        }
        else if(selectedValue=="semana"){
            filterByWeek();
        }else{
            filterByMonth();
        }
    }
    
    
}


//EXPORT FUNCTION

//GET DATA FROM ACTUAL SENSOR
//Example: name=export_humedad_ambiente
function getDataSensorExport(name){
    listToExport = [];
    var estacion = stationNameSensor;
    var sensor = name.substring(7,name.length).replace("_", " ");
    console.log(listDataSensor);
    if(listValuesMinMaxAvg.length==0 || listDataSensor.length==0){
        listValuesMinMaxAvg = [0, 0, 0];
    }

    console.log(listDataSensor, dataPoints);

    listToExport = [
        ["ESTATION", estacion],
        ["SENSOR", sensor],
        ["MAX", listValuesMinMaxAvg[0]],
        ["MIN", listValuesMinMaxAvg[1]],
        ["AVG", listValuesMinMaxAvg[2]],
        ["","","",""]     
    ];
    if (listDataSensor.length!=0){
        if("hour" in listDataSensor[0]){
            listToExport.push(["DATE", "HOUR", "VALUE", "UNIT"]);
        }else{
            listToExport.push(["DATE", "VALUE", "UNIT"]);
        }
        valuesList = []
        for(var i = 0; i<listDataSensor.length; i++){
            //x is the date in Date format
            var dateToExport = listDataSensor[i]["x"];
            valuesList.push(dateToExport.getDate()+"/"+(dateToExport.getMonth()+1)+"/"+dateToExport.getFullYear());
            if("hour" in listDataSensor[0]){
                valuesList.push(listDataSensor[i]["hour"]);
            }
            //y is the value
            valuesList.push(listDataSensor[i]["y"]);
            valuesList.push(dataSensor['unit']);
            listToExport.push(valuesList);
            valuesList=[];
        }
    
    }
    return listToExport;

}
//EXPORT ARRAY IN CSV 
//Example: name export_humedad_ambiente
function downloadCSV(name){
    dataSensorExport = getDataSensorExport(name);

    let csvContent = "data:text/csv;charset=utf-8,";
    dataSensorExport.forEach(function(rowArray){
       let row = rowArray.join(";");
       csvContent += row + "\r\n";
    }); 
    var encodedUri = encodeURI(csvContent);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "datos_"+name.substring(7,name.length)+".csv");
    document.body.appendChild(link); //Required

    link.click();
}