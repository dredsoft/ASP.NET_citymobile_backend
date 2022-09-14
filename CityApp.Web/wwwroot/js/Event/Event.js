var Event = {


    //ajax call change citation status
    DeleteEvent: function (EventId) {
        $.ajax({
            type: 'POST',
            url: objEvent.DeleteUrl,
            dataType: 'json',
            data: {
                Id: EventId
            },
            success: function (data) {
                window.location = objEvent.EventsUrl

            },
            cache: true
        });
    },

}
$(document).ready(function () {

    //Declare summernote editor
    $('#editor').summernote({
        height: 300,                 // set editor height
        minHeight: null,             // set minimum height of editor
        maxHeight: null,             // set maximum height of editor
        focus: true                  // set focus to editable area after initializing summernote
    });

    //Paste InnerHTML into summernote container
     $('#editor').summernote('code', $("#Body").val());    

    $("#submit").click(function () {
        debugger;
        var code = $('#editor').summernote('code');        
        if (code.trim() != "") {
            $("#Body").val(code);
        }
        else {
            $("#Body").val("");
        }
    });

    demo.initFormExtendedDatetimepickers()

    //Change the status of citation
    $(document).on('click', '#delete', function () {
        var Id = $('#delete').attr("EventId");
        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'YES,DELETE IT!',
            cancelButtonText: 'NO',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(
            function () {
                Event.DeleteEvent(Id)
            },
            function (dismiss) {
                if (dismiss === 'cancel') {

                }
            })
    });

});