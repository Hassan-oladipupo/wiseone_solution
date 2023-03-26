app.controller('CancelLeaveModalCtrl',
    function($scope, $filter, $modalInstance, leaveDto) {

        $scope.leave = leaveDto;

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

    }
);