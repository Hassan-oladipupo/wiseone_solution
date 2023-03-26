app.controller('functionModalCtrl',
    function($scope, $filter, $modalInstance, functionToUpdate) {

        $scope.function = functionToUpdate;

        $scope.Modules = [{
            value: 'APPROVEREMAIL',
            label: 'APPROVER EMAIL'
        }, {
            value: 'FUNCTION',
            label: 'FUNCTION'
        }, {
            value: 'STAFF',
            label: 'STAFF'
        }, {
            value: 'ROLE',
            label: 'ROLE'
        }, {
            value: 'CLASSROOM',
            label: 'CLASSROOM'
        }, {
            value: 'LOCATION',
            label: 'LOCATION'
        }, {
            value: 'SHIFT',
            label: 'SHIFT'
        }, {
            value: 'LEAVE',
            label: 'LEAVE'
        }, {
            value: 'TASK',
            label: 'TASK'
        }, {
            value: 'CLOCKIN',
            label: 'CLOCKIN'
        }, {
            value: 'WISEONE MOBILE',
            label: 'WISONE MOBILE'
        }, {
            value: 'REPORT',
            label: 'REPORT'
        }, {
            value: 'BACKGROUND SERVICES',
            label: 'BACKGROUND SERVICES'
        }];

        var obj = $filter('filter')($scope.Modules, { 'label': $scope.function.Module });

        $scope.selectedModule = obj[0];

        $scope.ok = function() {
            $scope.function.Module = $scope.selectedModule.label;
            $modalInstance.close($scope.function);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

    }
);