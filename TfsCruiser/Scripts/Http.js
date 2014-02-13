var Http;
(function (Http) {
    var HttpVerb = (function () {
        function HttpVerb() { }
        HttpVerb.CONNECT = 'CONNECT';
        HttpVerb.DELETE = 'DELETE';
        HttpVerb.GET = 'GET';
        HttpVerb.HEAD = 'HEAD';
        HttpVerb.OPTIONS = 'OPTIONS';
        HttpVerb.POST = 'POST';
        HttpVerb.PUT = 'PUT';
        HttpVerb.TRACE = 'TRACE';
        return HttpVerb;
    })();
    Http.HttpVerb = HttpVerb;    
})(Http || (Http = {}));
