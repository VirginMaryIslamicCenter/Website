angular.module('app').controller('calendarCtrl', ['$scope', 'eventDateService', function ($scope, eventDateService) {
    $scope.day = moment();

    $scope.getEventTitle = function (dt) {
        return eventDateService.eventDates[eventDateService.yyyymmdd(dt)];
    };
}]);