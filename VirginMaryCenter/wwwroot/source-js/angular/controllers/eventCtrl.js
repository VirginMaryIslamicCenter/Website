angular.module('app').controller('eventCtrl', ['$scope', '$window', '$http', '$sce', 'eventDateService', function ($scope, $window, $http, $sce, eventDateService) {
    $scope.loading = true;
    $scope.tmp = "";
    $scope.tmpjson = "";

    $scope.tmp = new Date();

    var getNextSaturday = function () {
        var dt = new Date();

        while (dt.getDay() !== 6) {
            dt.setDate(dt.getDate() + 1);
        }

        return dt;
    };

    $scope.eventPeriods =
        {
            "live": {
                name: "LIVE EVENTS",
                show: false,
                data: null,
                limitTo: 3
            },
            "upcoming": {
                name: "UPCOMING EVENTS",
                show: true,
                data: [],
                limitTo: 3
            },
            "past": {
                name: "PAST EVENTS",
                show: false,
                data: null,
                limitTo: 3
            }
        };

    $scope.getEventBackground = function (d) {
        return new Date(d).getHours() > 16 ? "night" : "day";
    };

    $scope.getEventBackgroundIcon = function (d) {
        return new Date(d).getHours() > 16 ? "fa-moon" : "fa-sun";
    };

    $scope.filterDesc = function (desc) {
        //desc = desc.replace("In the name of Allah, the Compassionate the Merciful", "");
        //desc = desc.replace("In the name of Allah, the Compassionate, the Merciful", "");
        //desc = desc.replace("Assalamu Aleykum", "");
        //desc = desc.trim();
        return desc;
    };

    $scope.getGoogleMapDirection = function (address, city, state) {
        return "https://www.google.com/maps/dir/?api=1&origin=" + encodeURI(address + " " + city + ", " + state);
    };

    var formatAMPM = function (date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    };

    $scope.sameDate = function (sT, eT) {
        var startTime = new Date(sT);
        var endTime = new Date(eT);

        if (startTime.getDate() === endTime.getDate()) {
            return true;
        }
        else
            return false;
    };
    

    $scope.eventView = function () {
        $scope.loading = true;

        $http({
            method: 'GET',
            url: "/api/events",
            params: {
            }
        }).then(function successCallback(response) {

            var eventsAll = JSON.parse(response.data).data;

            eventDateService.eventDates = [];
            for (var x = 0; x < eventsAll.length; x++) {
                //console.log(eventsAll[x].start_time);
                eventDateService.eventDates[eventDateService.yyyymmdd(eventsAll[x].start_time)] = eventsAll[x].name;
            }

            $scope.eventPeriods["live"].data = eventsAll.filter(e => (new Date(e.start_time).getTime() <= new Date().getTime() && new Date(e.end_time).getTime() >= new Date().getTime()));
            $scope.eventPeriods["live"].show = $scope.eventPeriods["live"].data.length > 0 ? true : false;

            var upcomingFilter = (eventsAll.filter(e => new Date(e.start_time).getTime() >= new Date().getTime()))
            if (upcomingFilter && upcomingFilter.length > 0) {
                $scope.eventPeriods["upcoming"].data = upcomingFilter;
            }
            else if (!$scope.eventPeriods["live"].data || $scope.eventPeriods["live"].data.length == 0) {
             
                $scope.eventPeriods["upcoming"].data = [{
                    default: true,

                    cover: {
                        source: "/images/background-blur-5.jpg"
                    },
                    is_draft: false,
                    name: "Event details TBD",
                    description: "We will make an announcement soon. Please check back later",
                    start_time: getNextSaturday(),
                    end_time: getNextSaturday()
                }];

                $scope.eventPeriods["upcoming"].show = true;
            }
            else {
                $scope.eventPeriods["upcoming"].data = null;
                $scope.eventPeriods["upcoming"].show = false;
            }
            console.log(new Date())
            $scope.eventPeriods["past"].data = eventsAll.filter(e => new Date(e.end_time).getTime() <= new Date().getTime());
            $scope.eventPeriods["past"].show = $scope.eventPeriods["past"].data.length > 0 ? true : false;

            console.log($scope.eventPeriods);
            $scope.loading = false;

            eventDateService.Refresh();


        }, function errorCallback(response) {
            $scope.hasError = true;
        });
    };


    $scope.eventView();

}]);