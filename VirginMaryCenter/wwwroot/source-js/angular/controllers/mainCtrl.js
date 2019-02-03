angular.module('app').controller('mainCtrl', ['$scope', '$window', '$http', '$sce', function ($scope, $window, $http, $sce) {

    $scope.serverName = $window.serverName;
    
    $scope.dashboard = {
        "Total": "-",
        "Transcribing": "-",
        "Transcribed": "-",
        "TranscribePending": "-",
        "Failed": "-",
        "pdfList": []
        }; 


    $scope.loading = '';
    $scope.searchGuid = "";
    $scope.searchFilename = "";
    $scope.searchMRN = "";
    $scope.searchStatus = "";

    $scope.timeframe = 'Last24Hours';
    $scope.department = 'All';

    $scope.selectedGUID = "";
    $scope.trust = $sce.trustAsHtml;

    $scope.Highlight = function (str, search) {
        if (!search) {
            return str;
        }
        else {
            var s = str.toLowerCase().indexOf(search.toLowerCase());
            if (s != -1) {
                str = (s == 0 ? "" : str.substr(0, s)) + "<span class=\"Highlight\">" + str.substr(s, search.length) + "</span>" + str.substr(s + search.length);
            }
            return str;
        }
    }
    
    var filterView = function(downloadCSV) {
        //$scope.dashboard = null;
        var daysBefore = 0;
        switch ($scope.timeframe) {
            case 'Last24Hours':
                daysBefore = 1; break;
            case 'Last7Days':
                daysBefore = 7; break;
            case 'Last30Days':
                daysBefore = 30; break;
            case 'Last60Days':
                daysBefore = 60; break;
            case 'Last90Days':
                daysBefore = 90; break;
            case 'Last1Year':
                daysBefore = 365; break;
            case 'All':
                daysBefore = 365 * 10; break;
        }

        var url;
        if (downloadCSV)
            url = '/api/OCRDB/Dashboard/$RegressionCSV';
        else
            url = "/api/OCRDB/Dashboard/$View"

        $http({
            method: 'GET',
            url: url,
            /*headers: { 'Content-Type': 'application/x-www-form-urlencoded' },*/
            params: {
                'Department': $scope.department == 'All' ? "" : $scope.department,
                'DaysBefore': daysBefore,
                'searchGuid': $scope.searchGuid,
                'searchFilename': $scope.searchFilename,
                'searchMRN': $scope.searchMRN,
                'searchStatus': $scope.searchStatus

            }
        }).then(function successCallback(response) {
            if (downloadCSV) {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!

                var yyyy = today.getFullYear();
                if (dd < 10) {
                    dd = '0' + dd;
                }
                if (mm < 10) {
                    mm = '0' + mm;
                }
                var filename = mm + dd + yyyy + "-" + $scope.department + "-Last" + daysBefore + "Days-" + $scope.searchMRN + ".csv";
                
                var anchor = angular.element('<a/>');
                anchor.attr({
                    href: 'data:attachment/csv;charset=utf-8,' + encodeURI(response.data),
                    target: '_blank',
                    download: filename
                })[0].click();

            }
            else {
                $scope.dashboard = response.data;
            }
            $scope.loading = false;


        }, function errorCallback(response) {
            $scope.hasError = true;
            console.log(response);
            $scope.dashboard = null;
        });
    };


    //Calls the server to delete all the PDFs that have TEST-PAC as the department
    $scope.deleteTestPac = function () {
        $http({
            method: 'DELETE',
            url: '/api/OCRDB/Dashboard/$DeleteTestPac',
            params: { 'DoIt': "yes"}
        }).then(function successCallback(response) {
            $scope.RefreshView();

            }, function errorCallback(response) {
                $scope.hasError = true;
                $scope.dashboard = null;
                console.log(response);
        })



    };

    $scope.downloadCSV = function () { filterView(true) };
    $scope.RefreshView = function () {
        $scope.loading = true;
        filterView(false);
    };

    $scope.RefreshView();
    $scope.overlayVisible = false;

    $scope.showOverlay = function (obj, type) {
        $scope.selectedGUID = obj.guid;
        $scope.overlayVisible = true;
        $scope.itemInfo = obj;
        $scope.itemJson = 'Loading ...';
        $scope.itemLog = 'Loading ...';
        $scope.itemMatched = 'Loading ...';
        $scope.itemPDF = '';
        $scope.itemMatchedPages = '';
        if (type == 'json')
            $scope.getItemJson(obj.guid);
        else if (type == 'matched')
            $scope.getItemMatched(obj.guid);
    };

    var getItem = function (url, guid) {
        var result;
        return $http({
            method: 'GET', 
            url: url,
            /*headers: { 'Content-Type': 'application/x-www-form-urlencoded' },*/
            params: { 'PdfId': guid }
        }).then(function successCallback(response) {
            return response.data;

        }, function errorCallback(response) {

        });
        
    }

    $scope.getItemJson = function (guid) {
        $scope.overlayNav = 'json';
        if ($scope.itemJson == 'Loading ...')
            getItem('/api/OCRDB/$OCRResult', guid).then(function (result) {
                if (!result && $scope.itemInfo.Status == "PROCESSED")
                    $scope.itemJson = "\nStrange. No Json was produced even though this file was processed.";
                else if (!result && $scope.itemInfo.Status == "UPLOADED") {
                    $scope.itemJson = "\nPDF has not gone through OCR to have any json output yet.";
                }
                else if (!result && $scope.itemInfo.Status == "PROCESSING") {
                    $scope.itemJson = "\nPDF is still being processed. Json not yet available.";
                }
                else
                    $scope.itemJson = result;
            });
    }
    $scope.getItemMatched = function (guid) {
        $scope.overlayNav = 'matched';
        if ($scope.itemMatched == 'Loading ...')
            getItem('/api/OCRDB/$OCRMatchedNoMatch', guid).then(function (result) {
                $scope.itemMatched = result;
            });
    }

    $scope.getItemLog = function (guid) {
        $scope.overlayNav = 'log';
        if ($scope.itemLog == 'Loading ...')
            getItem('/api/OCRDB/$OCRLog', guid).then(function (result) {
                console.log(!result);
                console.log(result);
                if (!result && $scope.itemInfo.Status == "PROCESSED")
                    $scope.itemLog = "\nLogs are only saved if the OCR Service has errors or failures.";
                else if (!result && $scope.itemInfo.Status == "UPLOADED") {
                    $scope.itemLog = "\nPDF has not gone through OCR to have any logs yet";
                }
                else if (!result && $scope.itemInfo.Status == "PROCESSING") {
                    $scope.itemLog = "\nPDF is still being processed. Log file not yet available.";
                }
                else
                    $scope.itemLog = result;
            });
    }

    $scope.getItemDownloadPDF = function (guid) {
        return '/api/OCRDB/$OCRPDF?PdfId=' + guid;
    };

    $scope.getItemPDF = function (guid) {
        $scope.overlayNav = 'pdf';

        if ($scope.itemPDF == '') {
            $http({
                method: 'GET',
                url: '/api/OCRDB/$OCRPDF',
                /*headers: { 'Content-Type': 'application/x-www-form-urlencoded' },*/
                responseType: 'arraybuffer',
                params: { 'PdfId': guid }
            }).then(function successCallback(response) {
                var file = new Blob([response.data], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                var itemPDF = $sce.trustAsResourceUrl(fileURL);
                $scope.itemPDF = itemPDF;
                $scope.fileURL = itemPDF;

            }, function errorCallback(response) {

            })
        }
    }
    
    $scope.statusColor = function(status) {
        switch (status) {
            case 'UPLOADED': return '#393939;';
            case 'PROCESSING': return 'royalblue';
            case 'PROCESSED': return '#1ab147';
            case 'DELETED': return 'lightgray';
            case 'ARCHIVED': return 'lightgray';
            case 'FAILED': return '#ca2828';
        }
    };
    $scope.showStatus = function (status) {
        return status;
    };

    $scope.loadingComplete = true;

    $scope.JsonPretty = function (jsonObj) {
        if (!jsonObj)
            return "";

        var json = JSON.stringify(jsonObj, undefined, 4);
        json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
            var cls = 'number';
            if (/^"/.test(match)) {
                if (/:$/.test(match)) {
                    cls = 'key';
                } else {
                    cls = 'string';
                }
            } else if (/true|false/.test(match)) {
                cls = 'boolean';
            } else if (/null/.test(match)) {
                cls = 'null';
            }
            return '<span class="' + cls + '">' + match + '</span>';
        });
    }
}]);