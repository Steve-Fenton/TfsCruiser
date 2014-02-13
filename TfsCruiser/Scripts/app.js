((function () {
    var overlay = new Notifier.Overlay();
    var interval;
    var updateSeconds = 60;
    var notifySeconds = 30;
    var countDown = updateSeconds;
    var nav = document.getElementById('bar');
    var clock = document.createElement('li');
    nav.appendChild(clock);
    var runCountDown = function () {
        interval = window.setInterval(tickTock, 1000);
    };
    var pauseCountDown = function () {
        window.clearInterval(interval);
    };
    var ajaxOkayCallback = function (response) {
        document.getElementById('main').innerHTML = response.responseText;
        overlay.alert('Build information updated', Notifier.MessageType.success);
        runCountDown();
    };
    var ajaxProblemCallback = function () {
        overlay.alert('There was a problem obtaining the latest build information', Notifier.MessageType.error);
        runCountDown();
    };
    var getAddress = function () {
        var address = document.location.href;
        if(address.indexOf('/Builds/Index') > -1) {
            address = address.replace('/Index', '/Update');
        } else {
            address = location.protocol + '//' + location.hostname + (location.port && ':' + location.port) + '/Builds/Update';
        }
        return address;
    };
    var requestUpdate = function () {
        overlay.alert('Updating...', Notifier.MessageType.success);
        var ajaxRequest = new Ajax.Request(Http.HttpVerb.GET, getAddress());
        ajaxRequest.addHttpStatusCallback([
            200
        ], ajaxOkayCallback);
        ajaxRequest.addDefaultCallback(ajaxProblemCallback);
        ajaxRequest.run();
    };
    var tickTock = function () {
        countDown--;
        clock.innerHTML = 'Next Update: ' + countDown;
        if(countDown === 0) {
            pauseCountDown();
            countDown = updateSeconds;
            requestUpdate();
        }
    };
    runCountDown();
})());
