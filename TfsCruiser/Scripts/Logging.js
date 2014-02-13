var Logging;
(function (Logging) {
    var Console = (function () {
        function Console() { }
        Console.targetConsole = window.console;
        Console.log = function log(message) {
            if(typeof Console.targetConsole !== 'undefined') {
                message = Console.getTimeStamp() + ' ' + message;
                Console.targetConsole.log(message);
            }
        };
        Console.getTimeStamp = function getTimeStamp() {
            var date = new Date();
            return Console.pad(date.getHours(), 2) + ':' + Console.pad(date.getMinutes(), 2) + ':' + Console.pad(date.getSeconds(), 2) + ':' + Console.pad(date.getMilliseconds(), 3);
        };
        Console.pad = function pad(input, length) {
            var output = input.toString();
            while(output.length < length) {
                output = '0' + output;
            }
            return output;
        };
        return Console;
    })();
    Logging.Console = Console;    
})(Logging || (Logging = {}));
