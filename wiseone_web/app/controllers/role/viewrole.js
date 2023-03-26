'use strict';

app.controller('ViewRoleCtrl', function($sessionStorage, $scope, $state, configFileService) {

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
            columns: [
                { "data": "Name" },
                { "data": "Type" },
                { "data": "CanAccessWeb" },
                { "data": "Status" },
                { "data": "CreatedOn" },
                { "data": "LastUpdatedOn" },
                {
                    "className": 'list',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $roleList.find('tbody').on('click', 'td.list', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.show(data);
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

    $scope.show = function(data) {

        $sessionStorage.transferData = {
            edit: false,
            data: data
        };

        $state.go('app.roledetail');
    };
});