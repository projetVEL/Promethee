angular
    .module("algoApp", ['ngConstellation'])
    .controller('mainController', ['$scope', 'constellationConsumer',
    function ($scope, constellation) {

        $scope.sent = "Developer";
        $scope.state = false;
        $scope.listeAlgos = [];

        constellation.intializeClient("http://localhost:8088", "8dea78b76b83d2ea291ed68db80e5cb1fd630ec8", "test");

        constellation.onUpdateStateObject(function (stateobject) {
            $scope.$apply(function () {

                if ($scope[stateobject.SentinelName] == undefined) {
                    $scope[stateobject.SentinelName] = {};
                }
                if ($scope[stateobject.SentinelName][stateobject.PackageName] == undefined) {
                    $scope[stateobject.SentinelName][stateobject.PackageName] = {}; 
                }
                $scope[stateobject.SentinelName][stateobject.PackageName][stateobject.Name] = stateobject;
                if ($scope['Developer'] != undefined && $scope['Developer']['AlgorithmePackage'] != undefined && $scope['Developer']['AlgorithmePackage']['PausedAlgorithmes'] != undefined) {
                    
                    $scope.listeAlgos[0] = $scope['Developer']['AlgorithmePackage']['PausedAlgorithmes'];
                }
                if ($scope['Developer'] != undefined && $scope['Developer']['AlgorithmePackage'] != undefined && $scope['Developer']['AlgorithmePackage']['Algorithmes'] != undefined) {

                    $scope.listeAlgos[1] = $scope['Developer']['AlgorithmePackage']['Algorithmes'];
                }
            });
        });
        //$scope.state; équivaut à $scope['state'];

        constellation.onConnectionStateChanged(function (change) {
            $scope.$apply(function () {
                $scope.state = change.newState == $.signalR.connectionState.connected;
                var a = $scope.state;
            });

            if (change.newState == $.signalR.connectionState.connected) {
                constellation.requestSubscribeStateObjects("*", "AlgorithmePackage", "*", "*");
            }
        });




        $scope.Test = function () {
            constellation.sendMessage({ Scope: 'Package', Args: ['ConstellationPackageConsole1'] }, 'changeVal', [42, "send from angular"]);
        };

        constellation.connect();

    }]);