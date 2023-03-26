app.controller('ConfigureModalCtrl',
    function($scope, $filter, $modalInstance, approverConfigurationDto) {

        $scope.approverEmail = approverConfigurationDto.configuration;
        $scope.newConfiguration = approverConfigurationDto.newConfiguration;
        $scope.requestingRoles = angular.copy(approverConfigurationDto.roles);
        $scope.approvingRoles = angular.copy(approverConfigurationDto.roles);
        $scope.approvingLocationTypes = getApprovingLocationTypes();
        $scope.selectedApprovingLocation = "";
        $scope.selectedApprovingRole = "";

        $scope.addConfiguration = function() {
            var roleExists = _.find($scope.approverEmail.ApprovingRoles, function(r, rkey) {
                return _.isEqual(r.ApprovingRole.ID, $scope.selectedApprovingRole.ID);
            });
            if (!roleExists) {

                $scope.approverEmail.ApprovingRoles.push({
                    ApprovingLocation: $scope.selectedApprovingLocation,
                    ApprovingRole: $scope.selectedApprovingRole
                });
            }
        };

        $scope.deleteConfiguration = function(approvingRole) {
            _.remove($scope.approverEmail.ApprovingRoles, function(r) {
                return _.isEqual(r.ApprovingRole.ID, approvingRole.ID);
            });
        };

        $scope.save = function() {
            $modalInstance.close($scope.approverEmail);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

        function getApprovingLocationTypes() {
            return [
                { Type: 'RequestingStaffLocation', Name: "REQUESTING STAFF's OFFICE" },
                { Type: 'HeadOfficeLocation', Name: "HEAD OFFICE" },
                { Type: 'OperationOfficeLocation', Name: "OPERATIONS DIRECTOR's OFFICE" },
            ];
        };
    }
);