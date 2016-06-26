var algoApp = angular.module("algoApp", ['ngConstellation']);
algoApp.controller('mainController', ['$scope', 'constellationConsumer', 'constellationController', '$location',
    function ($scope, constellation, controller, $location) {

        $scope.ConditionsPrototype = {
            "Value": undefined,
            "DynamicValue": null,
            "OperationTested": undefined,
            "IsTrue": false,
            "Variables": {
                "sentinel": "",
                "package": "",
                "variable": ""
            }
        };
        $scope.ExecutionPrototype = {
            "Arguments": [],
            "Variables": {
                "sentinel": "",
                "package": "",
                "callBack": ""
            }
        };
        $scope.Algorithme =
            {
                "Description": "",
                "URLPhotoDescription": "http://lorempixel.com/700/300/technics/",
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
        $scope.ConnectionControllerSuccess = false;
        $scope.ConnectionSuccess = false;
        $scope.LoadingAlgoSuccess = false;
        $scope.arbre = {};

        if ($location.search().sentinel) {
            $scope.sent = $location.search().sentinel;
        }
        $scope.algoName = undefined;

        if ($location.search().name) {
            $scope.algoName = $location.search().name;
        }

        $scope.listeAlgos = [];

        constellation.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "constellationAlgoApp");
        controller.intializeClient("http://localhost:8088", "fcfd2cff6a98b16994233b6c25be3860b0caff04", "controllerAlogApp");

        constellation.onUpdateStateObject(function (stateobject) {
            $scope.$apply(function () {
                //cree une nouvelle sentinelle
                if ($scope.arbre[stateobject.SentinelName] == undefined) {
                    $scope.arbre[stateobject.SentinelName] = {};
                }
                //cree un nouveau package a la sentinelle
                if ($scope.arbre[stateobject.SentinelName][stateobject.PackageName] == undefined) {
                    $scope.arbre[stateobject.SentinelName][stateobject.PackageName] = {};
                }
                //associe le SO au couple sentinel/package
                $scope.arbre[stateobject.SentinelName][stateobject.PackageName][stateobject.Name] = stateobject;

                if ($scope.arbre[$scope.sent] != undefined && $scope.arbre[$scope.sent]['AlgorithmePackage'] != undefined && $scope.arbre[$scope.sent]['AlgorithmePackage']['PausedAlgorithmes'] != undefined) {

                    $scope.listeAlgos[0] = $scope.arbre[$scope.sent]['AlgorithmePackage']['PausedAlgorithmes'];
                }
                if ($scope.arbre[$scope.sent] != undefined && $scope.arbre[$scope.sent]['AlgorithmePackage'] != undefined && $scope.arbre[$scope.sent]['AlgorithmePackage']['Algorithmes'] != undefined) {

                    $scope.listeAlgos[1] = $scope.arbre[$scope.sent]['AlgorithmePackage']['Algorithmes'];
                }
                if ($scope.listeAlgos.length == 2) {
                    $scope.loadAlgo($scope.listeAlgos);
                    $scope.LoadingAlgoSuccess = true;
                }
            });
        });

        constellation.onConnectionStateChanged(function (change) {
            if (change.newState == $.signalR.connectionState.connected) {
                $scope.ConnectionSuccess = true;
                constellation.requestStateObjects("*", "*", "*", "*");                
            }
        });
        controller.onConnectionStateChanged(function (change) {
            if (change.newState == $.signalR.connectionState.connected) {
                $scope.getSentinels();
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

        $scope.addExecution = function () {
            ExecutionPrototype = {
                "Arguments": [],
                "Variables": {
                    "sentinel": "",
                    "package": "",
                    "callBack": ""
                }
            };
            $scope.Algorithme.Executions.push(ExecutionPrototype);
        };
        $scope.addCondition = function () {
            ConditionsPrototype = {
                "Value": undefined,
                "DynamicValue": null,
                "OperationTested": undefined,
                "IsTrue": false,
                "Variables": {
                    "sentinel": "",
                    "package": "",
                    "variable": ""
                }
            };
            $scope.Algorithme.Conditions.push(ConditionsPrototype);
        };
        $scope.deleteCondition = function () {
            $scope.Algorithme.Conditions.pop();
        };
        $scope.deleteExecution = function () {
            $scope.Algorithme.Executions.pop();
        };
        $scope.verifAndSendAlgo = function () {
            //faire les verifs d'algo : conditions/exec vides, ...

            //fonctionne
            //constellation.sendMessage({ Scope: 'Package', Args: ['DESKTOP-E5D5ULL' + '/package1'] }, 'insert', 'hilihou');

            // ! si on n'a qu'un seul argument a revoyer (ce qui est le cas ici), on renvoie un élément simple, pas un tableau
            //constellation.sendMessage({ Scope: 'Sentinel', Args: [$scope.sent + '/AlgorithmePackage'] }, 'AddAlgorithme', $scope.Algorithme);
            window.location = 'http://localhost:56215/';
        }
        controller.onUpdateSentinelsList(function (sentinels) {
            $scope.$apply(function () {
                for (i in sentinels.List) {
                    if ($scope.arbre[sentinels.List[i].Description.SentinelName] == undefined) {
                        $scope.arbre[sentinels.List[i].Description.SentinelName] = {};
                    }
                    controller.requestPackagesList(sentinels.List[i].Description.SentinelName);
                }
            });            
        });
        controller.onUpdatePackageList(function (packages) {
            $scope.$apply(function () {
                for (i in packages.List)
                {
                    if ($scope.arbre[packages.SentinelName][packages.List[i].Package.Name] == undefined) {
                        $scope.arbre[packages.SentinelName][packages.List[i].Package.Name] = {};
                    }
                    controller.requestPackageDescriptor(packages.List[i].Package.Name);
                }
            });
        });
        controller.onUpdatePackageDescriptor(function (packageDescriptor) {
            $scope.$apply(function () {
                for (i in $scope.arbre) {
                    if ($scope.arbre[i][packageDescriptor.PackageName] != undefined) {
                        $scope.arbre[i][packageDescriptor.PackageName].PackageDescriptor = packageDescriptor.Descriptor;                        
                    }
                }
            });
        });        

        $scope.getSentinels = function () {
            controller.requestSentinelsList();           
        };



        controller.connect();
        constellation.connect();
    }]);
algoApp.controller('titleController', ['$scope', '$location',
    function ($scope, $location) {
        $scope.title = "Nouvel Algorithme";
        if ($location.search().name) {
            $scope.title = "Algorithme : " + $location.search().name;
        }
    }]);
