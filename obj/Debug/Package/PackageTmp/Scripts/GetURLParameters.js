function getUrlVars() {

    var vars = [], hash;
    var hashes;
    if (($.mobile.activePage)) {
        if ($.mobile.activePage.data('url').indexOf("?") != -1) {
            // first check the active page url for parameters           
            hashes = $.mobile.activePage.data('url').slice($.mobile.activePage.data('url').indexOf('?') + 1).split('&');
        }
        else { // otherwise just get the current url           
            hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        }
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            if (hash.length > 1 && hash[1].indexOf("#") == -1) {
                // filter out misinterpreted paramters caused by JQM url naming scheme                
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
        }
    }
    return vars;
}

function getUrlVarsPage(pageId) {

    var vars = [], hash;
    var hashes;
    if (($("#" + pageId).attr("data-url"))) {
        if ($("#" + pageId).attr("data-url").indexOf("?") != -1) {
            // first check the active page url for parameters           
            hashes = $("#" + pageId).attr("data-url").slice($("#" + pageId).attr("data-url").indexOf('?') + 1).split('&');
        }
        else { // otherwise just get the current url           
            hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        }
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            if (hash.length > 1 && hash[1].indexOf("#") == -1) {
                // filter out misinterpreted paramters caused by JQM url naming scheme                
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
        }
    }
    return vars;
}