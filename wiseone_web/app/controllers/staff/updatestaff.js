'use strict';

app.controller('UpdateStaffCtrl', function($sessionStorage, $scope, $state, configFileService, confirmService) {

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
            ajax: configFileService.apiHandler.retrieveActiveStaffs,
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
                { "data": "CreatedOn" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [4, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'A4'
            }]
        });

        $staffList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $staffList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    }

    $scope.refresh = function() {
        var table = $staffList.DataTable();
        table.ajax.reload();
    }

    $scope.confirmEdit = function(data) {

        var modalOptions = {
            headerText: 'Modify staff: ' + data.FirstName + ' ' + data.Surname + '?',
            bodyText: 'Are you sure you want to modify this staff?'
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $sessionStorage.transferData = {
                edit: true,
                data: data
            };

            $state.go('app.staffdetail');
        });
    };
});