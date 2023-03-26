angular.module('app')
    .config([
        '$ocLazyLoadProvider',
        function($ocLazyLoadProvider) {
            $ocLazyLoadProvider.config({
                debug: true,
                events: true,
                modules: [{
                        name: 'ui.select',
                        files: [
                            'lib/modules/angular-ui-select/select.css',
                            'lib/modules/angular-ui-select/select.js'
                        ]
                    },
                    {
                        name: 'daterangepicker',
                        serie: true,
                        files: [
                            'lib/modules/angular-daterangepicker/moment.js',
                            'lib/modules/angular-daterangepicker/daterangepicker.js',
                            'lib/modules/angular-daterangepicker/angular-daterangepicker.js'
                        ]
                    },
                    {
                        name: 'vr.directives.slider',
                        files: [
                            'lib/modules/angular-slider/angular-slider.min.js'
                        ]
                    },
                    {
                        name: 'textAngular',
                        files: [
                            'lib/modules/text-angular/textAngular-sanitize.min.js',
                            'lib/modules/text-angular/textAngular-rangy.min.js',
                            'lib/modules/text-angular/textAngular.min.js'
                        ]
                    }
                ]
            });
        }
    ]);