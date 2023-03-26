'use strict';
angular.module('app')
    .run(
        [
            '$rootScope', '$state', '$stateParams',
            function($rootScope, $state, $stateParams) {
                $rootScope.$state = $state;
                $rootScope.$stateParams = $stateParams;
            }
        ]
    )
    .config(
        [
            '$stateProvider', '$urlRouterProvider',
            function($stateProvider, $urlRouterProvider) {

                $urlRouterProvider
                    .otherwise('/login');
                $stateProvider
                    .state('login', {
                        url: '/login',
                        cache: false,
                        templateUrl: 'views/system/login.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Login'
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/system/login.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('forgotpassword', {
                        url: '/forgotpassword',
                        cache: false,
                        templateUrl: 'views/system/forgotpassword.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Forgot Password'
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/system/forgotpassword.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('resetpassword', {
                        url: '/resetpassword',
                        cache: false,
                        templateUrl: 'views/system/resetpassword.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Reset Password'
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/system/resetpassword.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app', {
                        abstract: true,
                        url: '/app',
                        templateUrl: 'views/layout.html?v=6'
                    })
                    .state('app.dashboard', {
                        url: '/dashboard',
                        cache: false,
                        templateUrl: 'views/dashboard.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Dashboard',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/dashboard.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.changepassword', {
                        url: '/changepassword',
                        cache: false,
                        templateUrl: 'views/system/changepassword.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Password Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/system/changepassword.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.addfunction', {
                        url: '/addfunction',
                        cache: false,
                        templateUrl: 'views/function/addfunction.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Function Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/functiondto.js?v=6',
                                            'app/controllers/function/addfunction.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updatefunction', {
                        url: '/updatefunction',
                        cache: false,
                        templateUrl: 'views/function/updatefunction.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Function Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/function/functionModalCtrl.js?v=6',
                                            'app/controllers/function/updatefunction.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewfunction', {
                        url: '/viewfunction',
                        cache: false,
                        templateUrl: 'views/function/viewfunction.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Function Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/function/viewfunction.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.addrole', {
                        url: '/addrole',
                        templateUrl: 'views/role/addrole.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Role Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'lib/jquery/jstree/jstree.min.js',
                                            'lib/jquery/jstree/jstree.css',
                                            'app/dto/roledto.js?v=6',
                                            'app/controllers/role/addrole.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updaterole', {
                        url: '/updaterole',
                        templateUrl: 'views/role/updaterole.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Role Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/role/updaterole.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.roledetail', {
                        url: '/roledetail',
                        templateUrl: 'views/role/roledetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Role Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'lib/jquery/jstree/jstree.min.js',
                                            'lib/jquery/jstree/jstree.css',
                                            'app/controllers/role/roledetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.enableordisablerole', {
                        url: '/enableordisablerole',
                        templateUrl: 'views/role/enableordisablerole.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Role Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/role/enableordisablerole.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewrole', {
                        url: '/viewrole',
                        templateUrl: 'views/role/viewrole.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Role Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/role/viewrole.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.configureemail', {
                        url: '/configureemail',
                        templateUrl: 'views/approveremail/configure.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Notification Rules',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/approveremaildto.js?v=6',
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/approveremail/configuremodal.js?v=6',
                                            'app/controllers/approveremail/slconfiguremodal.js?v=6',
                                            'app/controllers/approveremail/configure.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.approvestaffsignup', {
                        url: '/approvestaffsignup',
                        templateUrl: 'views/staff/approvestaffsignup.html?v=7',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/existingstaffctrl.js?v=7',
                                            'app/controllers/staff/approvalmodalctrl.js?v=7',
                                            'app/controllers/staff/approvestaffsignup.js?v=7',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updatestaff', {
                        url: '/updatestaff',
                        templateUrl: 'views/staff/updatestaff.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/updatestaff.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.staffdetail', {
                        url: '/staffdetail',
                        templateUrl: 'views/staff/staffdetail.html?v=7',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/staff/staffdetail.js?v=7',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.enableordisablestaff', {
                        url: '/enableordisablestaff',
                        templateUrl: 'views/staff/enableordisablestaff.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/enableordisablestaff.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewstaff', {
                        url: '/viewstaff',
                        templateUrl: 'views/staff/viewstaff.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/staff/viewstaff.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.cancelstaffleave', {
                        url: '/cancelstaffleave',
                        templateUrl: 'views/staff/cancelstaffleave.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/cancelstaffleave.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.createstaffleave', {
                        url: '/createstaffleave',
                        templateUrl: 'views/staff/createstaffleave.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/createstaffleave.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.staffleavedetails', {
                        url: '/staffleavedetails',
                        templateUrl: 'views/staff/staffleavedetails.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/staffleavedetails.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.staffleaveanalytics', {
                        url: '/staffleaveanalytics',
                        templateUrl: 'views/staff/staffleaveanalytics.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Staff Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/staff/staffleaveanalytics.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.addlocation', {
                        url: '/addlocation',
                        templateUrl: 'views/location/addlocation.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Location Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/locationdto.js?v=6',
                                            'app/controllers/location/addlocation.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updatelocation', {
                        url: '/updatelocation',
                        templateUrl: 'views/location/updatelocation.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Location Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/location/updatelocation.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.locationdetail', {
                        url: '/locationdetail',
                        templateUrl: 'views/location/locationdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Location Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/location/locationdetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.enableordisablelocation', {
                        url: '/enableordisablelocation',
                        templateUrl: 'views/location/enableordisablelocation.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Location Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/location/enableordisablelocation.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewlocation', {
                        url: '/viewlocation',
                        templateUrl: 'views/location/viewlocation.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Location Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/location/viewlocation.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.addclassroom', {
                        url: '/addclassroom',
                        templateUrl: 'views/classroom/addclassroom.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Room Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/classroomdto.js?v=6',
                                            'app/controllers/classroom/addclassroom.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updateclassroom', {
                        url: '/updateclassroom',
                        templateUrl: 'views/classroom/updateclassroom.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Room Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/classroom/updateclassroom.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.classroomdetail', {
                        url: '/classroomdetail',
                        templateUrl: 'views/classroom/classroomdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Room Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/classroom/classroomdetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.enableordisableclassroom', {
                        url: '/enableordisableclassroom',
                        templateUrl: 'views/classroom/enableordisableclassroom.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Room Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/classroom/enableordisableclassroom.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewclassroom', {
                        url: '/viewclassroom',
                        templateUrl: 'views/classroom/viewclassroom.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Room Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/classroom/viewclassroom.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.createshift', {
                        url: '/createshift',
                        templateUrl: 'views/shift/createshift.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/shiftdto.js?v=6',
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/shift/createshift.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updateshift', {
                        url: '/updateshift',
                        templateUrl: 'views/shift/updateshift.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/shift/updateshift.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.shiftdetail', {
                        url: '/shiftdetail',
                        templateUrl: 'views/shift/shiftdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/shiftdto.js?v=6',
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/shift/shiftdetail.js?v=7',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.approveshiftswap', {
                        url: '/approveshiftswap',
                        templateUrl: 'views/shift/approveshiftswap.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/shift/shiftswapmodal.js?v=6',
                                            'app/controllers/shift/approveshiftswap.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewshift', {
                        url: '/viewshift',
                        templateUrl: 'views/shift/viewshift.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/shift/viewshift.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.cancelshift', {
                        url: '/cancelshift',
                        templateUrl: 'views/shift/cancelshift.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Shift Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/shift/cancelshiftmodal.js?v=6',
                                            'app/controllers/shift/cancelshift.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.leaveconfiguration', {
                        url: '/leaveconfiguration',
                        templateUrl: 'views/leave/leaveconfiguration.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/dto/leavedto.js?v=6',
                                            'app/dto/locationdto.js?v=6',
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/leaveconfiguration.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updateleavecalender', {
                        url: '/updateleavecalender',
                        templateUrl: 'views/leave/updateleavecalender.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/updateleavecalender.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.enableordisableleave', {
                        url: '/enableordisableleave',
                        templateUrl: 'views/leave/enableordisableleave.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/enableordisableleave.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewleavecalendar', {
                        url: '/viewleavecalendar',
                        templateUrl: 'views/leave/viewleavecalendar.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/leave/viewleavecalendar.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.leaveconfigurationdetail', {
                        url: '/leaveconfigurationdetail',
                        templateUrl: 'views/leave/leaveconfigurationdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/leave/leaveconfigurationdetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.approveleave', {
                        url: '/approveleave',
                        templateUrl: 'views/leave/approveleave.html?v=7',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: 'First Level Approval'
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/leavemodal.js?v=7',
                                            'app/controllers/leave/approveleave.js?v=7'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.approvesecondlevel', {
                        url: '/approvesecondlevel',
                        templateUrl: 'views/leave/approvesecondlevel.html?v=7',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: 'Second Level Approval'
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/approvesecondlevelmodal.js?v=7',
                                            'app/controllers/leave/approvesecondlevel.js?v=7'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.cancelleave', {
                        url: '/cancelleave',
                        templateUrl: 'views/leave/cancelleave.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Leave Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/leave/cancelleavemodal.js?v=6',
                                            'app/controllers/leave/cancelleave.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.createtask', {
                        url: '/createtask',
                        templateUrl: 'views/task/createtask.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/filters/propsFilter.js?v=6',
                                            'app/dto/taskdto.js?v=6',
                                            'app/controllers/task/createtask.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updatetask', {
                        url: '/updatetask',
                        templateUrl: 'views/task/updatetask.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/task/updatetask.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.taskdetail', {
                        url: '/taskdetail',
                        templateUrl: 'views/task/taskdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/filters/propsFilter.js?v=6',
                                            'app/dto/taskdto.js?v=6',
                                            'app/controllers/task/taskdetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.updatetaskdetail', {
                        url: '/updatetaskdetail',
                        templateUrl: 'views/task/updatetaskdetail.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/filters/propsFilter.js?v=6',
                                            'app/dto/taskdto.js?v=6',
                                            'app/controllers/task/updatetaskdetail.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.deletetask', {
                        url: '/deletetask',
                        templateUrl: 'views/task/deletetask.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/task/deletetask.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewtask', {
                        url: '/viewtask',
                        templateUrl: 'views/task/viewtask.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Task Management',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/task/viewtask.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.approveovertime', {
                        url: '/approveovertime',
                        templateUrl: 'views/clockin/approveovertime.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Approve Over Time',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/services/confirm.js?v=6',
                                            'app/controllers/clockin/modal.js?v=6',
                                            'app/controllers/clockin/approveovertime.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewclockin', {
                        url: '/viewclockin',
                        templateUrl: 'views/clockin/viewclockin.html?v=6',
                        ncyBreadcrumb: {
                            label: 'View Clock In',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/clockin/viewclockin.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.clockindetails', {
                        url: '/clockindetails',
                        templateUrl: 'views/clockin/clockindetails.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Clock In Details',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/clockin/clockindetails.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.clockinmonitor', {
                        url: '/clockinmonitor',
                        templateUrl: 'views/clockin/clockinmonitor.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Monitor',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/clockin/clockinmonitor.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.viewreport', {
                        url: '/viewreport',
                        templateUrl: 'views/report/report.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Reports',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/report/reportmodal.js?v=6',
                                            'app/controllers/report/report.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.staffreport', {
                        url: '/staffreport',
                        templateUrl: 'views/report/staffreport.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Reports',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/report/staffreportmodal.js?v=6',
                                            'app/controllers/report/staffreport.js?v=6'
                                        ]
                                    });
                                }
                            ]
                        }
                    })
                    .state('app.services', {
                        url: '/viewservices',
                        cache: false,
                        templateUrl: 'views/services/viewservices.html?v=6',
                        ncyBreadcrumb: {
                            label: 'Background Services',
                            description: ''
                        },
                        resolve: {
                            deps: [
                                '$ocLazyLoad',
                                function($ocLazyLoad) {
                                    return $ocLazyLoad.load({
                                        serie: true,
                                        files: [
                                            'app/controllers/services/viewservices.js?v=6',
                                        ]
                                    });
                                }
                            ]
                        }
                    });
            }
        ]
    );