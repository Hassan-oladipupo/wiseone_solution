app.controller('CancelShiftModalCtrl',
    function($scope, $filter, $modalInstance, shiftDto) {

        $scope.shift = shiftDto;
        console.log($scope.shift.ApprovalStatus);
        $scope.enterDeclineReason = $scope.shift.ApprovalStatus == 'Declined' ? true : false;
        $scope.approvalType = $scope.shift.ApprovalStatus == 'Pending' ? '' : $scope.shift.ApprovalStatus == 'Approved' ? 'Approve' : 'Decline';

        $scope.setApproval = function() {
            if ($scope.approvalType == 'Decline') {
                $scope.enterDeclineReason = true;
            } else {
                $scope.enterDeclineReason = false;
            }
        }

        $scope.approve = function() {
            $scope.shift.update = true;
            $modalInstance.close($scope.shift);
        };

        $scope.decline = function() {
            $scope.shift.update = false;
            $modalInstance.close($scope.shift);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

    }
);