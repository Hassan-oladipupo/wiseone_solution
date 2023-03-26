app.controller('ApprovalModalCtrl',
    function($scope, $filter, $modalInstance, staffDto) {

        $scope.staff = staffDto;

        $scope.approve = function() {
            $scope.staff.update = true;
            $modalInstance.close($scope.staff);
        };

        $scope.decline = function() {
            $scope.staff.update = false;
            $modalInstance.close($scope.staff);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

    }
);