'use strict';

app
    .controller('ForgotPasswordCtrl', function($sessionStorage, $scope, $state, configFileService, Idle) {

        Idle.unwatch();

        $scope.username = '';

        $scope.retrieve = function() {

            $scope.loading = true;

            $scope.Password = {
                Username: $scope.username
            };

            configFileService
                .put(configFileService.apiHandler.forgotPassword, $scope.Password)
                .success(function(response, status, headers) {

                    $scope.username = '';

                    $scope.loading = false;

                    var message = "An email that contains a link to continue with your password reset has been sent to your email: " + response + ". If this email address is not correct, kindly contact your administrator to modify accordingly.";

                    configFileService.displayMessage('info', 'Password Management', message);

                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Password Management', data);
                    $scope.loading = false;
                });
        };

    });