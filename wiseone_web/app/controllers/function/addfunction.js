app.controller('AddFunctionCtrl', function($scope, configFileService) {

    $scope.function = new functiondto();

    $scope.selectedModule = '';

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


    $scope.save = function() {

        $scope.loading = true;
        $scope.function.Module = $scope.selectedModule.label;

        configFileService
            .post(configFileService.apiHandler.saveFunction, $scope.function)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Function Management', data);
                $scope.loading = false;
                $scope.reset();
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Function Management', data);
                $scope.loading = false;
            });
    };

    $scope.reset = function() {
        $scope.function = new functiondto();
        $scope.selectedModule = '';
    };

});