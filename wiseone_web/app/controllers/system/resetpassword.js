'use strict';

app
    .controller('ResetPasswordCtrl', function($location, $scope, $state, configFileService, Idle) {

        Idle.unwatch();

        $scope.newPassword = '';
        $scope.confirmPassword = '';

        $scope.init = function() {

            var passwordModel = {
                StaffId: $location.search().rq
            };

            configFileService
                .put(configFileService.apiHandler.confirmName, passwordModel)
                .success(function(data, status, headers) {
                    $scope.username = data.Username;
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Password Management', data);
                });

        }

        $scope.resetPassword = function() {

            if ($scope.confirmPassword != $scope.newPassword) {

                configFileService.displayMessage('info', 'Password Management', 'Password validation failed.');

            } else {

                $scope.loading = true;

                var password = {
                    Password: $scope.newPassword,
                    Username: $scope.username
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