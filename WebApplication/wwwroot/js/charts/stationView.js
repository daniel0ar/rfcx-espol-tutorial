//get Id of Station
var stationId = $("#stationId").text();

var CONFIG = {
    stationId : stationId,
    hoursAgo : 400, //From how many hours ago, I want retrieve first batch of data
    timeInterval : 5000, //At which rate I want to request for new single data. Milliseconds
    maxDataPointsAllowed : 15, //How many points I want to keep in the chart when adding new points
}

//build url to retrieve initial Data
let query =  timeStampQuery({
    momentJsObject : moment(),
    hoursAgo : CONFIG.hoursAgo
})  

let initialDataUrl = `api/Station/${CONFIG.stationId}/${query}`;
let sensorsUrl = `api/Station/${CONFIG.stationId}/Sensor`; 

let initialDataPromise = $.getJSON(initialDataUrl);
let sensorsPromise = $.getJSON(sensorsUrl);

$.when( sensorsPromise , initialDataPromise)
.done(function(sensorsResponse, initialDataResponse) {
    let sensors = sensorsResponse[0];
    sensors.forEach(function(sensor){
        //filter data of sensor
        let sensorData = initialDataResponse[0].filter( data => data.SensorId == sensor.Id );

        //process response
        let rawDataPoints = sensorData.map(toRawDataPoint).sort(byTimestamp);
        let dpsNullPointsAdded = addNullPoints(rawDataPoints, (CONFIG.timeInterval/1000));//Refactor with reduce
        let dataPoints = dpsNullPointsAdded.map(toDataPoint);
        
        //add basic statistics
        let rawDataPointsValues = rawDataPoints.map( element => parseFloat(element.Value) );
        let basicStatistics = computeBasicStatistics(rawDataPointsValues);       
        let chartDivId = `chart_${sensor.Id}`;
        let chartDiv = makeChartDiv(
            chartDivId,
            basicStatistics
        );
        $("div.chartsContainer").append(chartDiv);

        //create chart
        let chart =  realTimeChart(chartDivId, sensor.Type, "");
        let units = getUnits(sensorData);
        let toolTipContent = formatUnitsToTheToolTip(units);

        //update the chart
        chart.options.data[0].dataPoints = dataPoints;
        chart.toolTip.set("content", toolTipContent);

        //render changes
        chart.render();

        //build url to retrieve last Data  
        let lastDataUrl = `api/Station/${CONFIG.stationId}/Sensor/${sensor.Id}/Data/LastData`;

        //set a job to update charts for certain time period, this makes the chart realtime
        setInterval(
            updateChart, 
            CONFIG.timeInterval, 
            chart, 
            lastDataUrl, 
            CONFIG.maxDataPointsAllowed,
            chartDivId
        );
    });    
});

/*** Functions for charts logic ***/

//create a  CanvasJs chart object, given a divId
function realTimeChart(divId, axisYTitle, chartTitle){
    return new CanvasJS.Chart(divId, {
        height: 320,
        theme: "light2",
        title:{
            text : chartTitle
        },
        axisX:{
            valueFormatString: "hh:mm:ss TT" ,
            labelAngle: -90
        },
        axisY:{
            title : axisYTitle,
            titleFontSize: 18
        },
        data:[{
            xValueType: "dateTime",
            type : "line",
            markerSize: 5,
            dataPoints: []
        }]
    });
}

//build a timeStampQuery, check https://momentjs.com/docs
function timeStampQuery( { momentJsObject, hoursAgo } ){
    let now = momentJsObject.clone();//I make a clone to avoid modify the original object
    let endTimestamp  = now.unix();//unix() function gives the timestamp
    let startTimestamp = now.subtract(hoursAgo,'hours').unix();
    return `Timestamp?startTimestamp=${startTimestamp}&endTimestamp=${endTimestamp}`;
}

