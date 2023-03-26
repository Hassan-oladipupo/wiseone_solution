app.controller('StaffReportCtrl', function($scope, configFileService, $modal, $sessionStorage) {

    var $ReportList = $('#ReportList');

    $scope.reports = [];

    $scope.searchValue = {
        displayLoadingInfo: true,
        dateRequested: {
            startDate: moment().subtract(30, "days"),
            endDate: moment(),
            opts: {
                locale: {
                    applyClass: 'btn-green',
                    applyLabel: "Apply",
                    fromLabel: "From",
                    format: "DD/MM/YYYY",
                    toLabel: "To",
                    cancelLabel: 'Cancel',
                    customRangeLabel: 'Select date range'
                },
                ranges: {
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'Last 90 Days': [moment().subtract(89, 'days'), moment()]
                }
            }
        },
        type: 'AbsentReport'
    };

    $scope.search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveStaffReportSummary;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            Type: $scope.searchValue.type
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                $scope.reports = data.data;
                initReportList($scope.reports);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Report Management', data);
                $scope.loading = false;
            });

    };

    var initReportList = function(reports) {

        $ReportList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#ReportList')) {

            var table = $ReportList.DataTable();
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

            var table = $ReportList.DataTable({
                processing: false,
                data: reports,
                columns: [
                    { "data": "StaffName" },
                    { "data": "AbsentTimes" },
                    {
                        "className": 'list',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="Report Details" class = "fa fa-bars themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [0, "desc"]
                ],
                dom: 'frtip'
            });

            $ReportList.find('tbody').on('click', 'td.list', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $scope.viewdetails(data);
            });

            $ReportList.find('tbody').on('click', 'td.viewchart', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $scope.viewchart(data);
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

    $scope.viewdetails = function(data) {

        var modalInstance = $modal.open({
            templateUrl: 'views/report/staffreportmodal.html',
            controller: 'StaffReportModalCtrl',
            size: 'lg',
            resolve: {
                reportDto: function() {
                    return data;
                }
            }
        });

    };

    var formatDate = function(date) {

        var day = date.getDate(); // yields date
        var month = date.getMonth() + 1; // yields month (add one as '.getMonth()' is zero indexed)
        var year = date.getFullYear(); // yields year
        var hour = date.getHours(); // yields hours 
        var minute = date.getMinutes(); // yields minutes
        var second = date.getSeconds();

        var dateUtil = { Day: day, Month: month, Year: year };
        return dateUtil;

    };

});