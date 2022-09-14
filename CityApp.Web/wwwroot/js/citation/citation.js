
var citation =
    {
        IsRejected: false,
        //ajax call change citation status
        ChangeStatus: function (reason) {
            var CitationId = $('#ddlStatus').attr('citationId');
            var StatusId = $('#ddlStatus').val();
            blockUI();
            $.ajax({
                type: 'POST',
                url: objCitation.CitationUrl,
                dataType: 'json',
                data: {
                    CitationId: CitationId,
                    StatusId: StatusId,
                    Reason: reason
                },
                success: function (data) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("success", "Status Updated.");
                    }
                    var SelectedStatus = $('#ddlStatus option:selected').text();
                    citation.HideStatus(SelectedStatus);
                    window.location.reload();
                },
                cache: true
            });
        },

        //ajax call for unassign user
        UnassignUser: function () {
            var citationId = $('#ddlAssignedTo').attr('citationId');

            $.ajax({
                type: 'POST',
                url: objCitation.UnassignUrl,
                dataType: 'json',
                data: {
                    citationId: citationId
                },
                success: function (data) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("success", "User has been Unassigned.");
                    }
                    $('#ddlAssignedTo').val("0");
                },
                cache: true
            });

        },

        ///get url of video/image by attachment id
        GetUrlByAttachment: function (attachmentKey, attachmentType) {

            var url = objCitation.s3BucketUrl + attachmentKey;
            if (attachmentType == "video") {
                html = '<video width="100%" height="300" controls><source src="' + url + '" type="video/mp4"></video>';
            }
            else if (attachmentType == "image") {
                html = '<center><img class=\"img-responsive\" src="' + url + '"/></center>';
            }
            swal({
                html: html,
                confirmButtonColor: '#3085d6',
                confirmButtonClass: 'btn btn-success',
                buttonsStyling: false
            }).then(function (result) {

            });

        },

        //Hide Status 
        HideStatus: function (SelectedStatus) {
            if (SelectedStatus == "Approved") {
                $('#ddlStatus option[value="1"]').remove();
            }
            else if (SelectedStatus == "In Review") {
                $('#ddlStatus option[value="1"]').remove();
                $('#ddlStatus option[value="2"]').remove();
            }
            if (SelectedStatus == "Closed" || SelectedStatus == "Rejected") {
                document.getElementById('divStatus').innerHTML = '<label class="">' + SelectedStatus + '</label>';
            }

        },

        //Delete Attachment
        AttachmentDelete: function (AttachmentId) {
            blockUI();
            $.ajax({
                type: 'POST',
                url: objCitation.DeleteAttachment,
                dataType: 'json',
                data: {
                    attachmentId: AttachmentId
                },
                success: function (data) {
                    if (data.success == true) {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("success", data.message);
                            location.reload();
                        }
                    }
                    else {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("danger", data.message);
                        }

                    }
                    unblockUI();

                },
                cache: true
            });
        },

        //Save Comment
        SaveComment: function (CitationId, Comment) {
            $.ajax({
                type: 'POST',
                url: objCitation.SaveCommentUrl,
                dataType: 'json',
                async: false,
                data: {
                    citationId: CitationId,
                    commentId: $("#hdnCommentId").val(),
                    comment: Comment,
                    isPublic: $("#chkIsPublic").is(':checked')
                },
                success: function (data) {
                    if (citation.IsRejected == false) {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("success", "Comment saved successfully");
                        }
                        $("#btnAddComment").text("ADD COMMENT");
                        $("#btnCancelComment").hide();
                        $("#txtComment").val("");
                        $("#hdnCommentId").val("");
                        $("#chkIsPublic").prop("checked", false);
                    }
                    ReloadCommentsGrid(CitationId);
                },
                cache: true
            });

        },

        //Reject Reason
        Reasons: function () {
            swal({
                title: 'Reject Reason',
                html: '<div class="col-sm-10">' +
                '<div class="row">' +
                '<div class="form-group" > ' +
                '<select id="ddlReason" class="form-control">' +
                '<option value="">Select Reason</option>' +
                '<option value="Sorry, we couldn' + "'" + 't clearly see the license plate in your video and therefore could not approve it. Next time be sure to zoom in and clearly capture it.">License Plate</option>' +
                '<option value="Sorry, we couldn' + "'" + 't clearly see the driver in the video using a mobile device and therefore could not approve it.">Mobile Device</option>' +                
                '<option value="Sorry, We couldn' + "'" + 't approve your video as it appears you were driving while filming.Please note that this is illegal and a violation of our terms of use. Any additional videos you submit while driving will be grounds for the termination of your account.">User driving</option>' +
                '<option value="">Other</option>' +
                '</select>' +
                '<span id="spnReasonValidation" class="text-danger"></span>' +
                '</div>' +
                '</div>' +
                '<div class="row">' +
                ' <div class="form-group label-floating">' +
                ' <label  class="control-label"></label>' +
                '<textarea id="txtReason" style="height:60px !important" class="form-control"></textarea>' +
                '</div>' +
                '</div>' +
                '</div>',
                showCancelButton: true,
                confirmButtonClass: 'btn btn-success',
                cancelButtonClass: 'btn btn-danger',
                buttonsStyling: false,
                closeOnConfirm: false,
                preConfirm: function (textarea) {

                    if ($("#ddlReason").val() != "") {
                        return new Promise(function (resolve, reject) {
                            citation.IsRejected = true;
                            var CitationId = $('#ddlStatus').attr('citationId');
                            $("#hdnCommentId").val("");
                            blockUI();
                            citation.SaveComment(CitationId, $("#txtReason").val());
                            citation.ChangeStatus($("#txtReason").val());
                            unblockUI();
                            resolve()
                        })
                    }
                    else {
                        $("#spnReasonValidation").text("This field is required");
                        $(".swal2-confirm").attr("disabled", false);
                        $(".swal2-cancel").attr("disabled", false);
                    }
                },

            }).then(function (result) {

            })

        },


        //*********save video images************
        SaveVideoAttachment: function (time, src, videoId, citationId) {
            blockUI();
            $.ajax({
                type: 'POST',
                url: objCitation.SaveVideoAttachment,
                dataType: 'json',
                async: true,
                data: {
                    Time: time,
                    Src: src,
                    VideoId: videoId,
                    CitationNumber: objCitation.citationNumber,
                    CitationId: citationId
                },
                success: function (data) {
                    if (data.success == true) {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("success", "Image Captured Successfully.");
                        }
                    }
                    location.reload()
                    //ReloadCommentsGrid(CitationId);
                },
                cache: true
            });

        },

        //Message Pop up
        Message: function (Recipient, displayName) {

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
                            citation.SendMessage(recipient, displayName, $("#txtMessage").val());
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
        SendMessage: function (Recipient, DisplayName, Message) {
            $.ajax({
                type: 'POST',
                url: objCitation.SendMessageUrl,
                dataType: 'json',
                async: true,
                data: {
                    message: Message,
                    recipient: Recipient,
                    displayName: DisplayName
                },
                success: function (data) {
                    if (data.success === true) {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("success", "Message Sent");
                        }
                    }
                    else {
                        demo.showNotification("danger", data.message);
                    }


                },
                cache: true
            });

        },


        //*********delete citation************
        DeleteCitation: function (Id) {
            blockUI();
            $.ajax({
                type: 'POST',
                url: objCitation.DeleteCitationUrl,
                dataType: 'json',
                data: {
                    id: Id
                },
                success: function (data) {
                    if (data.success == true) {
                        window.location.href = objCitation.GetCitationUrl;
                    }
                    else {
                        if ($(".alert").length > 0) {
                        }
                        else {
                            demo.showNotification("danger", data.message);
                        }

                    }
                    unblockUI();

                },
                cache: true
            });
        },

    }


