angular.module('app').controller('prayertimeCtrl', ['$scope', '$localStorage', '$http', function ($scope, $localStorage, $http) {
    $scope.day = moment();
 
    $scope.prayerTimes = [];

    $scope.location = $localStorage.location;


    $scope.$watch(function () {
        return $localStorage.location;
    }, function (newCodes, oldCodes) {
            if (newCodes) {
                $scope.getPrayerTimes(newCodes);
                $scope.location = $localStorage.location;
            }

    });

    $scope.getPrayerTimes = function (locationInfo) {
        $http({
            method: 'GET',
            url: "api/prayertimes",
            params: {
                zipcode: locationInfo.zipCode,
                longitude: locationInfo.longitude,
                latitude: locationInfo.latitude,
                GMT: locationInfo.GMT,
                hasDayLightSavings: locationInfo.daylight
            }
        }).then(function successCallback(response) {
            console.log(response.data);
            $scope.prayerTimes = response.data;

        }, function errorCallback(response) {
                $scope.prayerTimesHasError = true;
                console.log(response);
        });
    };
    
}]);