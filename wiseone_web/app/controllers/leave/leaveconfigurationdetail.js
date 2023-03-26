'use strict';

app.controller('LeaveDetailCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.financialYear = $sessionStorage.transferData.data;
    $scope.financialYear.selectedMonth = $scope.financialYear.Months[0];
    $scope.edit = $sessionStorage.transferData.edit;
    $scope.bankHolidays = [];

    $scope.init = function() {
        _.forEach($scope.financialYear.Months, function(m, mkey) {
            _.forEach(m.Days, function(d, dkey) {
                if (d.BankHoliday) {
                    var holiday = d.Name + ' ' + d.Day + ', ' + m.Month + ' ' + m.Year;
                    $scope.bankHolidays.push(holiday);
                }
            });
        });
    }

    $scope.save = function() {

        $scope.loading = true;

        configFileService
            .put(configFileService.apiHandler.updateFinancialYear, $scope.financialYear)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Leave Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
                $scope.loading = false;
            });

    };

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

});