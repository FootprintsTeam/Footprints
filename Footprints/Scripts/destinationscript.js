
var map;
google.maps.event.addDomListener(window, 'load', initialize);

function initialize() {

    /* Create Google Map */
    var myOptions = {
        zoom: 6,
        center: new google.maps.LatLng(41, 19.6),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById('map-canvas'), myOptions);

    /* Add you'r curved lines here */
    $("#map-canvas").curvedLine({

        LatStart: 42.68243562027229,
        LngStart: 23.280029421875042,
        LatEnd: 42.488302202180364,
        LngEnd: 27.432861453125042
    });

}

function loadScript() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&' +
        'callback=initialize';
    document.body.appendChild(script);
}

window.onload = loadScript;

function loadImageLibrary() {
    jQuery("#media-container").nanoGallery({
        kind: 'flickr',
        userID: '99932419@N07'
    });
}

function loadCommentSection() {
    $('div.comment-container').comment({
        title: 'Comments',
        url_get: 'articles/id/1/comments/list',
        url_input: 'articles/id/1/comments/input',
        url_delete: 'articles/id/1/comments/delete',
        limit: 10,
        auto_refresh: false,
        refresh: 10000,
        transition: 'slideToggle',
    });

}

$('.dropdown-toggle').dropdown();

$(document).ready(function () {
    loadImageLibrary();
    loadCommentSection();

});


//-----------------------------------------
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