/*
     * jQuery File Upload Plugin JS Example
     * https://github.com/blueimp/jQuery-File-Upload
     *
     * Copyright 2010, Sebastian Tschan
     * https://blueimp.net
     *
     * Licensed under the MIT license:
     * http://www.opensource.org/licenses/MIT
     */


function setupFileUpload(previewMaxWidth,Token) {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload_'+Token).fileupload({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        previewMaxWidth: previewMaxWidth,
        downloadTemplateId: 'template-download_' + Token,
        uploadTemplateId: 'template-upload_' + Token
    });

    // Enable iframe cross-domain access via redirect option:
    $('#fileupload_' + Token).fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );
    
    // Load existing files:
    $('#fileupload_' + Token).addClass('fileupload-processing');
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: $('#fileupload_' + Token).fileupload('option', 'url'),
        dataType: 'json',
        context: $('#fileupload_' + Token)[0]
    }).always(function () {
        $(this).removeClass('fileupload-processing');
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, $.Event('done'), { result: result });
    });

}
