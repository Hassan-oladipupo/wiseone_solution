app.controller('CancelLeaveCtrl', function($scope, configFileService, $modal, $sessionStorage) {

    var $leaveList = $('#leaveList');

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

        var uri = configFileService.apiHandler.retrieveCancelLeaveRequests;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            Status: $scope.searchValue.status,
            LocationID: $sessionStorage.wiseOneUser.Location.ID,
            StaffID: $sessionStorage.wiseOneUser.ID
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initLeaveList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
                $scope.loading = false;
            });

    };

    var initLeaveList = function(leaveRequests) {

        $leaveList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#leaveList')) {

            var table = $leaveList.DataTable();
            table.clear().draw();
            table.rows.add(leaveRequests);
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

            var table = $leaveList.DataTable({
                processing: false,
                data: leaveRequests,
                columns: [
                    { "data": "StaffName" },
                    { "data": "LeaveTypeDescription" },
                    { "data": "NumberOfLeaveDaysTaken" },
                    { "data": "LocationName" },
                    { "data": "Approval.RequestedOn" },
                    { "data": "Approval.ApprovalStatus" },
                    {
                        "className": 'approve',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="Approve Cancel Leave Request" class = "fa fa-check-square-o themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [2, "desc"]
                ],
                dom: 'frtip'
            });

            $leaveList.find('tbody').on('click', 'td.approve', function() {
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
            backdrop: false,
            keyboard: true,
            templateUrl: 'views/leave/cancelleavemodal.html',
            controller: 'CancelLeaveModalCtrl',
            resolve: {
                leaveDto: function() {
                    return data;
                }
            }
        });

        modalInstance.result.then(function(leaveRequest) {

            $scope.loading = true;

            if (leaveRequest.update) {

                configFileService
                    .put(configFileService.apiHandler.approveCancelLeaveRequest, leaveRequest)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Leave Management', data);
                        $scope.loading = false;
                        $scope.search();
                    })
                    .error(function(data, status, headers) {
                        configFileService.displayMessage('info', 'Leave Management', data);
                        $scope.loading = false;
                    });
            } else {

                configFileService
                    .put(configFileService.apiHandler.declineCancelLeaveRequest, leaveRequest)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Leave Management', data);
                        $scope.loading = false;
                        $scope.search();
                    })
                    .error(function(data, status, headers) {
                        configFileService.displayMessage('info', 'Leave Management', data);
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