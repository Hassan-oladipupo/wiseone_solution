'use strict';

app.controller('UpdateRoleCtrl', function($sessionStorage, $scope, $state, configFileService, confirmService) {

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
            ajax: configFileService.apiHandler.retrieveActiveRoles,
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
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
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
    }

    $scope.refresh = function() {
        var table = $roleList.DataTable();
        table.ajax.reload();
    }

    $scope.confirmEdit = function(data) {

        var modalOptions = {
            headerText: 'Modify role: ' + data.Name + '?',
            bodyText: 'Are you sure you want to modify this role?'
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $sessionStorage.transferData = {
                edit: true,
                data: data
            };

            $state.go('app.roledetail');
        });
    }
});