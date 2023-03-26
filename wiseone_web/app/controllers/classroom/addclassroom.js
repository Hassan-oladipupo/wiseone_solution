app.controller('AddClassRoomCtrl', function($scope, configFileService) {

    $scope.classRoom = new classroomdto();

    $scope.init = function() {

        $scope.disabled = true;

        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Room Management', data);
            });

    };

    $scope.save = function() {

        $scope.loading = true;

        configFileService
            .post(configFileService.apiHandler.saveClassRoom, $scope.classRoom)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Room Management', data);
                $scope.loading = false;
                $scope.reset();
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Room Management', data);
                $scope.loading = false;
            });

    };

    $scope.reset = function() {
        $scope.classRoom = new classroomdto();
    };

});