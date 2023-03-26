app.controller('ViewClockInCtrl', function($scope, configFileService, $modal, $sessionStorage, $state) {

    var $clockInList = $('#clockInList');
    $scope.location = "";

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
        }
    };

    $scope.init = function() {

        $scope.disabled = true;
        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In Monitor', data);
            });

        $scope.search();
    };

    $scope.search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveSignInOuts;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            LocationID: $scope.location != "" ? $scope.location.ID : 0
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initClockInList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Clock In', data);
                $scope.loading = false;
            });

    };

    var initClockInList = function(clockIns) {

        $clockInList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#clockInList')) {

            var table = $clockInList.DataTable();
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

            var table = $clockInList.DataTable({
                processing: false,
                data: clockIns,
                columns: [
                    { "data": "Staff.StaffID" },
                    { "data": "Staff.Name" },
                    { "data": "Staff.Role.Name" },
                    { "data": "ExpectedTotalHours" },
                    { "data": "ActualTotalHours" },
                    { "data": "OverTimeTotalHours" },
                    {
                        "className": 'view',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="View Staff Clock In Details" class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [2, "desc"]
                ],
                dom: 'Bfrtip',
                buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5']
            });

            $clockInList.find('tbody').on('click', 'td.view', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $sessionStorage.transferData = {
                    data: data,
                    startDate: formatDate($scope.searchValue.dateRequested.startDate.toDate()),
                    endDate: formatDate($scope.searchValue.dateRequested.endDate.toDate())
                };

                $state.go('app.clockindetails');
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