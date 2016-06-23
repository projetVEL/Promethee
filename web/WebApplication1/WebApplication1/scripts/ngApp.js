var app = angular.module('mainApp', ['ngConstellation']);
app.controller('controller1', ['$scope', 'constellationConsumer', 'constellationController', function ($scope, constellation, controller) {
        
    constellation.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API");
    controller.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "Test API");


    console.log("coucou");


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
 