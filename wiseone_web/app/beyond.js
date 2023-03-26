angular.module('app')
    .controller('AppCtrl', [
        '$rootScope', '$localStorage', '$state', '$timeout', '$sessionStorage', '$modal', 'Idle',
        function($rootScope, $localStorage, $state, $timeout, $sessionStorage, $modal, Idle) {
            $rootScope.settings = {
                skin: '',
                color: {
                    themeprimary: '#0622a1',
                    themesecondary: '#ae003b',
                    themethirdcolor: '#ffce55',
                    themefourthcolor: '#a0d468',
                    themefifthcolor: '#e75b8d'
                },
                rtl: false,
                fixed: {
                    navbar: false,
                    sidebar: false,
                    breadcrumbs: false,
                    header: false
                }
            };
            if (angular.isDefined($localStorage.settings))
                $rootScope.settings = $localStorage.settings;
            else
                $localStorage.settings = $rootScope.settings;

            $rootScope.getElementsByAttribute = function(attribute) {

                var matchingElements = [];
                var allElements = document.getElementsByTagName('*');
                for (var i = 0, n = allElements.length; i < n; i++) {
                    if (allElements[i].getAttribute(attribute) !== null) {
                        // Element exists with attribute. Add to array.
                        matchingElements.push(allElements[i]);
                    }
                }
                return matchingElements;

            };

            $rootScope.systemHandler = {
                authorizeFunction: function(module, _function) {
                    var user = JSON.parse(window.sessionStorage.getItem("wiseOneUser"));
                    var functions = _.filter(user.Function, function(userFunction) {
                        return _.isEqual(module, userFunction.Module);
                    });
                    if (_.includes(_.map(functions, 'Type'), _function)) {
                        return true;
                    } else {
                        return false;
                    }
                },
                authorize: function(userFunctions, module) {
                    var moduleExists = _.find(userFunctions, function(userFunction) {
                        return _.isEqual(module, userFunction.Module);
                    });
                    if (!moduleExists) {
                        return false;
                    } else {
                        return true;
                    }
                },
                permitUser: function(module) {
                    var user = JSON.parse(window.sessionStorage.getItem("wiseOneUser"));
                    return utilities.systemHandler.authorize(user.Function, module);
                },
                setUserMenu: function(userFunctions, menus) {

                    $.each(menus, function(key, menu) {

                        var module = $(menu).attr("data-app-module");

                        var moduleExists = $rootScope.systemHandler.authorize(userFunctions, module);
                        if (!moduleExists) {
                            $(menu).hide();
                        }

                        //---Manage Sub Links
                        var subModules = [];
                        $.each(userFunctions, function() {
                            var userFunction = this;
                            if (_.isEqual(userFunction.Module, module)) {
                                subModules.push(userFunction.Type);
                            }
                        });

                        var subMenus = $(menu).find('.submodule');
                        $.each(subMenus, function() {
                            var subModule = $(this).attr("data-app-submodule");
                            if (!_.includes(subModules, subModule)) {
                                $(this).hide();
                            }
                        });
                    });
                },
                manageIdleness: function() {
                    closeModals();
                    Idle.watch();
                }
            };

            $rootScope.$watch('settings', function() {
                if ($rootScope.settings.fixed.header) {
                    $rootScope.settings.fixed.navbar = true;
                    $rootScope.settings.fixed.sidebar = true;
                    $rootScope.settings.fixed.breadcrumbs = true;
                }
                if ($rootScope.settings.fixed.breadcrumbs) {
                    $rootScope.settings.fixed.navbar = true;
                    $rootScope.settings.fixed.sidebar = true;
                }
                if ($rootScope.settings.fixed.sidebar) {
                    $rootScope.settings.fixed.navbar = true;


                    //Slim Scrolling for Sidebar Menu in fix state
                    var position = $rootScope.settings.rtl ? 'right' : 'left';
                    if (!$('.page-sidebar').hasClass('menu-compact')) {
                        $('.sidebar-menu').slimscroll({
                            position: position,
                            size: '3px',
                            color: $rootScope.settings.color.themeprimary,
                            height: $(window).height() - 90,
                        });
                    }
                } else {
                    if ($(".sidebar-menu").closest("div").hasClass("slimScrollDiv")) {
                        $(".sidebar-menu").slimScroll({ destroy: true });
                        $(".sidebar-menu").attr('style', '');
                    }
                }

                $localStorage.settings = $rootScope.settings;
            }, true);

            $rootScope.$watch('settings.rtl', function() {
                if ($state.current.name != "persian.dashboard" && $state.current.name != "arabic.dashboard") {
                    switchClasses("pull-right", "pull-left");
                    switchClasses("databox-right", "databox-left");
                    switchClasses("item-right", "item-left");
                }

                $localStorage.settings = $rootScope.settings;
            }, true);

            $rootScope.$on('$viewContentLoaded', function(event) {
                var ignoredStates = ['login', 'forgotpassword', 'resetpassword'];
                if (!_.isEmpty($state.current.name) && !_.includes(ignoredStates, $state.current.name)) {
                    var loggedInUser = $sessionStorage.wiseOneUser;
                    if (!loggedInUser) {
                        $state.go('login');
                    }
                }
            });

            $rootScope.$on('$viewContentLoaded',
                function(event, toState, toParams, fromState, fromParams) {
                    if ($rootScope.settings.rtl && $state.current.name != "persian.dashboard" && $state.current.name != "arabic.dashboard") {
                        switchClasses("pull-right", "pull-left");
                        switchClasses("databox-right", "databox-left");
                        switchClasses("item-right", "item-left");
                    }
                    if ($state.current.name == 'error404') {
                        $('body').addClass('body-404');
                    }
                    if ($state.current.name == 'error500') {
                        $('body').addClass('body-500');
                    }
                    $timeout(function() {
                        if ($rootScope.settings.fixed.sidebar) {
                            //Slim Scrolling for Sidebar Menu in fix state
                            var position = $rootScope.settings.rtl ? 'right' : 'left';
                            if (!$('.page-sidebar').hasClass('menu-compact')) {
                                $('.sidebar-menu').slimscroll({
                                    position: position,
                                    size: '3px',
                                    color: $rootScope.settings.color.themeprimary,
                                    height: $(window).height() - 90,
                                });
                            }
                        } else {
                            if ($(".sidebar-menu").closest("div").hasClass("slimScrollDiv")) {
                                $(".sidebar-menu").slimScroll({ destroy: true });
                                $(".sidebar-menu").attr('style', '');
                            }
                        }
                    }, 500);

                    window.scrollTo(0, 0);
                });

            $rootScope.$on('IdleStart', function() {
                // the user appears to have gone idle               
                closeModals();

                $rootScope.warning = $modal.open({
                    templateUrl: 'warning-dialog.html',
                    windowClass: 'modal-danger'
                });
            });

            $rootScope.$on('IdleEnd', function() {
                closeModals();
            });

            $rootScope.$on('IdleTimeout', function() {
                closeModals();
                $sessionStorage.wiseOneUser = undefined;
                $state.go('login');
                $rootScope.timedout = $modal.open({
                    templateUrl: 'timedout-dialog.html',
                    windowClass: 'modal-danger'
                });
            });

            function closeModals() {
                if ($rootScope.warning) {
                    $rootScope.warning.close();
                    $rootScope.warning = null;
                }

                if ($rootScope.timedout) {
                    $rootScope.timedout.close();
                    $rootScope.timedout = null;
                }
            }
        }
    ])
    .config(function(IdleProvider, KeepaliveProvider) {
        // configure Idle settings
        IdleProvider.idle(600); // in seconds
        IdleProvider.timeout(15); // in seconds
        KeepaliveProvider.interval(2); // in seconds
    })
    .run(function(Idle) {
        // start watching when the app runs. also starts the Keepalive service by default.
        Idle.watch();
    });