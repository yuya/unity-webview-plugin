;(function (global, document) {
var URL_SCHEME = "webviewbridge://",
    userAgent  = navigator.userAgent.toLowerCase(),
    isAndroid  = /android/.test(userAgent),
    body       = document.body,
    iframeBase = document.createElement("iframe"),
    iframe;

// Array.isArray
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/isArray
if (!Array.isArray) {
    Array.isArray = function (obj) {
        return Object.prototype.toString.call(obj) === "[object Array]";
    };
}

// Object.keys
// http://uupaa.hatenablog.com/entry/2012/02/04/145400
if (!Object.keys) {
    Object.keys = function (source) {
        var ret = [], i = 0, key;

        for (key in source) {
            if (source.hasOwnProperty(key)) {
                ret[i++] = key;
            }
        }

        return ret;
    };
}

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
    iframe     = iframeBase.cloneNode(false);
    iframe.src = URL_SCHEME;

    body.appendChild(iframe);
    body.removeChild(iframe);

    iframe = null;
}

function WebViewMediator() {
    var message, stack;

    this.queue   = [];
    this.command = function (path, args) {
        message = isAndroid ? URL_SCHEME + path : path;

        if (args) {
            stack = [];

            each(args, function (key) {
                stack.push(key + "=" + encodeURIComponent(args[key]));
            });

            message += "?" + stack.join("&");
        }

        this.queue.push(message);
        callCustomURLScheme();
    };

    this.callMessage = function () {
        return this.queue.shift();
    };

    global.WebViewMediatorInstance = this;
}

global.WebViewMediator = WebViewMediator;
})(this, this.document);
