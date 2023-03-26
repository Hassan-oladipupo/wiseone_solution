app.controller('LeaveModalSecondLevelCtrl',
    function($scope, $filter, $modalInstance, leaveDto, configFileService) {

        $scope.financialYears = [];
        $scope.financialYear = {};
        $scope.financialYearFetched = false;
        $scope.leaveDetailsFetched = false;
        $scope.showDeclineReason = false;
        $scope.showDeclineReasonText = false;
        $scope.declineReason = '';
        $scope.info = `Loading leave calendar for ${leaveDto.LeaveRequest.StaffLocation.Name}`;


        $scope.leave = leaveDto;

        $scope.approvalType = $scope.leave.Approval.ApprovalStatus == 'Acknowledged' ? '' : $scope.leave.Approval.ApprovalStatus == 'Approved' ? 'Approve' : $scope.leave.Approval.ApprovalStatus == 'Acknowledged' ? 'Approve' : 'Decline';

        getLocationFinancialYear();

        $scope.setApproval = function(approvalType) {
            $scope.approvalType = approvalType;
            if (approvalType == 'Approve') {
                $scope.showDeclineReasonText = true;
                $scope.showDeclineReason = false;
                $scope.leave.LeaveRequest.DeclineReason = '';
            } else if (approvalType == 'Decline') {
                $scope.showDeclineReasonText = false;
                $scope.showDeclineReason = true;
                $scope.setDeclineReason('');
            }
        }

        $scope.setDeclineReason = function(declineReason) {
            $scope.declineReason = declineReason;
            if (declineReason == 'Other reasons') {
                $scope.showDeclineReasonText = true;
                $scope.leave.LeaveRequest.DeclineReason = '';
            } else {
                $scope.showDeclineReasonText = false;
                $scope.leave.LeaveRequest.DeclineReason = $scope.declineReason;
            }
        }

        $scope.approve = function() {
            $scope.leave.update = true;
            $modalInstance.close($scope.leave);
        };

        $scope.decline = function() {
            $scope.leave.update = false;
            $modalInstance.close($scope.leave);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

        function getLocationFinancialYear() {

            var uri = configFileService.apiHandler.retrieveLocationFinancialYears + '?locationId=' + $scope.leave.LeaveRequest.StaffLocation.ID;

            configFileService
                .get(uri)
                .success(function(data, status, headers) {

                    $scope.financialYearFetched = true;
                    $scope.financialYears = data.data;

                    if ($scope.leave.LeaveRequest.FinancialYear) {
                        $scope.financialYear = _.find($scope.financialYears, function(year) {
                            return _.isEqual(year.ID, $scope.leave.LeaveRequest.FinancialYear.ID);
                        });
                    } else {
                        $scope.financialYear = _.find($scope.financialYears, function(year) {
                            return _.isEqual(year.Status, 'Opened');
                        });
                    }

                    $scope.leave.FinancialYear = $scope.financialYear;

                    $scope.months = _.filter($scope.financialYear.Months, function(month, mkey) {
                        return _.find($scope.leave.LeaveRequest.RequestedDays, function(leave, lkey) {
                            if (leave.Month == month.Month && leave.Year == month.Year) {
                                return month;
                            }
                        });
                    });
                    $scope.selectedMonth = $scope.months[0];

                    getStaffLeaves();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Leave Management', data);
                    $scope.loading = false;
                });

        }

        function getStaffLeaves() {

            $scope.leaveDetailsFetched = false;

            configFileService
                .get(`${configFileService.apiHandler.retrieveStaffLeavesByFinancialYear}?staffID=${$scope.leave.LeaveRequest.StaffID}&locationID=${$scope.leave.LeaveRequest.StaffLocation.ID}&financialYearID=${$scope.financialYear.ID}`)
                .success(function(data, status, headers) {
                    $scope.leaveDetailsFetched = true;
                    $scope.staffLeave = data;

                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Staff Management', data);
                });

        }
    }
);