var algoApp = angular.module("algoApp", ['ngConstellation']);
algoApp.controller('mainController', ['$scope', 'constellationConsumer', '$location',
    function ($scope, constellation, $location) {
        $scope.ConditionsPrototype ={
                "Value": 0,
                "DynamicValue": null,
                "OperationTested": 3,
                "IsTrue": false,
                "Variables": {
                    "sentinel": "",
                    "package": "",
                    "variable": ""
                }};
        $scope.ExecutionPrototype ={
                    "Arguments": [],
                "Variables": {
                    "sentinel": "",
                    "package": "",
                    "callBack": ""
                }};
        $scope.Algorithme =
            {
                "Description": "",
                "URLPhotoDescription": "",
                "Waiting": 0,
                "IsActive": true,
                "Conditions": [
                    $scope.ConditionsPrototype
                ],
                "Executions": [
                    $scope.ExecutionPrototype
                ],
                "Name": "",
                "DisableAfterRealisation": false,
                "Schedule": {
                    "Week": {
                        "Sunday": true,
                        "Monday": true,
                        "Tuesday": true,
                        "Wednesday": true,
                        "Thursday": true,
                        "Friday": true,
                        "Saturday": true
                    },
                    "Begin": {
                        "Second": 0,
                        "Minute": 0,
                        "Hour": 0,
                        "Day": 1,
                        "Month": 1
                    },
                    "End": {
                        "Second": 59,
                        "Minute": 59,
                        "Hour": 23,
                        "Day": 31,
                        "Month": 12
                    },
                    "ReactivationPeriode": "Minutes"
                }
            }

        $scope.sent = $location.search().sentinel;
        $scope.algoName = undefined;

        if ($location.search().name) {
            $scope.algoName = $location.search().name;
        }

        $scope.state = false;
        $scope.listeAlgos = [];

        constellation.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "test");

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
                if ($scope.listeAlgos.length == 2 && $scope.algoName != undefined) {
                    $scope.loadAlgo($scope.listeAlgos);
                }
            });
        });

        constellation.onConnectionStateChanged(function (change) {
            $scope.$apply(function () {
                $scope.state = change.newState == $.signalR.connectionState.connected;
            });

            if (change.newState == $.signalR.connectionState.connected) {

                constellation.requestSubscribeStateObjects("*", "AlgorithmePackage", "*", "*");
            }
        });
        $scope.loadAlgo = function (algos) {
            for (i in algos) {
                for (j in algos[i].Value) {
                    if (algos[i].Value[j].Name == $scope.algoName) {
                        $scope.Algorithme = algos[i].Value[j];
                    }
                }
            }
        };

        $scope.addExecution = function()
        {
            $scope.Algorithme.Executions.push($scope.ExecutionsPrototype);
        }
        $scope.addCondition = function () {
            $scope.Algorithme.Conditions.push($scope.ConditionsPrototype);
        }



        constellation.connect();
    }]);
algoApp.controller('titleController', ['$scope', '$location',
    function ($scope, $location) {
        $scope.title = "Nouvel Algorithme";
        if ($location.search().algo) {
            $scope.title = "Algorithme " + $location.search().algo;
        }
    }]);

