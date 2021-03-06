module Notifier {
    export class MessageType {
        static normal: string = 'messageTypeNormal';
        static success: string = 'messageTypeSuccess';
        static error: string = 'messageTypeError';
    }

    export class Overlay {
        private alertTimer;

        alert(message: string, messageType: string = MessageType.normal) {
            window.clearTimeout(this.alertTimer);
            var notification = document.getElementById('NotifierOverlayAlert') || this.createMessageElement();

            notification.className = messageType;
            notification.innerHTML = message;

            this.setOpacity(notification, 1);
            this.alertTimer = window.setTimeout(() => {
                this.setOpacity(notification, 0);
            }, 10 * 1000);
        }

        private setOpacity(element: HTMLElement, value: number) {
            element.style['webkitOpacity'] = value.toString();
            element.style['mozOpacity'] = value.toString();
            element.style['filter'] = 'alpha(opacity = ' + (value * 100) + ')';
            element.style.opacity = value.toString();
        }

        private setTransition(element: HTMLElement, value: string) {
            element.style['msTransition'] = 'all 0.5s ease';
            element.style['webkitTransition'] = 'all 0.5s ease';
            element.style['MozTransition'] = 'all 0.5s ease';
            element.style['OTransition'] = 'all 0.5s ease';
            element.style.transition = 'all 0.5s ease';
        }

        private createMessageElement() {
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
        }
    }
}