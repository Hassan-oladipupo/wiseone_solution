'use strict';

app.controller('UpdateLeaveCalenderCtrl', function($sessionStorage, $scope, $state, configFileService, confirmService) {

    var $calenderList = $('#calenderList');

    $scope.init = function() {

        $calenderList.find('tfoot').find('th').each(function() {
            var title = $calenderList.find('thead').find('th').eq($(this).index()).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        var table = $calenderList.DataTable({
            stateSave: false,
            processing: true,
            ajax: configFileService.apiHandler.retrieveFinancialYears + '?status=' + 'Opened',
            columns: [
                { "data": "StartDate" },
                { "data": "EndDate" },
                { "data": "Location.Name" },
                { "data": "LeavePriorNotice" },
                { "data": "ExcludeMonthsDetails" },
                { "data": "CreatedOn" },
                {
                    "className": 'edit',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-edit themeprimary bordered-themeprimary dt-icon"></i>'
                },
                {
                    "className": 'delete',
                    "orderable": false,
                    "data": null,
                    "defaultContent": '<i class = "fa fa-trash themesecondary bordered-themesecondary dt-icon"></i>'
                }
            ],
            order: [
                [0, "asc"]
            ],
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
        });

        $calenderList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $calenderList.find('tbody').on('click', 'td.delete', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmDelete(data);
        });

        $calenderList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    }

    $scope.refresh = function() {
        var table = $calenderList.DataTable();
        table.ajax.reload();
    }

    $scope.confirmEdit = function(data) {

        var modalOptions = {
            headerText: 'Update Financial Year: ' + data.StartDate + ' - ' + data.EndDate + '?',
            bodyText: 'Are you sure you want to update this Financial Year?'
        };

        confirmService.showModal({}, modalOptions).then(function() {
            $sessionStorage.transferData = {
                edit: true,
                data: data
            };

            $state.go('app.leaveconfigurationdetail');
        });
    }

    $scope.confirmDelete = function(data) {

        var modalOptions = {
            headerText: 'Delete Financial Year: ' + data.StartDate + ' - ' + data.EndDate + '?',
            bodyText: 'Are you sure you want to delete this Financial Year?'
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            configFileService
                .put(configFileService.apiHandler.deleteFinancialYear, data)
                .success(function(data, status, headers) {
                    configFileService.displayMessage('success', 'Leave Management', data);
                    $scope.loading = false;
                    $scope.refresh();
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Leave Management', data);
                    $scope.loading = false;
                });

        });
    }
});