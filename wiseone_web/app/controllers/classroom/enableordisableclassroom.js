'use strict';

app.controller('EnableorDisableClassRoomCtrl', function($scope, configFileService, confirmService) {

    var $classRoomList = $('#classRoomList');

    $scope.init = function() {

        $classRoomList.find('tfoot').find('th').each(function() {
            var title = $classRoomList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $classRoomList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveClassRooms,
            columnDefs: [{
                "targets": [5],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return full.Status == 'InActive' ? '<i class = "fa fa-toggle-on themesecondary bordered-themesecondary dt-icon"></i>' : '<i class = "fa fa-toggle-off themeprimary bordered-themeprimary dt-icon"></i>';
                }
            }],
            columns: [
                { "data": "Name" },
                { "data": "Location.Name" },
                { "data": "CreatedOn" },
                { "data": "LastModifiedOn" },
                { "data": "Status" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $classRoomList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $classRoomList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $classRoomList.DataTable();
        table.ajax.reload();
    };

    $scope.confirmEdit = function(data) {

        var subTitle = data.Status == 'Active' ? 'Disable Room ' : 'Enable Room ';
        var subMsg = data.Status == 'Active' ? 'Disable this Room?' : 'Enable this Room?';

        var modalOptions = {
            headerText: subTitle + data.Name + '?',
            bodyText: 'Are you sure you want to ' + subMsg
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            data.Status = data.Status == 'Active' ? 'InActive' : 'Active';

            configFileService
                .put(configFileService.apiHandler.enableOrDisableClassRoom, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Room Management', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Room Management', data);
                    $scope.loading = false;
                });
        });

    };
});