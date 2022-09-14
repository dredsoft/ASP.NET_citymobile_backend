var User = {


    SendPush: function () {

        $.ajax({
            type: 'POST',
            url: objAccountUser.SendPushUrl,
            dataType: 'json',
            async: true,            
            success: function (data) {
                if (data.success === true) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("success", "Push Send Successfully");
                    }
                }
                else {
                    demo.showNotification("danger", data.message);
                }


            },
            cache: true
        });

    },

    Message: function (Recipient) {

        swal({
            title: 'Send Message',
            html: '<div class="col-sm-10">' +
            '<div class="row">' +
            ' <div class="form-group label-floating">' +
            ' <label  class="control-label"></label>' +
            '<textarea id="txtMessage" placeholder="Message" style="height:60px !important" class="form-control"></textarea>' +
            '<span id="spnMessage" class="text-danger"></span>' +
            '</div>' +
            '</div>' +
            '</div>',
            showCancelButton: true,
            confirmButtonText: 'Send',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false,
            closeOnConfirm: false,
            preConfirm: function (textarea) {

                if ($("#txtMessage").val() != "") {
                    return new Promise(function (resolve, reject) {
                        var recipient = Recipient;
                        blockUI();
                        resolve()
                        User.SendMessage(recipient, $("#txtMessage").val());                       
                        unblockUI();
                       
                    })
                }
                else {
                    $("#spnMessage").text("This field is required");
                    $(".swal2-confirm").attr("disabled", false);
                    $(".swal2-cancel").attr("disabled", false);
                }
            },

        }).then(function (result) {

        })

    },

    //Send Message
    SendMessage: function (Recipient, Message) {
        $.ajax({
            type: 'POST',
            url: objAccountUser.SendMessageUrl,
            dataType: 'json',
            async: true,
            data: {
                message: Message,
                recipient: Recipient,
            },
            success: function (data) {
                if (data.success === true) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("success", "Message Send Successfully");
                    }
                }
                else {
                    demo.showNotification("danger", data.message);
                }


            },
            cache: true
        });

    },
}
$(document).ready(function () {


    //send message to receipt
    $(document).on('click', '#sndMessage', function () {

        var recipient =  $(this).attr('email');
        User.Message(recipient)
    });

    $(document).on('click', '#sndPush', function () {
        User.SendPush();
        
    });
});
