app.controller('StaffReportModalCtrl',
    function($scope, $filter, configFileService, $modalInstance, reportDto) {

        $scope.summary = reportDto;

        $scope.getReport = function() {

            var uri = configFileService.apiHandler.retrieveStaffReportDetail;

            var searchData = {
                ReportIDs: reportDto.ReportIDs
            };

            $scope.loading = true;
            configFileService
                .post(uri, searchData)
                .success(function(data, status, headers) {
                    $scope.loading = false;
                    initStaffList(data.data);
                })
                .error(function(data, status, headers) {
                    configFileService.displayMessage('info', 'Report Management', data);
                    $scope.loading = false;
                });

        };

        function initStaffList(reports) {

            $('#StaffList').find('tfoot').find('th').each(function() {
                var title = $(this).text();
                if (title != "")
                    $(this).html('<input type="text" placeholder="Search ' + title + '" />');
            });

            if ($.fn.DataTable.isDataTable('#StaffList')) {

                var table = $('#StaffList').DataTable();
                table.clear().draw();
                table.rows.add(reports);
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

                var table = $('#StaffList').DataTable({
                    processing: false,
                    data: reports,
                    columns: [
                        { "data": "Date" },
                        { "data": "LocationName" },
                        { "data": "RoomName" },
                        { "data": "From" },
                        { "data": "To" },
                        { "data": "Reason" },
                    ],
                    order: [
                        [0, "desc"]
                    ],
                    dom: 'frtip'
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
        }

        $scope.getReport();

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };

    }
);