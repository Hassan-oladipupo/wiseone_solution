'use strict';

app.controller('ViewFunctionCtrl', function($scope, configFileService) {

    var $functionList = $('#functionList');

    $scope.getFunctions = function() {

        $functionList.find('tfoot').find('th').each(function() {
            var title = $functionList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $functionList.DataTable({
            stateSave: true,
            processing: true,
            ajax: configFileService.apiHandler.retrieveFunctions,
            columns: [
                { "data": "Name" },
                { "data": "Type" },
                { "data": "Module" }
            ],
            order: [
                [2, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $functionList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    }

    $scope.refresh = function() {
        var table = $functionList.DataTable();
        table.ajax.reload();
    }
});