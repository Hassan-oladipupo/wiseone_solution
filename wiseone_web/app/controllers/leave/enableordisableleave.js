'use strict';

app.controller('EnableorDisableLeaveCtrl', function($scope, configFileService, confirmService) {

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
            ajax: configFileService.apiHandler.retrieveOpenedClosedFinancialYears,
            columnDefs: [{
                "targets": [6],
                "data": "description",
                "render": function(data, type, full, meta) {
                    return full.Status == 'Closed' ? '<i class = "fa fa-toggle-on themesecondary bordered-themesecondary dt-icon"></i>' : '<i class = "fa fa-toggle-off themeprimary bordered-themeprimary dt-icon"></i>';
                }
            }],
            columns: [
                { "data": "StartDate" },
                { "data": "EndDate" },
                { "data": "Location.Name" },
                { "data": "LeavePriorNotice" },
                { "data": "ExcludeMonthsDetails" },
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

        $calenderList.find('tbody').on('click', 'td.edit', function() {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var data = row.data();

            $scope.confirmEdit(data);
        });

        $calenderList.find('tfoot').find('input').on('keyup change', function() {
            table
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    };

    $scope.refresh = function() {
        var table = $calenderList.DataTable();
        table.ajax.reload();
    };

    $scope.confirmEdit = function(data) {

        var subTitle = data.Status == 'Opened' ? 'Disable Leave Calender ' : 'Enable Leave Calender ';
        var subMsg = data.Status == 'Opened' ? 'Disable this Leave Calender?' : 'Enable this Leave Calender?';

        var modalOptions = {
            headerText: subTitle + data.StartDate + ' - ' + data.EndDate + '?',
            bodyText: 'Are you sure you want to ' + subMsg
        };

        confirmService.showModal({}, modalOptions).then(function() {

            $scope.loading = true;

            data.Status = data.Status == 'Opened' ? 'Closed' : 'Opened';

            configFileService
                .put(configFileService.apiHandler.toggleFinancialYear, data)
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

    };
});