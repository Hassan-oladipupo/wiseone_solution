'use strict';

app.controller('ClassRoomDetailCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.classRoom = $sessionStorage.transferData.data;
    $scope.edit = $sessionStorage.transferData.edit;

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

    }

    $scope.save = function() {

        $scope.loading = true;

        configFileService
            .put(configFileService.apiHandler.updateClassRoom, $scope.classRoom)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Room Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Room Management', data);
                $scope.loading = false;
            });

    };

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

});