function toDataPoint(responseElement){
    let timestamp = responseElement.Timestamp;
    let value = responseElement.Value;

    //format x value
    let x = new Date(parseInt(timestamp)*1000);

    //format y value
    let y = (value == null) ? null : Number(parseFloat(value).toFixed(2));
    
    return {
        "x" : x,
        "y" : y
    };
}

function toRawDataPoint(responseElement){
    return {
        Timestamp : responseElement.Timestamp,                
        Value : responseElement.Value
    };
}

//compare function to be used in sort method
function byTimestamp(a,b){
    return a.Timestamp - b.Timestamp;
}

//place datapoints if between two datapoints there was supposed to be a value.
function addNullPoints(dataPoints, timeInterval){
    if (dataPoints.length > 0 ){
        let dataPointsNullPointsAdded = [];
        for(let i = 0 ; i < (dataPoints.length -1) ; i++){
        dataPointsNullPointsAdded.push(dataPoints[i]);
    
        let nextTimestamp = dataPoints[i+1].Timestamp;
        let currentTimestamp = dataPoints[i].Timestamp;
    
        if ( nextTimestamp > (currentTimestamp + timeInterval)) {
            let nullDataPoint = makeNullDataPoint(nextTimestamp, currentTimestamp);  
            dataPointsNullPointsAdded.push(nullDataPoint);
        }        
        }
        dataPointsNullPointsAdded.push(dataPoints[dataPoints.length - 1]);
        return dataPointsNullPointsAdded;
    }else {
        return [];
    }
}

function makeNullDataPoint(nextTimestamp, currentTimestamp){
    let middleTimestamp = Math.floor((nextTimestamp + currentTimestamp)/2);
    let nullDataPoint = {
        Timestamp: middleTimestamp,
        Value: null
    };
    return nullDataPoint;
}

function updateChart( 
    canvasJsChart, 
    lastDataURL, 
    maxDataPointsAllowed,
    chartDivId,
) {    
    $.getJSON(lastDataURL, function(data) {

        //either was previous data or not
        //process response
        let currentDataPoints = canvasJsChart.options.data[0].dataPoints;        
        let newDataPoint = toRawDataPoint(data);

        if (currentDataPoints.length > 0 ){
            let dataPointsLength = currentDataPoints.length;
            let lastDataPoint = currentDataPoints[dataPointsLength - 1];
                    
            let nextTimestamp = newDataPoint.Timestamp;
            let currentTimestamp = lastDataPoint.x.getTime()/1000;
            let timeInterval  = CONFIG.timeInterval/1000;
            if ( nextTimestamp > (currentTimestamp + timeInterval)){         
                let nullDataPoint = makeNullDataPoint(nextTimestamp, currentTimestamp);  
                currentDataPoints.push(toDataPoint(nullDataPoint));
            }
        }
                
        //update chart
        currentDataPoints.push(toDataPoint(newDataPoint));

        //avoids accumulate points in the chart
        if(currentDataPoints.length > maxDataPointsAllowed ){
            currentDataPoints.shift();
        }

        //render changes
        canvasJsChart.render();

        //update basic statistics
        let rawDataPointsValues = currentDataPoints.filter( element => element.y != null ).map( element => parseFloat(element.y) );
        let basicStatistics = computeBasicStatistics(rawDataPointsValues);        
        addDataToBasicStatisticsContainer(
            chartDivId,
            basicStatistics
        );
    });
}

function formatUnitsToTheToolTip(units){
    if ( units == "Celcius") {
        return "{y} °C" ; 
    } else if ( units == "Percent") {
        return "{y} %" ;
    } else {
        return "";
    }
}

function getUnits(data){
    if ( data[0] != null ){
        //take units from first element of response and assume all elements have same units    
        return data[0].Units;
    } else {
        return "";
    }
}

function makeChartDiv(
    chartDivId,
    basicStatistics
){    
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
        <div class="card-body">
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

function addDataToBasicStatisticsContainer(chartDivId, basicStatistics){
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

function formatFloat(value){
    return Number(parseFloat(value).toFixed(2));
}
