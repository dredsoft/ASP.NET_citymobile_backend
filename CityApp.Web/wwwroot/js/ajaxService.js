var ajaxService = function (type, url, postData, success, failure) {
    if (!window.jQuery) return;
    $.ajax({
        type: type,
        url: url,
        dataType: 'json',
        data: postData,
        contentType: "application/json; charset=utf-8",
        cashe:true
        //contentType: "text/html"
    }).done(function (data, result, xhr) {
        if (success != null) {
            if (success instanceof Array) {
                var l = success.length;
                for (var i = 0; i < l; i++) {
                    success[i](data, result, xhr);
                };
            } else {
                success(data, result, xhr);
            }
        }
    }).fail(function (xhr, status, error) {
        if (failure !== null) failure(xhr, status, error);

    });
};
