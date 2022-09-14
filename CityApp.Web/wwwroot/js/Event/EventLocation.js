var eventLocation, drawingManager, shapeOverlay, coords=[];
$(document).ready(function () {


    eventLocation = new Vue({
        el: '#event-location',
        components: {

        },
        mounted: function () {
        },
        data: {
            isReadyToSave: false,
            coordinates: []
        },
        computed: {

        },
        methods: {
            draw: function () {
                var isSafeToDraw = true;

                if (coords.length) {
                    if (confirm("By continuing, you will delete your existing boundary.  Is that okay?")) {
                        shapeOverlay.setMap(null);
                    }
                    else {
                        isSafeToDraw = false;
                    }
                }

                if (isSafeToDraw) {
                    var self = this;
                    drawingManager = new google.maps.drawing.DrawingManager({
                        drawingMode: google.maps.drawing.OverlayType.POLYGON,
                        drawingControl: true,
                        drawingControlOptions: {
                            position: google.maps.ControlPosition.TOP_CENTER,
                            drawingModes: ['polygon']
                        },
                        polygonOptions: {
                            strokeColor: '#FFF',
                            strokeOpacity: 0.8,
                            strokeWeight: 2,
                            fillColor: '#00bcd4',
                            fillOpacity: 0.35
                        }
                    });
                    drawingManager.setMap(map);
                    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
                        shapeOverlay = polygon;
                        //var polygon = args[0];
                        var path = polygon.getPath();
                        coords = [];
                        for (var i = 0; i < path.length; i++) {
                            coords.push({
                                order: i,
                                latitude: path.getAt(i).lat(),
                                longitude: path.getAt(i).lng()
                            });
                        }

                        self.isReadyToSave = true;
                        drawingManager.setDrawingMode(null);
                    });
                }

            },
            clear: function () {
                if (confirm("You are about to delete your event boundary.  Are you sure?")) {
                    shapeOverlay.setMap(null);
                    coords = [];
                    //Get Coordinates
                    ajaxService("POST", objEvent.DeleteCoords, null, function (data) {
                        demo.showNotification("success", "Boundary Deleted");
                    });
                }
            },

            initMap: function () {
                map = new google.maps.Map(document.getElementById('event-map'), {
                    zoom: 16,
                    center: new google.maps.LatLng(objEvent.lat, objEvent.long),
                    mapTypeId: 'hybrid'
                });

                //Get Coordinates
                ajaxService("GET", objEvent.GetCoords, null, function (data) {
                    var coordinates = data.data;
                    if (coordinates.length) {
                        // Define the LatLng coordinates for the polygon's path.
                        coords = [];
                        $.each(coordinates, function (index, value) {
                            coords.push({ lat: value.latitude, lng: value.longitude });
                        });

                        // Construct the polygon.
                        shapeOverlay = new google.maps.Polygon({
                            paths: coords,
                            strokeColor: '#FFF',
                            strokeOpacity: 0.8,
                            strokeWeight: 2,
                            fillColor: '#00bcd4',
                            fillOpacity: 0.35
                        });

                        shapeOverlay.setMap(map);
                    }

                });

            },

            submit: function () {
                var eventBoundaryCoordinates = [];
                $.each(coords, function (index,coord) {
                    eventBoundaryCoordinates.push({ EventId: objEvent.EventId, Order:coord.order, Latitude: coord.latitude, Longitude: coord.longitude })
                });

                blockUI();
                //POST to server
                ajaxService('POST', objEvent.AddCoords, JSON.stringify(eventBoundaryCoordinates),
                    function (data, result, xhr) {
                        demo.showNotification("success", "Boundary Created!");

                        self.isReadyToSave = false;
                        unblockUI();
                    },
                    function (xhr, status, error) { });
            }
        }
    });

});