app.controller('ChangePasswordCtrl', function($scope, $sessionStorage, configFileService) {

    $scope.newPassword = '';
    $scope.confirmPassword = '';

    $scope.change = function() {

        if ($scope.confirmPassword != $scope.newPassword) {

            configFileService.displayMessage('info', 'Password Management', 'Password validation failed.');

        } else {

            $scope.loading = true;

            var password = {
                Password: $scope.newPassword,
                Username: $sessionStorage.wiseOneUser.Username
            };

            configFileService
                .put(configFileService.apiHandler.changePassword, password)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Password Management', data);
                    $scope.loading = false;
                    $scope.reset();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Password Management', data);
                    $scope.loading = false;
                });

        }
    };

    $scope.reset = function() {
        $scope.newPassword = '';
        $scope.confirmPassword = '';
    };

});