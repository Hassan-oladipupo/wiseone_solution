'use strict';

app.controller('EnableorDisableRoleCtrl', function($scope, configFileService, confirmService) {

    var $roleList = $('#roleList');

    $scope.init = function() {

        $roleList.find('tfoot').find('th').each(function() {
            var title = $roleList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $roleList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveRoles,
            columnDefs: [{
                "targets": [6],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return full.Status == 'InActive' ? '<i class = "fa fa-toggle-on themesecondary bordered-themesecondary dt-icon"></i>' : '<i class = "fa fa-toggle-off themeprimary bordered-themeprimary dt-icon"></i>';
                }
            }],
            columns: [
                { "data": "Name" },
                { "data": "Type" },
                { "data": "CanAccessWeb" },
                { "data": "Status" },
                { "data": "CreatedOn" },
                { "data": "LastUpdatedOn" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $roleList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $roleList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $roleList.DataTable();
        table.ajax.reload();
    };

    $scope.confirmEdit = function(data) {

        var subTitle = data.Status == 'Active' ? 'Disable role ' : 'Enable role ';
        var subMsg = data.Status == 'Active' ? 'disable this role?' : 'enable this role?';

        var modalOptions = {
            headerText: subTitle + data.Name + '?',
            bodyText: 'Are you sure you want to ' + subMsg
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            data.Status = data.Status == 'Active' ? 'InActive' : 'Active';

            configFileService
                .put(configFileService.apiHandler.enableOrDisableRole, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Role Management', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Role Management', data);
                    $scope.loading = false;
                });
        });

    };
});