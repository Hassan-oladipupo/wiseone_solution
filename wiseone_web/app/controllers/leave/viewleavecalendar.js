'use strict';

app.controller('ViewLeaveCalenderCtrl', function($sessionStorage, $scope, $state, configFileService) {

    var $calenderList = $('#calenderList');

    $scope.info = 'Loading locations';
    $scope.financialYearInfo = '';
    $scope.fetched = false;
    $scope.financialYearsFetched = false;
    $scope.Locations = [];
    $scope.financialYears = [];
    $scope.financialYear = {};
    $scope.selectedLocation = "";
    $scope.leaveCalendarExists = false;
    $scope.bankHolidays = [];
    $scope.date = {};

    $scope.init = function() {
        $scope.date = moment().format('LLLL');
        $scope.getLocations();
    };

    $scope.getLocations = function() {
        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
                $scope.selectedLocation = $scope.Locations[0];
                $scope.fetched = true;

                $scope.getLocationFinancialYear();
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
            });
    };

    $scope.getLocationFinancialYear = function() {

        $scope.loading = true;
        $scope.financialYearInfo = `Loading financial year for ${$scope.selectedLocation.Name}`;

        var uri = configFileService.apiHandler.retrieveLocationFinancialYears + '?locationId=' + $scope.selectedLocation.ID;
        configFileService
            .get(uri)
            .success(function(data, status, headers) {

                $scope.loading = false;
                $scope.financialYears = data.data;

                if (_.size($scope.financialYears) > 0) {
                    $scope.financialYearsFetched = true;
                    $scope.financialYear = _.find($scope.financialYears, function(year) {
                        return _.isEqual(year.Status, 'Opened');
                    });

                    $scope.getLocationCalendar();
                } else {
                    $scope.financialYearsFetched = false;
                    $scope.financialYear = undefined;
                    $scope.getLocationCalendar();
                    $scope.financialYearInfo = `No financial year found for ${$scope.selectedLocation.Name}`;
                }
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
                $scope.loading = false;
            });

    };

    $scope.getLocationCalendar = function() {

        $scope.bankHolidays = [];

        if ($scope.financialYear) {
            $scope.leaveCalendarExists = true;
            $scope.financialYear.selectedMonth = $scope.financialYear.Months[0];

            _.forEach($scope.financialYear.Months, function(m, mkey) {
                _.forEach(m.Days, function(d, dkey) {
                    if (d.BankHoliday) {
                        var holiday = d.Name + ' ' + d.Day + ', ' + m.Month + ' ' + m.Year;
                        $scope.bankHolidays.push(holiday);
                    }
                });
            });
        } else {
            $scope.leaveCalendarExists = false;
        }
    };

    $scope.export = function() {
        html2canvas(document.getElementById('exportthis')).then(function(canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                pageSize: 'Legal',
                content: [{
                    image: data,
                    width: 500,
                }]
            };
            pdfMake.createPdf(docDefinition).download(`${$scope.selectedLocation.Name} - ${$scope.financialYear.selectedMonth.Label} Leave Calendar.pdf`);
        });
    };
});