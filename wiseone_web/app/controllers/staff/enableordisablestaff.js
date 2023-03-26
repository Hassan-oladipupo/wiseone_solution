'use strict';

app.controller('EnableorDisableStaffCtrl', function($scope, configFileService, confirmService) {

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
            }, {
                "targets": [7],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return full.Status == 'InActive' ? '<i class = "fa fa-toggle-on themesecondary bordered-themesecondary dt-icon"></i>' : '<i class = "fa fa-toggle-off themeprimary bordered-themeprimary dt-icon"></i>';
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
                { "data": "Status" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null
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
    };

    $scope.refresh = function() {
        var table = $staffList.DataTable();
        table.ajax.reload();
    };

    $scope.confirmEdit = function(data) {

        var subTitle = data.Status == 'Active' ? 'Disable staff ' : 'Enable staff ';
        var subMsg = data.Status == 'Active' ? 'disable this staff?' : 'enable this staff?';

        var modalOptions = {
            headerText: subTitle + data.Surname + '?',
            bodyText: 'Are you sure you want to ' + subMsg
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            data.Status = data.Status == 'Active' ? 'InActive' : 'Active';

            configFileService
                .put(configFileService.apiHandler.enableOrDisableStaff, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Staff Management', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Staff Management', data);
                    $scope.loading = false;
                });
        });

    };
});