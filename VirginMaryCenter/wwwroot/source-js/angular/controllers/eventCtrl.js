angular.module('app').controller('eventCtrl', ['$scope', '$window', '$http', '$sce', function ($scope, $window, $http, $sce) {
    $scope.loading = true;
    $scope.eventPeriods =
        {
            "live": {
                name: "LIVE EVENTS",
                data: []
            },
            "upcoming": {
                name: "UPCOMING EVENTS",
                data: []
            },
            "past": {
                name: "PAST EVENTS",
                data: [] 
            }
        };

    $scope.eventView = function () {
        $scope.loading = true;

        $http({
            method: 'GET',
            url: "/api/events",
            params: {
            }
        }).then(function successCallback(response) {
            var eventsAll = JSON.parse(response.data).data
            $scope.eventPeriods["live"].data = eventsAll.filter(e => new Date(e.start_time) >= new Date() && new Date(e.end_time) <= new Date());
            $scope.eventPeriods["upcoming"].data = eventsAll.filter(e => new Date(e.start_time) >= new Date());
            $scope.eventPeriods["past"].data = eventsAll.filter(e => new Date(e.start_time) < new Date());

            console.log($scope.eventPeriods);
            $scope.loading = false;
        }, function errorCallback(response) {
            $scope.hasError = true;
        });
    };


    $scope.eventView();

}]);