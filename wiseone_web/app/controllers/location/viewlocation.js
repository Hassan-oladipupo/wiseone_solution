'use strict';

app.controller('ViewLocationCtrl', function($sessionStorage, $scope, $state, configFileService) {

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
            columns: [
                { "data": "Name" },
                { "data": "Latitude" },
                { "data": "Longitude" },
                { "data": "HeadOffice" },
                { "data": "OperationOffice" },
                { "data": "Address" },
                { "data": "Status" },
                { "data": "CreatedOn" },
                { "data": "LastUpdatedOn" },
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
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
});