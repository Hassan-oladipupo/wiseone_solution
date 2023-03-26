'use strict';

app.controller('ViewStaffCtrl', function($sessionStorage, $scope, $state, configFileService) {

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
            ajax: configFileService.apiHandler.retrieveStaffs,
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
                { "data": "Location.Name" },
                { "data": "Role.Name" },
                { "data": "StartDate" },
                { "data": "EndDate" },
                { "data": "LeaveType" },
                { "data": "Status" },
                {
                    "className": 'view',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [6, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'A4'
            }]
        });

        $staffList.find('tbody').on('click', 'td.view', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $sessionStorage.transferData = {
                edit: false,
                data: data
            };

            $state.go('app.staffdetail');
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