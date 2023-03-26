app.controller('ApproveOverTimeCtrl', function($scope, configFileService, $modal, $sessionStorage) {

    var $overTimeList = $('#overTimeList');

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
        status: 'Pending'
    };

    $scope.search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveOverTimeForApproval;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            Status: $scope.searchValue.status,
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initOverTimeList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Shift', data);
                $scope.loading = false;
            });

    };

    var initOverTimeList = function(overTimes) {

        $overTimeList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#overTimeList')) {

            var table = $overTimeList.DataTable();
            table.clear().draw();
            table.rows.add(overTimes);
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

            var table = $overTimeList.DataTable({
                processing: false,
                data: overTimes,
                columns: [
                    { "data": "StaffName" },
                    { "data": "Date" },
                    { "data": "From" },
                    { "data": "To" },
                    { "data": "OverTimeData" },
                    { "data": "Room.Location.Name" },
                    { "data": "RequestedOn" },
                    { "data": "ApprovalStatus" },
                    {
                        "className": 'approve',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="Approve Over Time" class = "fa fa-check-square-o themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [2, "desc"]
                ],
                dom: 'frtip'
            });

            $overTimeList.find('tbody').on('click', 'td.approve', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $scope.approveordecline(data);
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

    $scope.approveordecline = function(data) {

        var modalInstance = $modal.open({
            templateUrl: 'views/clockin/modal.html',
            controller: 'OverTimeModalCtrl',
            resolve: {
                overTimeDto: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(overTime) {

            $scope.loading = true;

            if (overTime.update) {
                overTime.ApprovalStatus = 'Approved';
                configFileService
                    .put(configFileService.apiHandler.approveOrDeclineOverTime, overTime)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Shift', data);
                        $scope.loading = false;
                        $scope.search();
                    })
                    .error(function(data, status, headers) {
                        overTime.ApprovalStatus = 'Pending';
                        configFileService.displayMessage('info', 'Shift', data);
                        $scope.loading = false;
                    });
            } else {
                overTime.ApprovalStatus = 'Declined';
                configFileService
                    .put(configFileService.apiHandler.approveOrDeclineOverTime, overTime)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Shift', data);
                        $scope.loading = false;
                        $scope.search();
                    })
                    .error(function(data, status, headers) {
                        overTime.ApprovalStatus = 'Pending';
                        configFileService.displayMessage('info', 'Shift', data);
                        $scope.loading = false;
                    });
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