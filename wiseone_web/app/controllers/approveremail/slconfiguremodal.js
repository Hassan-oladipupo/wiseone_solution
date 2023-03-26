app.controller('SLConfigureModalCtrl',
    function($scope, $filter, $modalInstance, approverConfigurationDto) {

        $scope.approverEmail = approverConfigurationDto.configuration;
        $scope.approverRoles = angular.copy(approverConfigurationDto.roles);

        $scope.save = function() {
            $modalInstance.close($scope.approverEmail);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };
    }
);