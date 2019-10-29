function showAlertModal(id) {
    $("input#st_id").val(id);
    $("#alert_modal").modal("show");
}

function closeModal() {
    $("#alert_modal").modal("hide");
}

function deleteAudio() {
    var audio_id = $("input#st_id").val();
    var station_id = $("#StationId").find(":selected").val();
    $.ajax({
        url: 'api/Station/' + station_id + '/Audio/' + audio_id,
        type: 'DELETE',
        async: false
    });
    window.location.reload();
}

function downloadAudios() {
    var station_id = $("#StationId").find(":selected").val();
    var audios = $('.check:checkbox:checked').map(function (){
        var currentRow = $(this).closest('tr');
        return currentRow.find("td:eq(1)").text();
    }).get().join(',');
    if (audios == "") {
        alert("Por favor, Seleccione los audios que desea descargar.");
    } else {
        window.location = 'DownloadFile?namefile=' + audios + '&station=' + station_id;
    }
}

function newTag(t, id) {
    var tag = prompt("Ingrese una nueva etiqueta:");
    if (tag !== null && tag !== "") {
        $.ajax({
            type: 'PUT',
            url: 'AddTag',
            data: {
                "AudioId": id,
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
        var td = tr.find("td:eq(4)");
        console.WriteLine(td);
        if ($(td).find('.no-tags').length) {
            $(td).children().remove();
        }
        td.append('<span>' + tag + '</span>');
        $(td).append(" ");
    }
}