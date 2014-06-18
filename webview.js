// JS library for Unity webview plugin 1.0.4 Copyright (C) 2014 Yuya Hashimoto, zlib License.
// See https://github.com/yuya/unity-webview-plugin
;(function (global, document) {
var URL_SCHEME = "webviewbridge://",
    queue      = [],
    body       = document.body,
    iframeBase = document.createElement("iframe"),
    inlineCSS  = "position: absolute; width: 1px; height: 1px; border: none; visibility: hidden;",
    isAndroid  = /Android/.test(navigator.userAgent),
    WebViewMediator;

iframeBase.setAttribute("style", inlineCSS);

function each(collection, iterator) {
    var i = 0, len, ary, key;

    if (Array.isArray(collection)) {
        len = collection.length;

        for (; len; ++i, --len) {
            iterator(collection[i], i);
        }
    }
    else {
        ary = Object.keys(collection);
        len = ary.length;

        for (; len; ++i, --len) {
            key = ary[i];
            iterator(key, collection[key]);
        }
    }
}

function callCustomURLScheme() {
    var iframe = iframeBase.cloneNode(false);

    iframe.src = URL_SCHEME;

    body.appendChild(iframe);
    body.removeChild(iframe);

    iframe = null;
}

WebViewMediator = {
    Call: function (path, args) {
        var message = isAndroid ? URL_SCHEME + path : path,
            stack;

        if (args) {
            stack = [];

            each(args, function (key) {
                stack.push(key + "=" + encodeURIComponent(args[key]));
            });

            message += "?" + stack.join("&");
        }

        queue.push(message);
        callCustomURLScheme();
    },
    ShiftQueue: function () {
        return queue.length ? queue.shift() : "";
    }
};

global.WebViewMediator = WebViewMediator;
})(this, this.document);
