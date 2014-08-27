var map;
var marker;
var mapDestinations;
var tmpMarker;
var infowindow;
var pacinput;
var placeService;
var psContainer;
var nearbyPlaces;
var placeTypes = ["accounting", "airport", "amusement_park", "aquarium", "art_gallery", "atm", "bakery", "bank", "bar", "beauty_salon", "bicycle_store", "book_store", "bowling_alley", "bus_station", "cafe", "campground", "car_dealer", "car_rental", "car_repair", "car_wash", "casino", "cemetery", "church", "city_hall", "clothing_store", "convenience_store", "courthouse", "dentist", "department_store", "doctor", "electrician", "electronics_store", "embassy", "establishment", "finance", "fire_station", "florist", "food", "funeral_home", "furniture_store", "gas_station", "general_contractor", "grocery_or_supermarket", "gym", "hair_care", "hardware_store", "health", "hindu_temple", "home_goods_store", "hospital", "insurance_agency", "jewelry_store", "laundry", "lawyer", "library", "liquor_store", "local_government_office", "locksmith", "lodging", "meal_delivery", "meal_takeaway", "mosque", "movie_rental", "movie_theater", "moving_company", "museum", "night_club", "painter", "park", "parking", "pet_store", "pharmacy", "physiotherapist", "place_of_worship", "plumber", "police", "post_office", "real_estate_agency", "restaurant", "roofing_contractor", "rv_park", "school", "shoe_store", "shopping_mall", "spa", "stadium", "storage", "store", "subway_station", "synagogue", "taxi_stand", "train_station", "travel_agency", "university", "veterinary_care", "zoo"];
var frmDestination = document.forms['frmDestination'];
var txtDestinationName = frmDestination.elements["Name"];
var hdDestinationPlaceId = frmDestination.elements["PlaceID"];
var hdDestinationLatitude = frmDestination.elements["Latitude"];
var hdDestinationLongitude = frmDestination.elements["Longitude"];
var hdDestinationReference = frmDestination.elements["Reference"];
var hdDestinationPlaceName = frmDestination.elements["PlaceName"];
var hdDestinationAddress = frmDestination.elements["Address"];
var centerLatLng = new google.maps.LatLng(21.0226967, 105.8369637);
if (!(hdDestinationLatitude.value == 0 && hdDestinationLongitude.value == 0)) {
    centerLatLng = new google.maps.LatLng(hdDestinationLatitude.value, hdDestinationLongitude.value);
}
function initialize() {
    psContainer = document.getElementById('sp-container');
    var mapOptions = {
        center: centerLatLng,
        zoom: 11
    };
    if (document.getElementById('map-canvas-destinations') != null) {
        mapDestinations = new google.maps.Map(document.getElementById('map-canvas-destinations'), mapOptions);
        var currentPostitionMarker = new google.maps.Marker({
            position: new google.maps.LatLng(currentLatitude, currentLongitude),
            map: mapDestinations
        });
    }
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            if (typeof arrDestination === "undefined" || arrDestination.length == 0) {
                map.setCenter(new google.maps.LatLng(position.coords.latitude, position.coords.longitude));
            }
        }, function () { });
    }
    pacinput = (document.getElementById('pac-input'));
    var types = document.getElementById('type-selector');
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(pacinput);
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(types);
    var autocomplete = new google.maps.places.Autocomplete(pacinput);
    autocomplete.bindTo('bounds', map);
    //Get current destination
    infowindow = new google.maps.InfoWindow();
    if (typeof currentLatitude !== "undefined" && currentLatitude) {
        marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(currentLatitude, currentLongitude),
            visible: true
        });
    } else {
        marker = new google.maps.Marker({
            map: map,
            anchorPoint: new google.maps.Point(0, -29)
        });
    }
    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        psContainer.style.display = 'none';
        infowindow.close();
        marker.setVisible(false);
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }

        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }
        marker.setIcon(/** @type {google.maps.Icon} */({
            url: place.icon,
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
        }));
        marker.setPosition(place.geometry.location);
        marker.setVisible(true);
        var address = '';
        if (typeof place.address_components !== "undefined" && place.address_components) {
            address = [
              (place.address_components[0] && place.address_components[0].short_name || ''),
              (place.address_components[1] && place.address_components[1].short_name || ''),
              (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }
        txtDestinationName.value = place.name;
        hdDestinationPlaceId.value = place.place_id;
        hdDestinationPlaceName.value = place.name;
        if (typeof place.formatted_address !== "undefined" && place.formatted_address) {
            address = place.formatted_address;
        }
        hdDestinationAddress.value = address;
        hdDestinationLatitude.value = place.geometry.location.lat();
        hdDestinationLongitude.value = place.geometry.location.lng();
        if (place.reference && place.reference != null) {
            hdDestinationReference.value = place.reference;
        }

        infowindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
        infowindow.open(map, marker);
    });
    google.maps.event.addListener(map, 'click', function (event) {
        infowindow.close();
        hdDestinationPlaceId.value = '';
        hdDestinationPlaceName.value = '';
        hdDestinationAddress.value = '';
        hdDestinationReference.value = '';
        marker.setPosition(event.latLng);
        hdDestinationLatitude.value = marker.position.lat();
        hdDestinationLongitude.value = marker.position.lng();
        nearbySearch(event.latLng);
    });
    //search place when click on POI
    var fx = google.maps.InfoWindow.prototype.setContent;
    //override the built-in setContent-method
    google.maps.InfoWindow.prototype.setContent = function (content) {
        //when argument is a node
        if (content.querySelector) {
            var name = content.querySelector('.gm-title');
            if (name) {
                document.getElementById('pac-input').blur();
                document.getElementById('pac-input').value = name.textContent;
                document.getElementById('pac-input').focus();
            }
        }
        //run the original setContent-method
        fx.apply(this, arguments);
    };

    if (typeof arrDestination !== "undefined" && arrDestination && arrDestination.length > 0) {
        var bounds = new google.maps.LatLngBounds();
        for (index = 0; index <= arrDestination.length - 1; index++) {
            //Draw curved lines
            if (index < arrDestination.length - 1) {
                $("#map-canvas").curvedLine({
                    LatStart: arrDestination[index].Place.Latitude,
                    LngStart: arrDestination[index].Place.Longitude,
                    LatEnd: arrDestination[index + 1].Place.Latitude,
                    LngEnd: arrDestination[index + 1].Place.Longitude
                });
            }
            var newMarker = new google.maps.Marker({
                position: new google.maps.LatLng(arrDestination[index].Place.Latitude, arrDestination[index].Place.Longitude),
                map: map
            });
            bounds.extend(new google.maps.LatLng(arrDestination[index].Place.Latitude, arrDestination[index].Place.Longitude));
            // process multiple info windows
            (function (newMarker, index) {
                // add click event
                google.maps.event.addListener(newMarker, 'click', function () {
                    infowindow = new google.maps.InfoWindow({
                        content: GetInfoWindowContent(arrDestination[index].Name, arrDestination[index].TakenDate, destinationURL.replace('destinationID=xxx', 'destinationID=' + arrDestination[index].DestinationID))
                    });
                    infowindow.open(map, newMarker);
                });
            })(newMarker, index);
        }
        if (bounds && arrDestination.length > 1) {
            map.fitBounds(bounds);
        } else {
            map.setCenter(new google.maps.LatLng(arrDestination[0].Place.Latitude, arrDestination[0].Place.Longitude));
        }
    }
}
function nearbySearch(location) {
    marker.setIcon();
    map.panTo(location);
    placeService = new google.maps.places.PlacesService(map);
    var request = {
        location: location,
        radius: 500,
        RankBy: google.maps.places.RankBy.DISTANCE,
        types: placeTypes
    };
    placeService.nearbySearch(request, nearbySearch_callback);
}
function nearbySearch_callback(responses, status) {
    nearbyPlaces = responses;
    if (status == google.maps.places.PlacesServiceStatus.OK) {
        displaySuggestionPlaces();
    }
}
function displaySuggestionPlaces() {
    if (nearbyPlaces && nearbyPlaces.length > 0) {
        var html = '';
        var address = '';
        for (i = 0; i < nearbyPlaces.length; i++) {
            if (nearbyPlaces[i].name) {
                if (typeof nearbyPlaces[i].formatted_address !== "undefined" && nearbyPlaces[i].formatted_address) {
                    address = nearbyPlaces[i].formatted_address;
                } else if (nearbyPlaces[i].address_components) {
                    address = [
                      (nearbyPlaces[i].address_components[0] && nearbyPlaces[i].address_components[0].short_name || ''),
                      (nearbyPlaces[i].address_components[1] && nearbyPlaces[i].address_components[1].short_name || ''),
                      (nearbyPlaces[i].address_components[2] && nearbyPlaces[i].address_components[2].short_name || '')
                    ].join(' ');
                }
                html += '<div class=\"sp-item\" latitude=\"' + nearbyPlaces[i].geometry.location.lat() + '\" longitude=\"' + nearbyPlaces[i].geometry.location.lng() + '\" reference=\"' + nearbyPlaces[i].reference + '\" place_id=\"' + nearbyPlaces[i].place_id + '\" place_name=\"' + nearbyPlaces[i].name + '\">' + nearbyPlaces[i].name + '</div>';
            }
        }
        document.getElementById('sp-list').innerHTML = html;
        $('#sp-container').show();
    }
}
function displayPlaceMarker(place_id) {
    if (nearbyPlaces && nearbyPlaces.length > 0) {
        for (var i = 0; i < nearbyPlaces.length; i++) {
            if (nearbyPlaces[i].place_id == place_id) {
                if (tmpMarker != null) {
                    tmpMarker.setMap(null);
                }
                tmpMarker = new google.maps.Marker({
                    map: map,
                    position: nearbyPlaces[i].geometry.location,
                    animation: google.maps.Animation.BOUNCE
                });
                map.panTo(nearbyPlaces[i].geometry.location);
                break;
            }
        }
    }
}
function moveToSuggestionPlace() {
    if (tmpMarker != null) {
        marker.setPosition(tmpMarker.getPosition());
        tmpMarker.setMap(null);
    }
}
google.maps.event.addDomListener(window, 'load', initialize);
$(function () {
    $("#sp-close-button").click(function () { $("#sp-container").hide(); });
    $("#sp-close-button").focus(function () { $("#sp-container").hide(); });
    $(document).on('mouseover', '.sp-item', function (event) {
        //$(event.target).hide();
        displayPlaceMarker($(event.target).attr('place_id'));
    });
    $(document).on('mouseout', '.sp-item', function (event) {
        tmpMarker.setMap(null);
    });
    $(document).on('click', '.sp-item', function (event) {
        moveToSuggestionPlace();
        txtDestinationName.value = this.innerHTML;
        hdDestinationPlaceId.value = $(this).attr("place_id");
        hdDestinationPlaceName.value = $(this).attr("place_name");
        //Retrieve google place details
        var request = {
            placeId: $(this).attr("place_id")
        };
        placeService.getDetails(request, GetPlaceDetails_Callback);
        hdDestinationLatitude.value = $(this).attr("latitude");
        hdDestinationLongitude.value = $(this).attr("longitude");
        hdDestinationReference.value = $(this).attr("reference");
        $("#sp-container").hide();
    });
});
function GetPlaceDetails_Callback(place, status) {
    if (status == google.maps.places.PlacesServiceStatus.OK) {
        hdDestinationAddress.value = place.formatted_address;
    }
}
google.maps.event.addDomListener(window, "resize", resizingMap());

