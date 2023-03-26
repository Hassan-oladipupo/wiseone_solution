'use strict';

app.controller('CreateLeaveCtrl', function($sessionStorage, $scope, $window, configFileService, confirmService, WizardHandler) {

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
    $scope.selectedMonth = {};
    $scope.selectedDays = [];
    $scope.orderedDays = [];
    $scope.leaveType = 'full';
    $scope.typeOfLeave = {
        selectedLeaveType: {}
    };
    $scope.info = `Loading leave calendar for ${$scope.staff.Location.Name}`;
    $scope.leaveDetailsInfo = `Loading staff leave details`;
    $scope.staffLeave = {};

    $scope.setLeaveType = function(leaveType) {
        $scope.leaveType = leaveType;
    }

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

        var month = moment().format('MMMM');
        var monthIdx = _.findIndex($scope.financialYear.Months, { Month: month });
        $scope.selectedMonth = $scope.financialYear.Months[monthIdx];
        $scope.typeOfLeave.selectedLeaveType = $scope.financialYear.LeaveTypes[0];

        configFileService
            .get(`${configFileService.apiHandler.retrieveStaffLeavesByFinancialYear}?staffID=${$scope.staff.ID}&locationID=${$scope.staff.Location.ID}&financialYearID=${$scope.financialYear.ID}`)
            .success(function(data, status, headers) {
                $scope.leaveDetailsFetched = true;
                $scope.staffLeave = data;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Staff Management', data);
            });

    };

    $scope.save = function() {

        var leaveIsDeductible = $scope.typeOfLeave.selectedLeaveType.Deductible;
        var numberofDeductibleLeaveDays = $scope.staffLeave.NumberOfLeaveRemaining;
        var daysSelected = _.size($scope.selectedDays);
        var daysTaken = $scope.leaveType == 'full' ? daysSelected : parseFloat(daysSelected - 1) + 0.5;

        if (leaveIsDeductible && daysTaken > numberofDeductibleLeaveDays) {

            configFileService.displayMessage('info', 'Staff Management', `Leave request not allowed! Number of selected days for leave is more than staff's available deductible leave days.`);

        } else {

            var modalOptions = {
                headerText: 'Create Leave',
                bodyText: `Are you sure you want to create leave for ${$scope.staff.Name}?`
            };

            confirmService.showModal({}, modalOptions).then(function() {

                var leaveDayDescription = "";
                if (daysSelected == 1) {
                    leaveDayDescription = $scope.leaveType == 'full' ? 'A full day' : $scope.leaveType == 'part' ? 'A half day AM' : 'A half day PM';
                } else {
                    leaveDayDescription = $scope.leaveType == 'full' ? daysSelected + ' full days' : (daysSelected - 1) + ' and half days';
                }

                var leaveRequest = {
                    RequestedDays: $scope.selectedDays,
                    StaffID: $scope.staff.ID,
                    StaffUsername: $scope.staff.Username,
                    StaffEmail: $scope.staff.Email,
                    StaffTelephone: $scope.staff.Telephone,
                    StaffName: $scope.staff.FirstName + ' ' + $scope.staff.Surname,
                    StaffKnownAs: $scope.staff.KnownAs,
                    StaffLocation: $scope.staff.Location,
                    LeaveDaysTaken: daysTaken,
                    LeaveDayDescription: leaveDayDescription,
                    LeaveType: $scope.typeOfLeave.selectedLeaveType.Type,
                    FinancialYear: $scope.financialYear
                };

                $scope.loading = true;
                configFileService
                    .put(configFileService.apiHandler.saveLeaveRequest, leaveRequest)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Staff Management', data);
                        $scope.loading = false;
                        resetWizard();
                    })
                    .error(function(data, status, headers) {
                        configFileService.displayMessage('info', 'Staff Management', data);
                        $scope.loading = false;
                    });

            });
        }
    };

    var resetWizard = function() {

        $scope.financialYear = {};
        $scope.financialYearExists = false;
        $scope.financialYearFetched = false;
        $scope.leaveDetailsFetched = false;
        $scope.selectedMonth = {};
        $scope.selectedDays = [];
        $scope.leaveType = 'full';
        $scope.typeOfLeave = {
            selectedLeaveType: {}
        };
        $scope.info = `Loading staff leave details and leave calendar for ${$scope.staff.Location.Name}`;
        $scope.staffLeave = {};

        $scope.init();

        var wizard = WizardHandler.wizard();
        if (wizard) {
            wizard.goTo(0);
        }
    }

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

});