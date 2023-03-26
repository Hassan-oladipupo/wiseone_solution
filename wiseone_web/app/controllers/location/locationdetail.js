'use strict';

app.controller('LocationDetailCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.location = $sessionStorage.transferData.data;
    $scope.edit = $sessionStorage.transferData.edit;

    $scope.save = function() {

        $scope.loading = true;

        configFileService
            .put(configFileService.apiHandler.updateLocation, $scope.location)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Location Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Location Management', data);
                $scope.loading = false;
            });
    };

    $scope.goBack = function() {
        $window.history.back();
    };

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });
});