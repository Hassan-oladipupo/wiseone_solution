app.controller('OverTimeModalCtrl',
    function($scope, $filter, $modalInstance, overTimeDto) {

        $scope.overTime = overTimeDto;

        $scope.approve = function() {
            $scope.overTime.update = true;
            $modalInstance.close($scope.overTime);
        };

        $scope.decline = function() {
            $scope.overTime.update = false;
            $modalInstance.close($scope.overTime);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

    }
);