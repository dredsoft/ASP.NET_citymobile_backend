var violation = {
    orderlist: [],
    AddQuestion: function (event) {
        if ($("#Question").val().trim() !== "") {
            var ViolationId = $(event).attr('violationid');
            var Question = $("#Question").val();
            var choices = $("#txtChoiceQuestion").val()
            var Type = $("#drpQuestionType").val()
            if (Type != "TextField") {
                if (choices.trim() !== "") {
                    violation.SaveQuestion(ViolationId, Question, Type, choices);
                }
                else {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("danger", "Enter Choice");
                    }
                }
            }
            else {
                violation.SaveQuestion(ViolationId, Question, Type, choices);
            }

        }
        else {
            if ($(".alert").length > 0) {
            }
            else {
                demo.showNotification("danger", "Enter Question");
            }
        }
    },

    //*************************Save Violation Question**************************************
    SaveQuestion: function (ViolationId, Question, Type, Choices) {
        $.ajax({
            type: 'POST',
            url: objViolation.SaveQuestionUrl,
            dataType: 'json',
            async: false,
            data: {
                violationId: ViolationId,
                questionId: $("#hdnQuestionId").val(),
                question: Question,
                type: Type,
                choices: Choices,
                isPublic: $("#chkIsPublic").is(':checked')
            },
            success: function (data) {
                if (data.success === true) {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("success", "Question saved successfully");
                    }
                    $("#btnAddQuestion").text("Add A QUESTION");
                    $("#btnCancelQuestion").hide();
                    $("#Question").val("");
                    $("#hdnQuestionId").val("");
                    $("#chkIsPublic").prop("checked", false);
                    $('#txtChoiceQuestion').tagsinput('removeAll');
                    $("#drpQuestionType").val("TextField");
                    $("#divChoice").hide();
                    ReloadQuestionsGrid(ViolationId)
                }
                else {
                    if ($(".alert").length > 0) {
                    }
                    else {
                        demo.showNotification("danger", data.message);
                    }

                }


            },
            cache: true
        });

    },

    //************************Save Question Order Function**********************************
    SaveOrder: function () {
        $.ajax({
            type: 'POST',
            url: objViolation.SaveOrderUrl,
            dataType: 'json',
            async: false,
            data: {
                Orderlist: violation.orderlist

            },
            success: function (data) {
                if (data.success === true) {
                    violation.orderlist.length = 0;
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

    //sort question li
    sortable('.js-sortable', {
        forcePlaceholderSize: true,
        placeholderClass: 'p1 mb1 bg-navy border border-yellow',
    });

    //**********************************Initilize sortable function **********************************
    $('.js-sortable').sortable({
        update: function (e, ui) {

            $(".js-sortable li").each(function (i, el) {
                var newIndex = i;
                $(this).attr('order', newIndex);
                var _questionId = $(this).attr('questionID');
                violation.orderlist.push({ questionID: _questionId, order: newIndex });
            });
            violation.SaveOrder();
        }

    });



    $("#hrfQuestion").click(function () {
        $("#btntoggle").hide();
    });

    $(".violationTab").click(function () {
        $("#btntoggle").show();
    });

    //**********************************Edit Comment Event**********************************************
    $(document).on('click', '.editQuestion', function () {
        var ViolationId = $(this).attr('violationId');
        var QuestionID = $(this).attr('questionID');
        $('#txtChoiceQuestion').tagsinput('removeAll');
        $.ajax({
            type: 'POST',
            url: objViolation.EditQuestionUrl,
            dataType: 'json',
            data: {
                questionID: QuestionID
            },
            success: function (data) {
                if (data.comment !== '') {
                    debugger;
                    $("#Question").val(data.question);
                    $("#chkIsPublic").prop("checked", data.isRequired);
                    $("#hdnQuestionId").val(data.questionID);
                    $("#btnAddQuestion").text("UPDATE");
                    if (data.type != "TextField") {
                        $("#drpQuestionType").val(data.type);
                        $("#txtChoiceQuestion").tagsinput('add', data.choices);
                        $("#divChoice").show();
                    }
                    $("#btnCancelQuestion").show();

                }
                else {
                    demo.showNotification("danger", "You can't edit this comment");
                }
            },
            cache: true
        });
    });

    //**********************************Comment Delete Event***********************************************
    $(document).on('click', '.DeleteQuestion', function () {
        var ViolationId = $(this).attr('violationId');
        var QuestionID = $(this).attr('questionID');
        $.ajax({
            type: 'POST',
            url: objViolation.DeleteQuestionUrl,
            dataType: 'json',
            data: {
                questionID: QuestionID
            },
            success: function (data) {
                if (data === '1') {
                    demo.showNotification("success", "Deleted successfully");
                    ReloadQuestionsGrid(ViolationId);
                }
                else {
                    demo.showNotification("danger", "You can't delete this comment");
                }
            },
            cache: true
        });
    });


    //**************************Cancel Edit Question Event************************************************
    $('#btnCancelQuestion').click(function () {
        $("#btnAddQuestion").text("ADD COMMENT");
        $("#btnCancelQuestion").hide();
        $("#Question").val("");
        $("#hdnQuestionId").val("");
        $("#chkIsPublic").prop("checked", false);
        $('#txtChoiceQuestion').tagsinput('removeAll');
        $("#drpQuestionType").val("TextField");
        $("#divChoice").hide();
    });

    //***********************Question Type Dropdown Change Event ****************************************
    $("#drpQuestionType").change(function () {
        if ($(this).val() == "TextField") {
            $("#divChoice").hide();
            $('#txtChoiceQuestion').tagsinput('removeAll');
        }
        else {
            $("#divChoice").show();

        }
    });

    //***********************Question focusout Event ****************************************
    $("#Question").on("focusout", function () {
        if (($(this).val() != "")) {
            $("#divQuestion").removeClass("is-empty")
        }
    });


});
//*****************************Question Grid Reload function**********************************************
function ReloadQuestionsGrid(ViolationID) {
    $.ajax({
        type: 'POST',
        url: objViolation.GetQuestionsUrl,
        dataType: 'json',
        data: {
            violationId: ViolationID
        },
        success: function (data) {
            var html = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    html = html + '<li class="p1 mb1 navy bg-yellow panel panel-default" style= "position: relative; z-index: 10" questionID= ' + data[i].questionID + ' order= ' + data[i].order + ' >' + data[i].question
                    if (data[i].enableEdit) {
                        html = html + '<br /><a href="javascript:void(0)" violationId="' + ViolationID + '" questionID="' + data[i].questionID + '" class="blue-link editQuestion">Edit</a>'
                        html = html + ' <a href="javascript:void(0)" violationId="' + ViolationID + '" questionID="' + data[i].questionID + '" class="blue-link DeleteQuestion">Delete</a>'
                    }
                    html = html + '</li >'

                }
            }
            $("#accordion").html('');
            $("#accordion").html(html);
        },
        cache: true
    });
}