app.controller('AddLocationCtrl', function($scope, configFileService) {

    $scope.location = new locationdto();

    $scope.save = function() {

        $scope.loading = true;

        configFileService
            .post(configFileService.apiHandler.saveLocation, $scope.location)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Location Management', data);
                $scope.loading = false;
                $scope.reset();
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Location Management', data);
                $scope.loading = false;
            });
    };

    $scope.reset = function() {
        $scope.location = new locationdto();
    };

});