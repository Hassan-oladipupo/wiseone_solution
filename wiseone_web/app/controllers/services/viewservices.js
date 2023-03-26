app.controller('ServicesCtrl', function($scope, $interval, configFileService, $timeout) {

    $scope.Services = [];
    $scope.promise = null;

    $scope.getServices = function() {

        if ($scope.promise) {
            $interval.cancel($scope.promise);
        }

        services();
        $scope.promise = $interval(services, 60000);

    };

    var services = function() {

        $scope.disabled = true;
        configFileService
            .get(configFileService.apiHandler.retrieveServices)
            .success(function(data, status, headers) {
                $scope.Services = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Background Services', data);
            });
    };

    $scope.$on('$destroy', function() {
        $interval.cancel($scope.promise);
    });

});