'use strict';

app
    .controller('LoginCtrl', function($sessionStorage, $scope, $state, configFileService, Idle) {

        Idle.unwatch();

        $scope.iOS = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);

        $scope.password = {
            Username: '',
            Password: ''
        };

        $scope.login = function() {

            $scope.loading = true;

            configFileService
                .post(configFileService.apiHandler.login, $scope.password)
                .success(function(data, status, headers) {

                    $scope.loading = false;

                    if (!data.Role.CanAccessWeb) {
                        configFileService.displayMessage('info', 'Login Management', 'Kindly use the WISEONE Mobile');
                    } else if (data.Status == 'InActive') {
                        configFileService.displayMessage('info', 'Login Management', 'Your account is disabled. Contact your administrator');
                    } else {
                        $sessionStorage.wiseOneUser = data;
                        $state.go('app.dashboard');
                    }

                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Login Management', data);
                    $scope.loading = false;
                });
        };

    });