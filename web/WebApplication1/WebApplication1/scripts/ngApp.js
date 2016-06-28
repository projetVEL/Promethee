angular
    .module("algoApp", ['ngConstellation'])
    .controller('mainController', ['$scope', 'constellationConsumer',
    function ($scope, constellation) {

        $scope.state = false;
        $scope.listeAlgos = [];
        $scope.listeSentinelsName = [];
        $scope.sentinelName = undefined;
        $scope.ConnectionWait = true;
        $scope.ConnectionSuccess = false;

        constellation.intializeClient("http://localhost:8088", "1182b2384b6f311117b8fbbf4c37087982007885", "test");

        constellation.onUpdateStateObject(function (stateobject) {
            $scope.$apply(function () {
                //cree une nouvelle sentinelle
                if ($scope[stateobject.SentinelName] == undefined) {
                    $scope[stateobject.SentinelName] = {};
                    //on cree une liste de sentinelles ayant le package AlgorithmePackage
                    $scope.listeSentinelsName.push(stateobject.SentinelName);
                    $scope.sentinelName = stateobject.SentinelName;
                }
                //cree un nouveau package a la sentinelle
                if ($scope[stateobject.SentinelName][stateobject.PackageName] == undefined) {
                    $scope[stateobject.SentinelName][stateobject.PackageName] = {}; 
                }
                //associe le SO au couple sentinel/package
                $scope[stateobject.SentinelName][stateobject.PackageName][stateobject.Name] = stateobject;

                //definit la sentinelle a prendre (de preference pas Developer)
                for (sentinel in $scope.listeSentinelsName)
                {
                    if ($scope.listeSentinelsName[sentinel] != "Developer")
                    {
                        $scope.sentinelName = $scope.listeSentinelsName[sentinel];
                    }
                }
                //console.log($scope.listeAlgos);

                if ($scope[$scope.sentinelName] != undefined && $scope[$scope.sentinelName]['AlgorithmePackage'] != undefined && $scope[$scope.sentinelName]['AlgorithmePackage']['PausedAlgorithmes'] != undefined) {
                    
                    $scope.listeAlgos[0] = $scope[$scope.sentinelName]['AlgorithmePackage']['PausedAlgorithmes'];
                }
                if ($scope[$scope.sentinelName] != undefined && $scope[$scope.sentinelName]['AlgorithmePackage'] != undefined && $scope[$scope.sentinelName]['AlgorithmePackage']['Algorithmes'] != undefined) {

                    $scope.listeAlgos[1] = $scope[$scope.sentinelName]['AlgorithmePackage']['Algorithmes'];
                }
            });
        });
        constellation.onConnectionStateChanged(function (change) {
            $scope.$apply(function () {
                $scope.state = change.newState == $.signalR.connectionState.connected;
            });

            if (change.newState == $.signalR.connectionState.connected) {
                constellation.requestStateObjects("*", "AlgorithmePackage", "*", "*");
                $scope.ConnectionSuccess = true;
                $scope.ConnectionWait = false;
            }
        });


        constellation.connect();

    }]);