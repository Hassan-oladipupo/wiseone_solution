app.controller('ApproverEmailCtrl', function($scope, configFileService, confirmService, $modal) {

    var $emailList = $('#emailList'),
        $slEmailList = $('#slEmailList');

    $scope.roles = [];
    $scope.requestingRoles = [];
    $scope.approvingRoles = [];

    $scope.init = function() {
        $scope.getSettings();
        $scope.getSecondLevelSettings();
        $scope.getRoles();
    };

    $scope.getRoles = function() {
        $scope.loading = true;
        configFileService
            .get(configFileService.apiHandler.retrieveRoles)
            .success(function(data, status, headers) {
                $scope.roles = data.data;
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Approver Configuration', data);
            });
    };

    $scope.getSettings = function() {

        $emailList.find('tfoot').find('th').each(function() {
            var title = $emailList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $emailList.DataTable({
            processing: true,
            ajax: configFileService.apiHandler.retrieveConfigurations,
            columns: [
                { "data": "ApprovalTypeDesc" },
                { "data": "RequestingRole.Name" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
                },
                {
                    "className": 'delete',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-trash themesecondary bordered-themesecondary dt-icon"></i>'
                }
            ],
            order: [
                [1, "asc"]
            ],
            dom: 'frtip'
        });

        $emailList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $emailList.find('tbody').on('click', 'td.delete', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmDelete(data);
        });

        $emailList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.getSecondLevelSettings = function() {

        $slEmailList.find('tfoot').find('th').each(function() {
            var title = $slEmailList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $slEmailList.DataTable({
            processing: true,
            ajax: configFileService.apiHandler.retrieveSecondLevelConfigurations,
            columns: [
                { "data": "ApproverRole.Name" },
                {
                    "className": 'delete',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-trash themesecondary bordered-themesecondary dt-icon"></i>'
                }
            ],
            order: [
                [1, "asc"]
            ],
            dom: 'frtip'
        });

        $slEmailList.find('tbody').on('click', 'td.delete', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            var modalOptions = {
                headerText: 'Delete ' + data.ApproverRole.Name + ' ?',
                bodyText: 'Are you sure you want to delete this approver?'
            };

            confirmService.showModal({}, modalOptions).then(function() {

                $scope.loading = true;

                configFileService
                    .put(configFileService.apiHandler.deleteSecondLevelConfiguration, data)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Approval Configuration', data);
                        $scope.loading = false;
                        $scope.refreshSL();
                    })
                    .error(function(data, status, headers) {
                        configFileService.displayMessage('info', 'Approval Configuration', data);
                        $scope.loading = false;
                    });
            });

        });

        $slEmailList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.confirmEdit = function(data) {

        var data = {
            newConfiguration: false,
            roles: $scope.roles,
            configuration: data
        };

        var modalInstance = $modal.open({
            backdrop: false,
            keyboard: true,
            templateUrl: 'views/approveremail/configuremodal.html',
            controller: 'ConfigureModalCtrl',
            size: 'lg',
            resolve: {
                approverConfigurationDto: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(approverEmail) {
            $scope.loading = true;
            configFileService
                .put(configFileService.apiHandler.updateConfiguration, approverEmail)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Approval Configuration', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Approval Configuration', data);
                    $scope.loading = false;
                });
        });

    };

    $scope.confirmDelete = function(data) {

        var modalOptions = {
            headerText: 'Delete ' + data.ApprovalTypeDesc + ' approval configuration for ' + data.RequestingRole.Name + '?',
            bodyText: 'Are you sure you want to delete this configuration?'
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            configFileService
                .put(configFileService.apiHandler.deleteConfiguration, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Approval Configuration', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Approval Configuration', data);
                    $scope.loading = false;
                });
        });

    };

    $scope.addNewConfiguration = function() {

        var data = {
            newConfiguration: true,
            roles: $scope.roles,
            configuration: new approveremaildto()
        };

        var modalInstance = $modal.open({
            backdrop: false,
            keyboard: true,
            templateUrl: 'views/approveremail/configuremodal.html',
            controller: 'ConfigureModalCtrl',
            size: 'lg',
            resolve: {
                approverConfigurationDto: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(approverEmail) {
            $scope.loading = true;
            configFileService
                .post(configFileService.apiHandler.saveConfiguration, approverEmail)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Approval Configuration', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Approval Configuration', data);
                    $scope.loading = false;
                });
        });
    };

    $scope.addNewSLConfiguration = function() {

        var data = {
            roles: $scope.roles,
            configuration: new approveremaildto()
        };

        var modalInstance = $modal.open({
            backdrop: false,
            keyboard: true,
            templateUrl: 'views/approveremail/slconfiguremodal.html',
            controller: 'SLConfigureModalCtrl',
            resolve: {
                approverConfigurationDto: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(approverEmail) {
            $scope.loading = true;
            configFileService
                .post(configFileService.apiHandler.saveSecondLevelConfiguration, approverEmail)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Approval Configuration', data);
                    $scope.loading = false;
                    $scope.refreshSL();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Approval Configuration', data);
                    $scope.loading = false;
                });
        });
    };

    $scope.refresh = function() {
        var table = $emailList.DataTable();
        table.ajax.reload();
    };

    $scope.refreshSL = function() {
        var table = $slEmailList.DataTable();
        table.ajax.reload();
    };

});