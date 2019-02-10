(function () {
    angular.module('app').factory('eventDateService', function () {


        function yyyymmdd(date) {
            return moment(date).format("YYYY-MM-DD");

          /*  var d = new Date(date),
                month = '' + (d.getMonth() + 1),
                day = '' + d.getDate(),
                year = d.getFullYear();

            if (month.length < 2) month = '0' + month;
            if (day.length < 2) day = '0' + day;

            return [year, month, day].join('-');*/

        }

        var eventDates = [];

        var Refresh = function () {

        };

        return {
            eventDates: eventDates,
            yyyymmdd: yyyymmdd,
            Refresh: Refresh
        };

    });

})();
