'use strict';

app.controller('UpdateLocationCtrl', function($sessionStorage, $scope, $state, configFileService, confirmService) {

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
            ajax: configFileService.apiHandler.retrieveActiveLocations,
            columns: [
                { "data": "Name" },
                { "data": "Latitude" },
                { "data": "Longitude" },
                { "data": "HeadOffice" },
                { "data": "OperationOffice" },
                { "data": "Address" },
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
    }

    $scope.refresh = function() {
        var table = $locationList.DataTable();
        table.ajax.reload();
    }

    $scope.confirmEdit = function(data) {

        var modalOptions = {
            headerText: 'Modify location: ' + data.Name + '?',
            bodyText: 'Are you sure you want to modify this location?'
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $sessionStorage.transferData = {
                edit: true,
                data: data
            };

            $state.go('app.locationdetail');
        });
    }
});