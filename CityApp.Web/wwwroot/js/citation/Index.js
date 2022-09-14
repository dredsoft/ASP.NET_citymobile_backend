var citationIndex = {

    CitationPrint: function () {
        $.ajax({
            type: 'POST',
            url: objCitationIndex.PrintCitationUrl,
            dataType: 'json',
            async: false,
            data: {
                createdFrom: $("#CreatedFrom").val(),
                createdTo: $("#CreatedTo").val(),
                statusId: $("#StatusId").val(),
                assignedToId: $("#ddlAssignedTo").val(),
                licensePlate: $("#txtLicensePlate").val(),
                violationId: $("#ViolationId").val(),
                postalCode: $("#PostalCode").val(),
                street: $("#Street").val(),
            },
            success: function (data) {
                var innerhtml = "";
                innerhtml += "<div id='masterContent' class='material-datatables responsivetable'>";
                innerhtml += "<table id='datatables' class='table table-striped table-no-bordered table-hover' cellspacing='0' style='width:100%'>";
                innerhtml += "<thead> <tr> <th>Violation</th>";
                innerhtml += "<th>Violation Code </th>";
                innerhtml += "<th>Ticket Number </th>";
                innerhtml += "<th>Status </th>";
                innerhtml += "<th>Location </th>";               
                innerhtml += "<th>Assigned To</th>";
                innerhtml += "<th>Created </th></tr></thead>";
                if (data.data != null) {
                    for (var i = 0; i < data.data.citationCsvItem.length; i++) {
                        innerhtml += "<tbody> <tr>";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].violationName + "</td >";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].violationCode + "</td>";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].citationNumber + "</td>";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].status + "</td>";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].street + ", " + data.data.citationCsvItem[i].postalCode + "</td>";                       
                        innerhtml += "<td>" + data.data.citationCsvItem[i].assignedTo + "</td>";
                        innerhtml += "<td>" + data.data.citationCsvItem[i].created + "</td>";
                        innerhtml += "</tr ></tbody >";
                    }
                }
                else {
                    innerhtml += "<tbody> <tr></tr ></tbody >";
                }

                innerhtml += "</table></div>";
                $("#divContent").html(innerhtml);

               
            },
            cache: true
        });

    },

}

$(document).ready(function () {
    $("#PrintPreview").click(function () {
        citationIndex.CitationPrint();
    });   


    $("#PrintPreview").printPreview({
        obj2print: '#masterContent',
        width: '810'
    });

});