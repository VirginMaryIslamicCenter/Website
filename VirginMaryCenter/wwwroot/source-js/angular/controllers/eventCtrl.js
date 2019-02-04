angular.module('app').controller('eventCtrl', ['$scope', '$window', '$http', '$sce', function ($scope, $window, $http, $sce) {
    $scope.loading = true;
    $scope.events = [];

    $scope.eventView = function() {
        $scope.loading = true;

        $http({
            method: 'GET',
            url: "/api/events",
            params: {
            }
        }).then(function successCallback(response) {
            $scope.events = response.data.data;
            console.log($scope.events);
            $scope.loading = false;
        }, function errorCallback(response) {
            $scope.hasError = true;
        });
    };


    $scope.eventView();

}]);