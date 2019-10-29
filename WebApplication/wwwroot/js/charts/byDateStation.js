$(function(){

//set current date to datepickers
let date_pickers = Array.from(document.getElementsByClassName("date-picker"));
date_pickers.forEach(function(date_picker){
    let now = moment();    
    document.querySelector("input.date-picker-end").value = now.format('YYYY-MM-DD');
    date_picker.value = now.subtract(29,'days').format('YYYY-MM-DD');    
});

let filterButton = document.querySelector("button#button-filter");

filterButton.addEventListener("click", function(){ 
    let selectedSensorText = document.querySelector("select.form-control").value;
    let sensorType = selectedSensorText.trim().split(" ")[0];
    let sensorLocation = selectedSensorText.trim().split(" ")[1];

    let startDateMoment =  moment(document.querySelector("input.date-picker").value);    
    let endDateMoment =  moment(document.querySelector("input.date-picker-end").value);
    let startTimestamp = startDateMoment.unix();
    let endTimestamp = endDateMoment.unix();

    let validDateRange = isValidDateRange(
        startTimestamp, 
        endTimestamp,
        moment().unix()     
    );
    let dataUrl = `api/AvgPerDateStation?SensorType=${sensorType}&SensorLocation=${sensorLocation}&StartTimestamp=${startTimestamp}&EndTimestamp=${endTimestamp}`;
    let stationsUrl = ` api/Station/`;

    let dataPromise = $.getJSON(dataUrl);
    let stationsPromise = $.getJSON(stationsUrl);

    if ( !validDateRange ) {
        alert("Los valores en el rango a filtrar son inválidos. Es probable que esté tratando de filtrar valores que aún no existen.");
    } else {
        $.when( stationsPromise , dataPromise)
        .done(function(stationsResponse, dataResponse) {
            let stations = stationsResponse[0];

            if( dataResponse[0].length < 1 ) {
                alert("No existen valores en el rango especificado");
            }
            
            //This chart is unique, and basic statistics involve data from all stations           
            let chartDivId = `chartAvgPerDateBetweenStations`;
            let chartDiv = makeChartDiv({
                chartDivId: chartDivId,
                basicStatistics : null
            });
            
            if ( $(`div#_${chartDivId}.card`).length ) {
                $(`div#_${chartDivId}.card`).remove(); 
            }
            $("div.chartsContainer").append(chartDiv);            
        
        
            //create chart                        
            let axisYTitle = `${sensorType} ${sensorLocation}`;
            let chart = aggregationChart({
                chartDivId : chartDivId,                    
                chartTitle : "",
                axisYTitle : axisYTitle,
                axisXFormat: "DD MMM YY"
            });
            
            let allValuesForBasicStatistics = [];//gather all values to compute basic statistics
            stations.forEach(function(station){
                //filter data of station
                let stationData = dataResponse[0].find( data => data.StationId == station.Id );

                //if no value, .find() returns undefined. So, check for safety
                let aggregates =  stationData != null ? stationData.aggregates : [];
                                
                let rawDataPoints = aggregates.map(function(aggregate){
                    let year = aggregate._id.year; 
                    let month =  aggregate._id.month ;
                    let day =  aggregate._id.dayOfMonth;
                    let avg = aggregate.avg;
                    
                    let dateObject = new Date(year, (month - 1), day);
                    let timestamp = dateObject.getTime()/1000;//gives timestamp in seconds
                    //console.log(timestamp);
                    return {
                        Timestamp : timestamp,                
                        Value : avg
                    }; 
                }).sort(byTimestamp);

                let dpsNullPointsAdded = addNullPoints_(
                    rawDataPoints, 
                    startTimestamp,
                    endTimestamp,
                    86400
                );

                let dataPoints = dpsNullPointsAdded.map(function(responseElement){
                    let timestamp = responseElement.Timestamp;
                    let value = responseElement.Value;

                    //format x value
                    let x = new Date(parseInt(timestamp)*1000);

                    //format y value
                    let y = (value == null) ? null : formatFloat(value);
                    
                    return {
                        "x" : x,
                        "y" : y
                    };
                });
                
                let rawDataPointsValues = rawDataPoints.map( element => element.Value );
                allValuesForBasicStatistics = allValuesForBasicStatistics.concat(rawDataPointsValues);
                        
                let toolTipContent = `{y} ${unitsFromSensorType(sensorType)}`;
                let nameOfData = `${station.Name}`;
                chart.options.data.push({         
                    legendMarkerType: "circle",
                    toolTipContent: toolTipContent,
                    showInLegend: true,
                    name : nameOfData,
                    xValueType: "dateTime",
                    type : "line",                                        
                    markerSize: 5,
                    dataPoints: dataPoints
                });
                //render changes
                chart.render();                                   
            });    

            //after we gathered all data, we run basic statistics
            let basicStatistics = computeBasicStatistics(allValuesForBasicStatistics);
            addDataToBasicStatisticsContainer({
                chartDivId: chartDivId, 
                basicStatistics: basicStatistics
            })
            
        });
    }  
   
});
filterButton.click();


function aggregationChart({
    chartDivId,    
    chartTitle,
    axisYTitle,
    axisXFormat
}){
    return new CanvasJS.Chart(chartDivId, {
        animationEnabled:true,
        height: 320,
        theme: "light2",
        title:{
            text : chartTitle
        },
        legend: {
            horizontalAlign: "right", // "center" , "right"
            verticalAlign: "top",  // "top" , "bottom"
            cursor: "pointer",
            itemclick: function (e) {
                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }
                e.chart.render();
            }
        },
        axisX:{
            valueFormatString: axisXFormat,
            labelAngle: -90
        },
        axisY:{
            title : axisYTitle,
            titleFontSize: 18
        },
        data:[]
    });
}

//place datapoints if between two datapoints there was supposed to be a value.
//different from the used in realtime chart
function addNullPoints_(
    dataPoints,
    startTimestamp,
    endTimestamp,
    timeInterval
){
    
    if (dataPoints.length > 0 ){
        let timestampRange = endTimestamp - startTimestamp;
        let timestampsLength = Math.floor(timestampRange/timeInterval);
        let timestamps  = Array.from({length: timestampsLength}, (_, i) => (startTimestamp + i*timeInterval) );
        let dataPointsNullPointsAdded = timestamps.map(function(timestamp){
            dpFound = dataPoints.find(dp => dp.Timestamp == timestamp);
            dataPoint = (dpFound == null) ? { Timestamp: timestamp, Value: null } : dpFound ;
            return dataPoint;
        });
        return dataPointsNullPointsAdded;
    }else {
        return [];
    }
};

function computeBasicStatistics(values){

    if (values.length > 0){
        return {
            min : ss.min(values),
            max : ss.max(values),
            mean : ss.mean(values)
        }
    } else {
        return null;
    }
}

function isValidDateRange(
    startTimestamp,
    endTimestamp, 
    nowTimestamp
){
    return  !(startTimestamp > nowTimestamp || endTimestamp > nowTimestamp);
}

function makeChartDiv({
    chartDivId,
    basicStatistics
}){    
    let min, max, mean;
    if( basicStatistics != null ) {
        min  = formatFloat(basicStatistics.min);
        max  = formatFloat(basicStatistics.max);
        mean = formatFloat(basicStatistics.mean);
    }else {
        min = max = mean = "";
    }
    
    let chartDiv = `
    <div class="card mb-4" id="_${chartDivId}">
        <div class="card-body" >
            <div id="${chartDivId}" style="height: 320px" class="canvasJsChart"></div>
        </div>        
        <div class="card-footer text-center">
            <i class="material-icons iconBasicStatitic">&#xe15d;</i>
            min                
            <p class="basicStatisticValue" id="minVal">${min}</p>
            
            <i class="material-icons iconBasicStatitic">&#xe148;</i>
            max
            <p class="basicStatisticValue" id="maxVal">${max}</p>

            <i class="fa iconBasicStatitic">&#xf10c;</i>
            avg
            <p class="basicStatisticValue" id="avgVal">${mean}</p>                
        </div>
        
    </div>                      
    `;
    return chartDiv;
};


function addDataToBasicStatisticsContainer({
    chartDivId, 
    basicStatistics
}){
    let min, max, mean;
    if( basicStatistics != null ) {
        min  = formatFloat(basicStatistics.min);
        max  = formatFloat(basicStatistics.max);
        mean = formatFloat(basicStatistics.mean);
    } else {
        min = max = mean = "";
    }
        
    
    $(`div.card#_${chartDivId} .card-footer p#minVal`).text(min);
    $(`div.card#_${chartDivId} .card-footer p#maxVal`).text(max);
    $(`div.card#_${chartDivId} .card-footer p#avgVal`).text(mean); 
}

function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
};

//compare function to be used in sort method
function byTimestamp(a,b){
    return a.Timestamp - b.Timestamp;
}

function unitsFromSensorType(sensorType){
    let units;
    switch(sensorType){
        case "Temperature":
            units = "C°";
            break;
        case "Humidity":
            units = "%";
            break;
        default :
            units = "";
            break;            
    }
    return units;
}

});