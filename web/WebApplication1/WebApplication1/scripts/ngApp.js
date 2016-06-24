<<<<<<< HEAD
﻿var app = angular.module('app', ['ngConstellation']);
app.controller('controller1', ['$scope', 'constellationConsumer', 'constellationController', function ($scope, constellation, controller) {
        
    constellation.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API");
    controller.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API");
=======
﻿var demo = angular.module('demoApp', ['ngConstellation']); console.log(0);
var constellation = $.signalR.createConstellationConsumer("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API");
var controller = $.signalR.createConstellationController("http://localhost:8088", "789", "TestAPI");
>>>>>>> 515545784db5de93168ce3ace86d539c1fca9b2f

console.log(0.5);
demo.controller('MyController', ['$scope', 'constellationConsumer', 'constellationController', function ($scope, constellation, controller) {
    console.log(1);
    constellation.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API"); console.log(2);
    controller.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API"); console.log(3);

    var constellationA = $.signalR.createConstellationConsumer("http://127.0.0.1:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "AConstellation"); console.log(4);

    controller.onReceiveLogMessage(function (log) {
        console.log(log);
    });

    controller.onConnectionStateChanged(function (change) {
        console.log("Controller", change.newState);
    });

    constellation.onUpdateStateObject(function (message) {
        $scope.$apply(function () {
            $scope[message.Name] = message;
        });
    });

    constellation.onConnectionStateChanged(function (change) {
        console.log("Consumer", change.newState);
        if (change.newState === $.signalR.connectionState.connected) {
            constellation.requestSubscribeStateObjects("*", "DemoPackage", "*", "*");
        }
    });

    $scope.Test = function () {
        constellation.sendMessage({ Scope: 'Package', Args: ['DemoPackage'] }, 'TestSebXParam', ['input', 12, 6, false]);
        constellation.sendMessageWithSaga({ Scope: 'Package', Args: ['DemoPackage'] }, 'TestSebXParam', ['input', 12, 6, false], function (msg) { console.log("RESP", msg); });
    };

    constellation.connect();
    controller.connect();
}]);