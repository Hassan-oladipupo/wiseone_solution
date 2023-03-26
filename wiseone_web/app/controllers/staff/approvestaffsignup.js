app.controller('ApproveStaffSignUpCtrl', function($scope, configFileService, $modal) {

    var $staffList = $('#staffList');

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

    $scope.init = function() {
        search();
    };

    $scope.searchSignUp = function() {
        search();
    };

    var search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveSignUpRequests;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            Status: $scope.searchValue.status
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initStaffList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Staff Management', data);
                $scope.loading = false;
            });

    };

    var initStaffList = function(tasks) {

        $staffList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#staffList')) {

            var table = $staffList.DataTable();
            table.clear().draw();
            table.rows.add(tasks);
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

            var table = $staffList.DataTable({
                processing: false,
                data: tasks,
                columnDefs: [{
                    "targets": [0],
                    "data": "description",
                    "render": function(data, type, full, meta) {
                        return '<img src="' + data + '" alt ="images" class="image-icon" />';
                    }
                }],
                columns: [{
                        "data": "Picture",
                        "orderable": false
                    },
                    { "data": "Surname" },
                    { "data": "FirstName" },
                    { "data": "MiddleName" },
                    { "data": "Username" },
                    { "data": "Location.Name" },
                    { "data": "Role.Name" },
                    { "data": "RequestedOn" },
                    { "data": "Status" },
                    {
                        "className": 'approveStaffSignUp',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="Approve Staff Sign Up" class = "fa fa-check-square-o themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [7, "desc"]
                ],
                dom: 'Bfrtip',
                buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', {
                    extend: 'pdfHtml5',
                    orientation: 'landscape',
                    pageSize: 'A4'
                }]
            });

            $staffList.find('tbody').on('click', 'td.approveStaffSignUp', function() {
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

        var modalInstance = {};

        if (data.ExistingStaff) {

            modalInstance = $modal.open({
                templateUrl: 'views/staff/existingstaffmodal.html',
                controller: 'ExistingStaffCtrl',
                resolve: {
                    staffDto: function() {
                        return data;
                    }
                }
            });

        } else {

            modalInstance = $modal.open({
                templateUrl: 'views/staff/approvalmodal.html',
                controller: 'ApprovalModalCtrl',
                resolve: {
                    staffDto: function() {
                        return data;
                    }
                }
            });

        }

        modalInstance.result.then(function(staff) {

            $scope.loading = true;

            if (staff.update) {

                if (staff.StartDate && !isValidDate(staff.StartDate)) {
                    configFileService.displayMessage('info', 'Staff Management', 'Invalid start date entered. Date format is DD/MM/YYYY');
                    return
                }

                if (staff.EndDate && !isValidDate(staff.EndDate)) {
                    configFileService.displayMessage('info', 'Staff Management', 'Invalid end date entered. Date format is DD/MM/YYYY');
                    return
                }

                if (!_.isEmpty(staff.StaffID) && !_.isEmpty(staff.OfficialEmail) && !_.isEmpty(staff.EmploymentType) && staff.NumberOfLeaveDays != 0) {
                    staff.Status = 'Active';
                    configFileService
                        .put(configFileService.apiHandler.approveStaffSignUp, staff)
                        .success(function(data, status, headers) {
                            configFileService.displayMessage('success', 'Staff Management', data);
                            $scope.loading = false;
                            search();
                        })
                        .error(function(data, status, headers) {
                            configFileService.displayMessage('info', 'Staff Management', data);
                            $scope.loading = false;
                        });
                } else {
                    configFileService.displayMessage('info', 'Staff Management', 'Staff ID, Official Email, Employment Type and Number of Leave Days are required for approval.');
                    $scope.loading = false;
                }
            } else {
                configFileService
                    .put(configFileService.apiHandler.declineStaffSignUp, staff)
                    .success(function(data, status, headers) {
                        configFileService.displayMessage('success', 'Staff Management', data);
                        $scope.loading = false;
                        search();
                    })
                    .error(function(data, status, headers) {
                        configFileService.displayMessage('info', 'Staff Management', data);
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

    function isValidDate(testdate) {
        var isValid = false;
        var date_regex = /^\d{2}\/\d{2}\/\d{4}$/;
        let validDate = date_regex.test(testdate);
        if (validDate) {
            let testDates = testdate.split('/');
            let day = parseInt(testDates[0]);
            let month = parseInt(testDates[1]);
            if (day <= 31 && month <= 12) {
                isValid = true;
            }
        }
        return isValid;
    }

});