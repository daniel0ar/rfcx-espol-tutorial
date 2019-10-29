window.onload = function() {
    
    var StationName =  $("#stationName").text();
    console.log(`Enviando datos simulados a ${StationName}...`);

    // initial value
    var yValueTemp = 25; 
    var yValueHum = 40;    
    var StationId = $("#stationId").text();

    var randomValues = function () {
        var deltaY1, deltaY2;
        var now = moment();
        var xVal = now.unix();
        deltaY1 = .5 + Math.random() *(-.5-.5);
        deltaY2 = .5 + Math.random() *(-.5-.5);
        
        // adding random value and rounding it to two digits. 
        yValueTemp = Math.round((yValueTemp + deltaY1)*100)/100;
        yValueHum = Math.round((yValueHum + deltaY2)*100)/100;

        var data = 
            {
                "data":
                [
                    {
                        "StationId": StationId,
                        "SensorId": 1,
                        "Timestamp": xVal,
                        "Type": "Temperature",
                        "Value": yValueTemp,
                        "Units": "Celcius",
                        "Location": "Environment"
                    },
                    {
                        "StationId": StationId,
                        "SensorId": 2,
                        "Timestamp": xVal,
                        "Type": "Humidity",
                        "Value": yValueHum ,
                        "Units": "Percent",
                        "Location": "Environment"
                    }
                ]
            };

        $.ajax({
            url: 'api/Data',
            type: 'POST',
            data: JSON.stringify(data),
            dataType: 'json',
            contentType: "application/json",
            success: function() {                
                console.log(`Envío de datos simulados a ${StationName} exitoso.`);
            }
        });
    };

    setInterval(randomValues, 3000);
}