$('#edit-destination-modal').on('show.bs.modal', function() {
    resizeMap();
})

function resizeMap() {
   if(typeof map =="undefined") return;
   setTimeout( function(){resizingMap();} , 400);
}

function resizingMap() {
   if(typeof map =="undefined") return;
   var center = map.getCenter();
   google.maps.event.trigger(map, 'resize');
   map.setCenter(center); 
}

(function ($) {
    var evenOdd = 0;

    $.fn.extend({

        curvedLine: function (options) {

            var defaults = {
                LatStart: null,
                LngStart: null,
                LatEnd: null,
                LngEnd: null,
                Color: "#FF0000",
                Opacity: 1,
                Weight: 3,
                GapWidth: 0,
                Horizontal: true,
                Multiplier: 1,
                Resolution: 0.1,
                Map: map
            }

            var options = $.extend(defaults, options);

            return this.each(function () {

                var o = options;

                var LastLat = o.LatStart;
                var LastLng = o.LngStart;

                var PartLat;
                var PartLng;

                var Points = new Array();
                var PointsOffset = new Array();

                for (point = 0; point <= 1; point += o.Resolution) {
                    Points.push(point);
                    offset = (0.6 * Math.sin((Math.PI * point / 1)));
                    PointsOffset.push(offset);
                }

                var OffsetMultiplier = 0;

                if (o.Horizontal == true) {

                    var OffsetLenght = (o.LngEnd - o.LngStart) * 0.1;

                } else {

                    var OffsetLenght = (o.LatEnd - o.LatStart) * 0.1;

                }

                for (var i = 0; i < Points.length; i++) {

                    if (i == 4) {

                        OffsetMultiplier = 1.5 * o.Multiplier;

                    }

                    if (i >= 5) {

                        OffsetMultiplier = (OffsetLenght * PointsOffset[i]) * o.Multiplier;

                    } else {

                        OffsetMultiplier = (OffsetLenght * PointsOffset[i]) * o.Multiplier;

                    }

                    if (o.Horizontal == true) {

                        PartLat = (o.LatStart + ((o.LatEnd - o.LatStart) * Points[i])) + OffsetMultiplier;
                        PartLng = (o.LngStart + ((o.LngEnd - o.LngStart) * Points[i]));

                    } else {

                        PartLat = (o.LatStart + ((o.LatEnd - o.LatStart) * Points[i]));
                        PartLng = (o.LngStart + ((o.LngEnd - o.LngStart) * Points[i])) + OffsetMultiplier;

                    }

                    curvedLineCreateSegment(LastLat, LastLng, PartLat, PartLng, o.Color, o.Opacity, o.Weight, o.GapWidth, o.Map);

                    LastLat = PartLat;
                    LastLng = PartLng;

                }

                curvedLineCreateSegment(LastLat, LastLng, o.LatEnd, o.LngEnd, o.Color, o.Opacity, o.Weight, o.GapWidth, o.Map);

            });

        }

    });

    function curvedLineCreateSegment(LatStart, LngStart, LatEnd, LngEnd, Color, Opacity, Weight, GapWidth, Map) {

        evenOdd++;

        if (evenOdd % (GapWidth + 1))
            return;

        var LineCordinates = new Array();

        LineCordinates[0] = new google.maps.LatLng(LatStart, LngStart);
        LineCordinates[1] = new google.maps.LatLng(LatEnd, LngEnd);

        var Line = new google.maps.Polyline({
            path: LineCordinates,
            geodesic: false,
            strokeColor: Color,
            strokeOpacity: Opacity,
            strokeWeight: Weight
        });

        Line.setMap(Map);


    }

})(jQuery);
function GetInfoWindowContent(placeName, takenDate, url) {
    var date = new Date(takenDate.substring(0, 19));
    return contentString = '<p><a href="' + url + '"><i class="fa fa-map-marker"></i> ' + placeName + '</a></p><p><i class="fa fa-calendar"></i> ' + $.datepicker.formatDate('d MM yy', date) + '</p>';
}