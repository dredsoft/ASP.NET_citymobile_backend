var HeatMap = {
    arr: [],

    DrawHeatMap: function () {
        var CreatedFrom = $("#CreatedFrom").val();
        var CreatedTo = $("#CreatedTo").val();
        var StatusId = $("#StatusId").val();
        var AssignedToId = $("#AssignedToId").val();
        var LicensePlate = $("#LicensePlate").val();
        var ViolationTypeId = $("#ViolationTypeId").val();
        var ViolationId = $("#ViolationId").val();

        $.ajax({
            type: 'POST',
            url: objCitation.CitationUrl,
            dataType: 'json',
            data: {
                CreatedFrom: CreatedFrom,
                CreatedTo: CreatedTo,
                StatusId: StatusId,
                AssignedToId: AssignedToId,
                LicensePlate: LicensePlate,
                ViolationTypeId: ViolationTypeId,
                ViolationId: ViolationId
            },
            success: function (data) {
                HeatMap.arr.length = 0;
                for (var i = 0; i <= data.citationsListItem.length - 1; i++) {
                    HeatMap.arr.push(new google.maps.LatLng(data.citationsListItem[i].latitude, data.citationsListItem[i].longitude));
                }
                var centerPoint = '';
                if (data.citationsListItem.length > 0) {
                    for (var i = 0; i <= data.citationsListItem.length - 1; i++) {
                        if (data.citationsListItem[i].latitude != 0) {
                            centerPoint = new google.maps.LatLng(data.citationsListItem[i].latitude, data.citationsListItem[i].longitude);
                            break;
                        }
                        else {
                            centerPoint = new google.maps.LatLng(37.775, -122.434);
                        }
                    }
                }
                else {
                    centerPoint = new google.maps.LatLng(37.775, -122.434);
                }

                initMap(HeatMap.arr, centerPoint);
            },
            cache: true
        });
    }
}

$(document).ready(function () {

    demo.initFormExtendedDatetimepickers();

    HeatMap.DrawHeatMap();

});


function initMap(arr, centerPoint) {
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 13,
        center: centerPoint,
        mapTypeId: 'satellite'
    });

    heatmap = new google.maps.visualization.HeatmapLayer({
        data: arr,
        map: map
    });

}

function toggleHeatmap() {
    heatmap.setMap(heatmap.getMap() ? null : map);
}

function changeGradient() {
    var gradient = [
        'rgba(0, 255, 255, 0)',
        'rgba(0, 255, 255, 1)',
        'rgba(0, 191, 255, 1)',
        'rgba(0, 127, 255, 1)',
        'rgba(0, 63, 255, 1)',
        'rgba(0, 0, 255, 1)',
        'rgba(0, 0, 223, 1)',
        'rgba(0, 0, 191, 1)',
        'rgba(0, 0, 159, 1)',
        'rgba(0, 0, 127, 1)',
        'rgba(63, 0, 91, 1)',
        'rgba(127, 0, 63, 1)',
        'rgba(191, 0, 31, 1)',
        'rgba(255, 0, 0, 1)'
    ]
    heatmap.set('gradient', heatmap.get('gradient') ? null : gradient);
}

function changeRadius() {
    heatmap.set('radius', heatmap.get('radius') ? null : 20);
}

function changeOpacity() {
    heatmap.set('opacity', heatmap.get('opacity') ? null : 0.2);
}