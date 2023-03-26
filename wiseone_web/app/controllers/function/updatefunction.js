app.controller('UpdateFunctionCtrl', function($scope, $modal, configFileService) {

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
                { "data": "Module" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
                }
            ],
            order: [
                [2, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $functionList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.saveFunction(data);
        });

        $functionList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $functionList.DataTable();
        table.ajax.reload();
    };

    $scope.saveFunction = function(data) {

        var modalInstance = $modal.open({
            templateUrl: 'views/function/modal.html',
            controller: 'functionModalCtrl',
            resolve: {
                functionToUpdate: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(modifiedFunction) {

            configFileService.displayMessage('info', 'Function Management', 'updating function...');

            configFileService
                .put(configFileService.apiHandler.updateFunction, modifiedFunction)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Function Management', data);
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Function Management', data);
                });
        });

    };
});