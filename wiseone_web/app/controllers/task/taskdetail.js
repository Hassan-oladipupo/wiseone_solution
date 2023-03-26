app.controller('TaskDetailCtrl', function($sessionStorage, $scope, $window, configFileService, WizardHandler) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.task = $sessionStorage.transferData.data;

    $scope.selectedStaff = [];
    $scope.taskUpdates = [];
    $scope.dateOfCompletion = '';

    $scope.init = function() {

        $scope.dateOfCompletion = (moment($scope.task.DateofCompletionStr, "DD/MM/YYYY")).format('dddd, MMM Do YYYY');
        console.log($scope.dateOfCompletion);

        $scope.disabled = true;

        configFileService
            .get(configFileService.apiHandler.retrieveTaskStaff + '?taskID=' + $scope.task.ID)
            .success(function(data, status, headers) {
                $scope.selectedStaff = [];
                _.forEach(data, function(taskStaff) {
                    taskStaff.Staff.TaskLeader = taskStaff.TaskLeader;
                    $scope.selectedStaff.push(taskStaff.Staff);
                });
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
            });

        configFileService
            .get(configFileService.apiHandler.retrieveTaskUpdates + '?taskID=' + $scope.task.ID)
            .success(function(data, status, headers) {
                $scope.taskUpdates = data;
                $scope.disabled = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
            });

    };

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

});