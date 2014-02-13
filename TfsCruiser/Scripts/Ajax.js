var Ajax;
(function (Ajax) {
    var encoding = Encoding;
    var Request = (function () {
        function Request(httpVerb, uri, isAsync) {
            if (typeof isAsync === "undefined") { isAsync = true; }
            this.uri = uri;
            this.isAsync = isAsync;
            this.headers = [];
            this.callbacks = [];
            this.defaultCallback = null;
            this.requestObject = {
            };
            this.isComplete = false;
            this.httpVerb = httpVerb.toUpperCase();
            this.setRequestObject();
        }
        Request.prototype.addHttpHeader = function (httpHeader) {
            this.headers.push(httpHeader);
        };
        Request.prototype.addBasicAuthentication = function (userName, password) {
            var token = userName + ':' + password;
            var base64 = new encoding.Base64();
            var authorization = base64.encode(token);
            this.headers.push({
                Name: 'Authorization',
                Value: authorization
            });
        };
        Request.prototype.addHttpStatusCallback = function (httpStatusCodes, callback) {
            for(var i = 0; i < httpStatusCodes.length; ++i) {
                var statusCode = httpStatusCodes[i];
                this.callbacks[statusCode] = callback;
            }
        };
        Request.prototype.addDefaultCallback = function (callback) {
            this.defaultCallback = callback;
        };
        Request.prototype.run = function () {
            var request = this.requestObject;
            var self = this;
            if(self.uri.indexOf('?') > -1) {
                self.uri += '&' + new Date().getTime();
            } else {
                self.uri += '?' + new Date().getTime();
            }
            request.open(self.httpVerb, self.uri, self.isAsync);
            for(var i = 0; i < self.headers.length; ++i) {
                request.setRequestHeader(self.headers[i].Name, self.headers[i].Value);
            }
            request.onreadystatechange = function () {
                if(request.readyState == 4 && !self.isComplete) {
                    self.isComplete = true;
                    if(self.callbacks[request.status]) {
                        var functionToCall = self.callbacks[request.status];
                        functionToCall(request);
                    } else if(self.defaultCallback) {
                        self.defaultCallback(request);
                    }
                }
            };
            request.send();
        };
        Request.prototype.setRequestObject = function () {
            if(XMLHttpRequest) {
                this.requestObject = new XMLHttpRequest();
            } else {
                try  {
                    this.requestObject = new ActiveXObject('Msxml2.XMLHTTP');
                } catch (e) {
                    try  {
                        this.requestObject = new ActiveXObject('Microsoft.XMLHTTP');
                    } catch (e) {
                    }
                }
            }
        };
        return Request;
    })();
    Ajax.Request = Request;    
})(Ajax || (Ajax = {}));
