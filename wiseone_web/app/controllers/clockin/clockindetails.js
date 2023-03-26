app.controller('ClockInDetailsCtrl', function($sessionStorage, $scope, $window, configFileService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    var $clockInDetails = $('#clockInDetails');

    $scope.clockIn = $sessionStorage.transferData.data;
    $scope.startDate = $sessionStorage.transferData.startDate;
    $scope.endDate = $sessionStorage.transferData.endDate;

    $scope.search = function() {

        var uri = configFileService.apiHandler.retrieveStaffSignInOuts;

        var searchData = {
            FromDate: $scope.startDate,
            ToDate: $scope.endDate,
            StaffID: $scope.clockIn.Staff.ID
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initClockInDetails(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In', data);
                $scope.loading = false;
            });

    };

    var initClockInDetails = function(clockIns) {

        $clockInDetails.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#clockInDetails')) {

            var table = $clockInDetails.DataTable();
            table.clear().draw();
            table.rows.add(clockIns);
            table.columns.adjust().draw();
            table.search("").draw();

            table.columns().every(function() {
                var that = this;

                $('input', this.footer()).on('keyup change', function() {
                    if (that.search() !== this.value) {
                        that
                            .search(this.value)
                            .draw();
                    }
                });
            });

        } else {

            var table = $clockInDetails.DataTable({
                processing: false,
                data: clockIns,
                columns: [
                    { "data": "Date" },
                    { "data": "ShiftStartTime" },
                    { "data": "ClockInTime" },
                    { "data": "ClockOutTime" },
                    { "data": "ShiftEndTime" },
                    { "data": "ApprovedOverTime" },
                ],
                order: [
                    [0, "asc"]
                ],
                dom: 'Bfrtip',
                buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
            });

            table.columns().every(function() {
                var that = this;
                $('input', this.footer()).on('keyup change', function() {
                    if (that.search() !== this.value) {
                        that
                            .search(this.value)
                            .draw();
                    }
                });
            });

        }
    };

    $scope.goBack = function() {
        $window.history.back();
    };

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });
});