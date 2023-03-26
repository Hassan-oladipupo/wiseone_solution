'use strict';

app.controller('StaffDetailCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.staff = $sessionStorage.transferData.data;
    $scope.edit = $sessionStorage.transferData.edit;

    $scope.init = function() {

        $scope.disabled = true;

        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Staff Management', data);
            });

        configFileService
            .get(configFileService.apiHandler.retrieveActiveRoles)
            .success(function(data, status, headers) {
                $scope.Roles = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Staff Management', data);
            });

    }

    $scope.save = function() {

        if ($scope.staff.StartDate && !isValidDate($scope.staff.StartDate)) {
            configFileService.displayMessage('info', 'Staff Management', 'Invalid start date entered. Date format is DD/MM/YYYY');
            return
        }

        if ($scope.staff.EndDate && !isValidDate($scope.staff.EndDate)) {
            configFileService.displayMessage('info', 'Staff Management', 'Invalid end date entered. Date format is DD/MM/YYYY');
            return
        }

        $scope.loading = true;
        configFileService
            .put(configFileService.apiHandler.updateStaff, $scope.staff)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Staff Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Staff Management', data);
                $scope.loading = false;
            });

    };

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

    function isValidDate(testdate) {
        var isValid = false;
        var date_regex = /^\d{2}\/\d{2}\/\d{4}$/;
        let validDate = date_regex.test(testdate);
        if (validDate) {
            let testDates = testdate.split('/');
            let day = parseInt(testDates[0]);
            let month = parseInt(testDates[1]);
            if (day <= 31 && month <= 12) {
                isValid = true;
            }
        }
        return isValid;
    }

});