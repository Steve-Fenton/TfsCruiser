var Notifier;
(function (Notifier) {
    var MessageType = (function () {
        function MessageType() { }
        MessageType.normal = 'messageTypeNormal';
        MessageType.success = 'messageTypeSuccess';
        MessageType.error = 'messageTypeError';
        return MessageType;
    })();
    Notifier.MessageType = MessageType;    
    var Overlay = (function () {
        function Overlay() { }
        Overlay.prototype.alert = function (message, messageType) {
            if (typeof messageType === "undefined") { messageType = MessageType.normal; }
            var _this = this;
            window.clearTimeout(this.alertTimer);
            var notification = document.getElementById('NotifierOverlayAlert') || this.createMessageElement();
            notification.className = messageType;
            notification.innerHTML = message;
            this.setOpacity(notification, 1);
            this.alertTimer = window.setTimeout(function () {
                _this.setOpacity(notification, 0);
            }, 10 * 1000);
        };
        Overlay.prototype.setOpacity = function (element, value) {
            element.style['webkitOpacity'] = value.toString();
            element.style['mozOpacity'] = value.toString();
            element.style['filter'] = 'alpha(opacity = ' + (value * 100) + ')';
            element.style.opacity = value.toString();
        };
        Overlay.prototype.setTransition = function (element, value) {
            element.style.msTransition = 'all 0.5s ease';
            element.style['webkitTransition'] = 'all 0.5s ease';
            element.style['MozTransition'] = 'all 0.5s ease';
            element.style['OTransition'] = 'all 0.5s ease';
            element.style.transition = 'all 0.5s ease';
        };
        Overlay.prototype.createMessageElement = function () {
            var messageElement = document.createElement('div');
            messageElement.id = 'NotifierOverlayAlert';
            messageElement.style.position = 'fixed';
            messageElement.style.bottom = '10px';
            messageElement.style.width = '90%';
            messageElement.style.left = '4%';
            messageElement.style.padding = '1%';
            this.setOpacity(messageElement, 0);
            this.setTransition(messageElement, 'all 0.5s ease');
            document.body.appendChild(messageElement);
            return messageElement;
        };
        return Overlay;
    })();
    Notifier.Overlay = Overlay;    
})(Notifier || (Notifier = {}));
