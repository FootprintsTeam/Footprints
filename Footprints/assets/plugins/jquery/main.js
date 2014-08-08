/*
 * jQuery File Upload Plugin JS Example 8.9.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/* global $, window */

$(function () {
    'use strict';
    $('#fileupload').fileupload({
        //url: $('#fileupload').attr('action') + '/' + $('#albumid').val(),
        url: $('#fileupload').attr('action'),
        // Enable image resizing, except for Android and Opera,
        // which actually support image resizing, but fail to
        // send Blob objects via XHR requests:
        disableImageResize: true,
        maxFileSize: 8 * 1024 * 1024,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i
    });
    
});

