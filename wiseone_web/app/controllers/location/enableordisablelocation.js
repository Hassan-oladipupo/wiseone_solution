'use strict';

app.controller('EnableorDisableLocationCtrl', function($scope, configFileService, confirmService) {

    var $locationList = $('#locationList');

    $scope.init = function() {

        $locationList.find('tfoot').find('th').each(function() {
            var title = $locationList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $locationList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveLocations,
            columnDefs: [{
                "targets": [7],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return full.Status == 'InActive' ? '<i class = "fa fa-toggle-on themesecondary bordered-themesecondary dt-icon"></i>' : '<i class = "fa fa-toggle-off themeprimary bordered-themeprimary dt-icon"></i>';
                }
            }],
            columns: [
                { "data": "Name" },
                { "data": "Latitude" },
                { "data": "Longitude" },
                { "data": "HeadOffice" },
                { "data": "OperationOffice" },
                { "data": "Address" },
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

        $locationList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $locationList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $locationList.DataTable();
        table.ajax.reload();
    };

    $scope.confirmEdit = function(data) {

        var subTitle = data.Status == 'Active' ? 'Disable location ' : 'Enable location ';
        var subMsg = data.Status == 'Active' ? 'disable this location?' : 'enable this location?';

        var modalOptions = {
            headerText: subTitle + data.Name + '?',
            bodyText: 'Are you sure you want to ' + subMsg
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            data.Status = data.Status == 'Active' ? 'InActive' : 'Active';

            configFileService
                .put(configFileService.apiHandler.enableOrDisableLocation, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Location Management', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Location Management', data);
                    $scope.loading = false;
                });
        });

    };
});