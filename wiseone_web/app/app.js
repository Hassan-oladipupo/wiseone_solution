'use strict';
angular
    .module('app', [
        'mgo-angular-wizard',
        'ngMask',
        'ngAnimate',
        'ngCookies',
        'ngResource',
        'ngSanitize',
        'ui.select',
        'ngTouch',
        'ngStorage',
        'ngTagsInput',
        'ui.router',
        'ncy-angular-breadcrumb',
        'ui.bootstrap',
        'ngIdle',
        'ui.utils',
        'oc.lazyLoad',
        'angular-ladda',
        'daterangepicker',
        'toaster',
        'n3-line-chart',
        'n3-pie-chart',
        'nvd3'
    ])
    .config(['$httpProvider', function($httpProvider) {
        //initialize get if not there
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }

        //disable IE ajax request caching
        $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        // extra
        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    }]);