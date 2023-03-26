'use strict';

app.controller('StaffLeaveDetailsCtrl', function($sessionStorage, $scope, $window, configFileService, confirmService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.staff = $sessionStorage.transferData.data;
    $scope.financialYears = [];
    $scope.financialYear = {};
    $scope.preSelectedFinancialYear = undefined;
    $scope.financialYearExists = false;
    $scope.financialYearFetched = false;
    $scope.leaveDetailsFetched = false;
    $scope.info = `Loading leave calendar for ${$scope.staff.Location.Name}`;
    $scope.leaveDetailsInfo = `Loading staff leave details`;
    $scope.staffLeave = {};

    $scope.init = function() {
        $scope.getLocationFinancialYear();
    }

    $scope.getLocationFinancialYear = function() {

        var uri = configFileService.apiHandler.retrieveLocationFinancialYears + '?locationId=' + $scope.staff.Location.ID;
        configFileService
            .get(uri)
            .success(function(data, status, headers) {

                $scope.financialYearFetched = true;
                $scope.financialYears = data.data;

                if (_.size($scope.financialYears) > 0) {

                    _.forEach($scope.financialYears, function(financialYear) {
                        financialYear.RawStartDate = moment(financialYear.StartDate, ['MM/DD/YYYY', 'DD/MM/YYYY']);
                    });
                    $scope.financialYears = _.orderBy($scope.financialYears, ['RawStartDate'], ['asc']);

                    if ($scope.preSelectedFinancialYear) {
                        $scope.financialYear = _.find($scope.financialYears, function(year) {
                            return _.isEqual(year.ID, $scope.preSelectedFinancialYear.ID);
                        });
                    } else {
                        $scope.financialYear = _.find($scope.financialYears, function(year) {
                            return _.isEqual(year.Status, 'Opened');
                        });
                    }

                    $scope.preSelectedFinancialYear = $scope.financialYear;
                } else {
                    $scope.financialYear = undefined;
                }

                if (!$scope.financialYear) {
                    $scope.financialYearExists = false;
                    $scope.info = `No leave calender found for location: ${$scope.staff.Location.Name}.`;
                } else {
                    $scope.financialYearExists = true;
                    $scope.getStaffLeaves($scope.financialYear);
                }
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
                $scope.loading = false;
            });

    };

    $scope.getStaffLeaves = function(financialYear) {

        $scope.leaveDetailsFetched = false;
        $scope.financialYear = financialYear;
        $scope.preSelectedFinancialYear = financialYear;

        configFileService
            .get(`${configFileService.apiHandler.retrieveStaffLeavesByFinancialYear}?staffID=${$scope.staff.ID}&locationID=${$scope.staff.Location.ID}&financialYearID=${$scope.financialYear.ID}`)
            .success(function(data, status, headers) {
                $scope.leaveDetailsFetched = true;
                $scope.staffLeave = data;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
            });

    };

    $scope.deleteLeave = function(leave) {

        var modalOptions = {
            headerText: 'Cancel Leave',
            bodyText: `Are you sure you want to cancel the selected leave?`
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $scope.loading = true;
            configFileService
                .put(configFileService.apiHandler.deleteLeaveRequest, leave)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Staff Management', data);
                    $scope.loading = false;
                    reset();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Staff Management', data);
                    $scope.loading = false;
                });
        });
    };

    var reset = function() {

        $scope.financialYear = {};
        $scope.financialYearExists = false;
        $scope.financialYearFetched = false;
        $scope.leaveDetailsFetched = false;
        $scope.info = `Loading staff leave details`;
        $scope.staffLeave = {};

        $scope.init();
    }

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

});