﻿var map;
var marker;
var tmpMarker;
var infowindow;
var pacinput;
var placeService;
var psContainer;
var nearbyPlaces;
var placeTypes = ["accounting", "airport", "amusement_park", "aquarium", "art_gallery", "atm", "bakery", "bank", "bar", "beauty_salon", "bicycle_store", "book_store", "bowling_alley", "bus_station", "cafe", "campground", "car_dealer", "car_rental", "car_repair", "car_wash", "casino", "cemetery", "church", "city_hall", "clothing_store", "convenience_store", "courthouse", "dentist", "department_store", "doctor", "electrician", "electronics_store", "embassy", "establishment", "finance", "fire_station", "florist", "food", "funeral_home", "furniture_store", "gas_station", "general_contractor", "grocery_or_supermarket", "gym", "hair_care", "hardware_store", "health", "hindu_temple", "home_goods_store", "hospital", "insurance_agency", "jewelry_store", "laundry", "lawyer", "library", "liquor_store", "local_government_office", "locksmith", "lodging", "meal_delivery", "meal_takeaway", "mosque", "movie_rental", "movie_theater", "moving_company", "museum", "night_club", "painter", "park", "parking", "pet_store", "pharmacy", "physiotherapist", "place_of_worship", "plumber", "police", "post_office", "real_estate_agency", "restaurant", "roofing_contractor", "rv_park", "school", "shoe_store", "shopping_mall", "spa", "stadium", "storage", "store", "subway_station", "synagogue", "taxi_stand", "train_station", "travel_agency", "university", "veterinary_care", "zoo"];
var frmNewDestination = document.forms['frmNewDestination'];
var txtDestinationName = frmNewDestination.elements["Name"];
var hdDestinationPlaceId = frmNewDestination.elements["PlaceId"];
var hdDestinationLocation = frmNewDestination.elements["Location"];
function initialize() {
    psContainer = document.getElementById('sp-container');
    var mapOptions = {
        center: new google.maps.LatLng(-33.8688, 151.2195),
        zoom: 17
    };
    map = new google.maps.Map(document.getElementById('map-canvas'),
      mapOptions);

    pacinput = /** @type {HTMLInputElement} */(
        document.getElementById('pac-input'));

    var types = document.getElementById('type-selector');
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(pacinput);
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(types);

    var autocomplete = new google.maps.places.Autocomplete(pacinput);
    autocomplete.bindTo('bounds', map);

    infowindow = new google.maps.InfoWindow();
    marker = new google.maps.Marker({
        map: map,
        anchorPoint: new google.maps.Point(0, -29)
    });

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
        if (place.address_components) {
            address = [
              (place.address_components[0] && place.address_components[0].short_name || ''),
              (place.address_components[1] && place.address_components[1].short_name || ''),
              (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }

        txtDestinationName.value = place.name;
        hdDestinationPlaceId.value = place.place_id;
        hdDestinationLocation.value = place.geometry.location;

        infowindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
        infowindow.open(map, marker);
    });

    google.maps.event.addListener(map, 'click', function (event) {
        marker.setPosition(event.latLng);
        nearbySearch(event.latLng);
    });

    //search place when click on POI
    var fx = google.maps.InfoWindow.prototype.setPosition;
    google.maps.InfoWindow.prototype.setPosition = function () {
        $("#sp-container").hide();
        if (this.logAsInternal) {
            google.maps.event.addListenerOnce(this, 'map_changed', function () {
                var map = this.getMap();
                if (map) {
                    marker.setPosition(this.getPosition());
                    searchExactlyPlace(this.getPosition());
                }
            });
        }
        fx.apply(this, arguments);
        infowindow.close();
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
function searchExactlyPlace(location) {
    map.panTo(location);
    placeService = new google.maps.places.PlacesService(map);
    var request = {
        location: location,
        radius: 5,
        RankBy: google.maps.places.RankBy.DISTANCE,
        types: placeTypes
    };
    placeService.nearbySearch(request, searchExactlyPlace_callback);
}
function searchExactlyPlace_callback(responses, status) {
    if (status == google.maps.places.PlacesServiceStatus.OK && responses && responses.length > 0) {
        marker.setPosition(responses[0].geometry.location);
        map.panTo(responses[0].geometry.location);
    }
}
function displaySuggestionPlaces() {
    if (nearbyPlaces && nearbyPlaces.length > 0) {
        var html = '';
        for (i = 0; i < nearbyPlaces.length; i++) {
            if (nearbyPlaces[i].name) {
                html += '<div class=\"sp-item\" location=\"' + nearbyPlaces[i].geometry.location + '\" place_id=\"' + nearbyPlaces[i].place_id + '\">' + nearbyPlaces[i].name + '</div>';
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
        hdDestinationLocation.value = $(this).attr("location");
        $("#sp-container").hide();
    });
});