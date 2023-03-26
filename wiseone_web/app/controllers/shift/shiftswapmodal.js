app.controller('ShiftSwapModalCtrl',
    function($scope, $filter, $modalInstance, shiftSwapDto) {

        $scope.shiftSwap = shiftSwapDto;
        $scope.enterDeclineReason = $scope.shiftSwap.Status == 'Declined' ? true : false;
        $scope.approvalType = $scope.shiftSwap.Status == 'Pending' ? '' : $scope.shiftSwap.Status == 'Approved' ? 'Approve' : 'Decline';

        $scope.setApproval = function() {
            if ($scope.approvalType == 'Decline') {
                $scope.enterDeclineReason = true;
            } else {
                $scope.enterDeclineReason = false;
            }
        }

        $scope.approve = function() {
            $scope.shiftSwap.update = true;
            $modalInstance.close($scope.shiftSwap);
        }

        $scope.decline = function() {
            $scope.shiftSwap.update = false;
            $modalInstance.close($scope.shiftSwap);
        }

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        }

    }
);