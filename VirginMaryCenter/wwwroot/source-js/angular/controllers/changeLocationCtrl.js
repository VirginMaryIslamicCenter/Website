angular.module('app').controller('changeLocationCtrl', ['$scope', '$localStorage','$http', function ($scope, $localStorage, $http) {
    $scope.ChangeLocation = function () {
        $scope.ZipCodeList = null;

        $http({
            method: 'GET',
            url: "api/prayertimes/validateLocation",
            params: {
                ZipOrCityState: $scope.inputLocation
            }
        }).then(function successCallback(response) {
            console.log(response.data);
            if (!response.data.length && response.data.zipCode) {
                $localStorage.location = response.data;
                $('#userLocation').modal('hide');
            }
            else if (response.data.length === 1) {
                $localStorage.location = response.data;
                $('#userLocation').modal('hide');
            }
            else {
                $scope.ZipCodeList = response.data;
            }


            //$('#userLocation').modal('hide');

        }, function errorCallback(response) {
            if (response.data.errorMsg)
                $scope.AddressErrorMsg = response.data.errorMsg;
            else
                $scope.AddressErrorMsg = 'Unknown error occured. Try again later.';

            console.log(response);
        });
    };


    if (!$localStorage.location) {
        $scope.inputLocation = '94587';
        console.log("changing to default location");
        $scope.ChangeLocation();
        console.log($localStorage.location);

    }
    
    $scope.AddressErrorMsg = "";
    $scope.ZipCodeList = null;

    $scope.zipClick = function (ziparray) {
        $localStorage.location = ziparray;
        $('#userLocation').modal('hide');
    };

  

    $scope.getEventTitle = function (dt) {
        return eventDateService.eventDates[eventDateService.yyyymmdd(dt)];
    };
}]);