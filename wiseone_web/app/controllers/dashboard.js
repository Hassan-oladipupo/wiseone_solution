'use strict';

app
    .controller('DashboardCtrl', function($rootScope, $sessionStorage, $scope, $state) {

        var loggedInUser = $sessionStorage.wiseOneUser;
        if (loggedInUser) {
            var modules = $rootScope.getElementsByAttribute('data-app-module');
            $rootScope.systemHandler.setUserMenu(loggedInUser.Function, modules);
        }
    });