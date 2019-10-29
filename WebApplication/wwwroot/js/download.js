
//Seleccionar audios por medio de checklist para descaragar
$(document).ready(function(){
    //console.log("actualizado");
    $("#selectAll").change(function(){
        var cbs = $("input.check");
        if($("#selectAll").is(":checked")){
            cbs.each(function(){
            $(this).prop("checked", true);
        });
        } else {
            cbs.each(function(){
            $(this).prop("checked", false);
        });
        }
    });

    $(".dl").click(function(){
        var cbs = $("input.check");
        console.log("dentro de function");
        //console.log(cbs);
        var lista_check = "";
        cbs.each(function(){
        if($(this).is(":checked")){
            var station_id = $("#StationId").find(":selected").val();
            //console.log(station_id);
            //console.log(typeof(station_id));
            lista_check = lista_check + $(this).val() + ",";
            //console.log(typeof($(this).val()));
            
            console.log("lista check",lista_check);

        }
        });
        lista_check = lista_check.substring(0, lista_check.length - 1);
        //console.log(lista_check);
        $("#lista").attr("value", lista_check);
        if(lista_check.length != 0){
        $("#form2").submit();        
        } else {
        alert("No ha seleccionado ningún archivo");
        }
    }); 
});