$(document).ready(function () {

    var message = $("#hdnUploadMessage").val();

    if (message != "") {

        if (message == "File Uploaded Successfully." || message == "File Deleted Successfully." || message == "Updated") {

            if ($(".alert").length > 0) {
            }
            else {
                demo.showNotification("success", message);
            }
        }
        else {
            if ($(".alert").length > 0) {
            }
            else {
                demo.showNotification("danger", message);
            }
        }

        removeQString("message");
    }

    var SelectedStatus = $('#ddlStatus option:selected').text();
    var SelectedVal = $('#ddlStatus').val();
    citation.HideStatus(SelectedStatus);


    //*********save images as thumbnails************
    $("#btnSaveImage").click(function () {
        var vid = document.getElementById("citationVideo");
        var time = vid.currentTime;
        var src = $("#videoSrc").attr('src');
        var videoId = $(this).attr("VideoAttachmentId");
        var citationId = $(this).attr("citationId");
        citation.SaveVideoAttachment(time, src, videoId, citationId)
    });

    //Change the status of citation
    $('#ddlStatus').change(function () {
        var CurrentStatus = $('#ddlStatus option:selected').text();
        swal({
            title: 'Are you sure?',
            text: "You are changing the status from " + SelectedStatus + " to " + CurrentStatus,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(
            function () {
                if (CurrentStatus != "Rejected") {
                    citation.ChangeStatus("");
                }
                else {
                    citation.Reasons();
                }
            },
            function (dismiss) {
                if (dismiss === 'cancel') {
                    $('#ddlStatus').val(SelectedVal);
                }
            })
    });

    //change assignTo citation
    $('#ddlAssignedTo').change(function () {
        var CitationId = $(this).attr('citationId');
        var AssignToId = $(this).val();
        if (AssignToId != "0") {
            blockUI();
            $.ajax({
                type: 'POST',
                url: objCitation.AssignUrl,
                dataType: 'json',
                data: {
                    citationId: CitationId,
                    assignToId: AssignToId

                },
                success: function (data) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        var assignee = $("#ddlAssignedTo option:selected").text();
                        demo.showNotification("success", "Assignment complete. An email has been sent to " + assignee);
                    }
                    unblockUI();
                },
                cache: true
            });
        }
    });

    //click on cancel the comment saving
    $('#btnCancelComment').click(function () {
        $("#btnAddComment").text("ADD COMMENT");
        $("#btnCancelComment").hide();
        $("#txtComment").val("");
        $("#hdnCommentId").val("");
        $("#chkIsPublic").prop("checked", false);
    });

    //add new comments
    $('#btnAddComment').click(function () {

        if ($("#txtComment").val().trim() != "") {
            var CitationId = $(this).attr('citationId');
            var Comment = $("#txtComment").val();
            citation.SaveComment(CitationId, Comment);

        }
        else {
            demo.showNotification("danger", "Enter Comment");
        }
    });

    //edit an existing comment by comment id
    $(document).on('click', '.editComment', function () {
        var CitationId = $(this).attr('citationId');
        var CommentID = $(this).attr('CommentID');
        $.ajax({
            type: 'POST',
            url: objCitation.EditCommentUrl,
            dataType: 'json',
            data: {
                commentId: CommentID
            },
            success: function (data) {
                if (data.comment != '') {
                    $("#txtComment").val(data.comment);
                    $("#chkIsPublic").prop("checked", data.isPublic);
                    $("#hdnCommentId").val(data.commentID);
                    $("#btnAddComment").text("UPDATE");
                    $("#btnCancelComment").show();
                    $("#txtComment").focus();
                }
                else {
                    demo.showNotification("danger", "You can't edit this comment");
                }
            },
            cache: true
        });
    });

    //delete an existing comment
    $(document).on('click', '.DeleteComment', function () {
        var CitationId = $(this).attr('citationId');
        var CommentID = $(this).attr('CommentID');
        $.ajax({
            type: 'POST',
            url: objCitation.DeleteCommentUrl,
            dataType: 'json',
            data: {
                commentId: CommentID
            },
            success: function (data) {
                if (data == '1') {
                    demo.showNotification("success", "Deleted successfully");
                    ReloadCommentsGrid(CitationId);
                }
                else {
                    demo.showNotification("danger", "You can't delete this comment");
                }
            },
            cache: true
        });
    })

    //unassign user from citation
    $("#lnkUnassign").click(function () {
        if ($('#ddlAssignedTo').val() != "0") {
            citation.UnassignUser();
        }
        else {
            if ($(".alert").length > 0) {
            }
            else {
                demo.showNotification("warning", "No User Assigned.");
            }
        }

    });

    //add comment on enter click
    $('body').keypress(function (e) {

        if (e.which == 13) {
            $('#btnAddComment').click();
            return false;
        }
    });

    $(".lnkDisplayAttachment").click(function () {
        var attachmentKey = $(this).parent().find("input[type='hidden']").val();
        var attachmentType = $(this).attr('attachmenttype');
        citation.GetUrlByAttachment(attachmentKey, attachmentType);
    });

    $('#GetCitationZip').click(function () {
        blockUI();
        var CitationId = $(this).attr('citationId');
        var CitationNumber = $(this).attr('citationNumber');
        $.ajax({
            type: 'POST',
            url: objCitation.SendEvidencePackage,
            dataType: 'json',
            data: {
                citationId: CitationId,
                citationNumber: CitationNumber
            },
            success: function (data) {
                if (data) {
                    demo.showNotification("success", "Package Created.");
                    window.reload(); 
                }
                else {
                    demo.showNotification("danger", "No media available");
                }
                unblockUI();

            },
            cache: true
        });
    });

    //Delete Attachment
    $(document).on('click', '.clsDeleteAttachment', function () {
        var attachmentType = $(this).attr('attachmentId');
        swal({
            title: 'Are you sure?',
            text: "You are about to permanently delete.",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'YES',
            cancelButtonText: 'NO',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(
            function () {
                citation.AttachmentDelete(attachmentType)
            },
            function (dismiss) {
                if (dismiss === 'cancel') {

                }
            })
    });

    $(document).on('change', '#ddlReason', function () {
        if ($(this).val() != "") {
            $("#txtReason").val($(this).val());
            $("#spnReasonValidation").text('');
        }
        else {
            $("#txtReason").val('');
        }
    })

    //Delete Citation
    $(document).on('click', '#btnDeleteCitation', function () {
        swal({
            title: 'Are you sure?',
            text: "This will permanently delete the citation",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(
            function () {
                var Id = $("#btnDeleteCitation").attr("citationId");
                citation.DeleteCitation(Id);
            },
            function (dismiss) {
                if (dismiss === 'cancel') {


                }
            })
    })


    $(document).on('click', '#sndMessage', function () {

        var recipient = $(this).attr('email');
        var displayName = $(this).attr('displayName');
        citation.Message(recipient, displayName)
    });


    demo.initFormExtendedDatetimepickers();


});

window.initMap = function initMap() {
    var myLatLng = {
        lat: objCitation.varlat, lng: objCitation.varlng
    };

    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 18,
        center: myLatLng,
        mapTypeId: google.maps.MapTypeId.SATELLITE

    });

    var marker = new google.maps.Marker({
        position: myLatLng,
        map: map
    });
}

function ReloadCommentsGrid(CitationID) {
    $.ajax({
        type: 'POST',
        url: objCitation.GetCommentsUrl,
        dataType: 'json',
        data: {
            citationId: CitationID
        },
        success: function (data) {
            var html = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    html = html + '<div class="panel panel-default"><div class="panel-heading" role="tab" id="headingOne"><a role="button" data-parent="#accordion" href="#" aria-expanded="false" aria-controls="collapseTwo">'
                    html = html + '<h4 class="panel-title">' + data[i].createdBy + ' added a comment - ' + data[i].createdHumanizerDate + '<i class="material-icons" style="float:left">keyboard_arrow_down</i>'
                    html = html + '</h4></a></div><div id="' + data[i].commentID + '" aria-labelledby="headingOne">'
                    html = html + '<div class="panel-body">' + data[i].comment
                    if (data[i].enableEdit) {
                        html = html + '<br/><a href="javascript:void(0)" CitationID="' + CitationID + '" CommentID="' + data[i].commentID + '" class="blue-link editComment">Edit</a>'
                        html = html + ' <a href="javascript:void(0)" CitationID="' + CitationID + '" CommentID="' + data[i].commentID + '" class="blue-link DeleteComment">Delete</a>'
                    }
                    html = html + '</div></div></div>'
                }
            }
            $("#accordion").html(html);
        },
        cache: true
    });
}

function removeQString(key) {
    var urlValue = document.location.href;
    //Get query string value
    var searchUrl = location.search;

    if (key != "") {
        oldValue = getParameterByName(key);
        removeVal = key + "=" + oldValue;
        urlValue = urlValue.replace(searchUrl, '');
    }
    else {
        var searchUrl = location.search;
        urlValue = urlValue.replace(searchUrl, '');
    }
    history.pushState({ state: 1, rand: Math.random() }, '', urlValue);
}


function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}