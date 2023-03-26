'use strict';

app.controller('CancelStaffLeaveCtrl', function($scope, configFileService, confirmService, $sessionStorage, $state) {

    var $staffList = $('#staffList');

    $scope.init = function() {

        $staffList.find('tfoot').find('th').each(function() {
            var title = $staffList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $staffList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveActiveStaffOnLeave,
            columnDefs: [{
                "targets": [0],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return '<img src="' + data + '" alt ="images" class="image-icon" />';
                }
            }],
            columns: [{
                    "data": "Picture",
                    "orderable": false
                },
                { "data": "Name" },
                { "data": "Username" },
                { "data": "StaffID" },
                { "data": "Role.Name" },
                { "data": "Location.Name" },
                {
                    "className": 'analytics',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i title="View Staff Leave Analytics" class = "fa fa-pie-chart themeprimary bordered-themeprimary dt-icon"></i>'
                },
                {
                    "className": 'view',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i title="View Staff Leave Details" class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                },
                {
                    "className": 'create',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i title="Create Leave for Staff" class = "fa fa-plus-square themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'A4'
            }]
        });

        $staffList.find('tbody').on('click', 'td.analytics', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $sessionStorage.transferData = {
                data: data
            };

            $state.go('app.staffleaveanalytics');
        });

        $staffList.find('tbody').on('click', 'td.view', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $sessionStorage.transferData = {
                data: data
            };

            $state.go('app.staffleavedetails');
        });

        $staffList.find('tbody').on('click', 'td.create', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $sessionStorage.transferData = {
                data: data
            };

            $state.go('app.createstaffleave');
        });

        $staffList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $staffList.DataTable();
        table.ajax.reload();
    };

});