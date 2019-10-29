function showAlertModal(id) {
    $("input#st_id").val(id);
    $("#alert_modal").modal("show");
}

function closeModal() {
    $("#alert_modal").modal("hide");
}

function deleteStation() {
    var audio_id = $("input#st_id").val();
    var station_id = $("#StationId").find(":selected").val();
    $.ajax({
        url: 'api/Station/' + station_id + '/Audio/' + audio_id,
        type: 'DELETE',
        async: false
    });
    window.location.reload();
}

function downloadImagenes() {
    console.log("actualizado images");
    var station_id = $("#StationId").find(":selected").val();
    console.log(station_id);

    var image_id = $("#imagen").find(":selected").val();
    console.log(image_id);

    var images = $('.check:checkbox:checked').map(function (){
        var currentRow = $(this).closest('tr');
        return currentRow.find("td:eq(2)").text();
    }).get().join(',');

    console.log(images);
    var image = images+".jpg";
    console.log(image);
    if (images == "") {
        alert("Por favor, Seleccione las imagenes desea descargar.");
    } else {
        window.location = 'imgcapture/download?namefile=' + images + '&station=' + station_id;
    }
}



function newTag(t, id) {
    
    var tag = prompt("Ingrese una nueva etiqueta prueba:");
    if (tag !== null && tag !== "") {
        $.ajax({
            type: 'PUT',
            url: 'AddTag',
            data: {
                "ImageId": id,
                "Tag": tag
            },
            error: function(xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
            },
            success: function() {
                alert("Se ha agregado una nueva etiqueta");
            }
        });
        var tr = $(t).closest('tr');
        var td = tr.find("td:eq(5)");
        if ($(td).find('.no-tags').length) {
            $(td).children().remove();
        }
        td.append('<span>' + tag + '</span>');
        $(td).append(" ");
    }
}