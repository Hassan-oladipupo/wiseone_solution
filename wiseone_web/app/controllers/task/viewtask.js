app.controller('ViewTaskCtrl', function($scope, configFileService, $modal, $sessionStorage, $state) {

    var $taskList = $('#taskList');

    $scope.searchValue = {
        dateRequested: {
            startDate: moment().subtract(30, "days"),
            endDate: moment(),
            status: '',
            staffName: '',
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
    };

    $scope.init = function() {
        search();
    };

    $scope.searchTasks = function() {
        search();
    };

    var search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveTasks;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate,
            Status: $scope.searchValue.status,
            StaffName: $scope.searchValue.staffName
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initTaskList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
                $scope.loading = false;
            });

    };

    var initTaskList = function(tasks) {

        $taskList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#taskList')) {

            var table = $taskList.DataTable();
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

            var table = $taskList.DataTable({
                stateSave: false,
                processing: false,
                data: tasks,
                columns: [
                    { "data": "Subject" },
                    { "data": "Type" },
                    { "data": "TaskLeader" },
                    { "data": "DateofCompletionStr" },
                    { "data": "LastUpdatedBy" },
                    { "data": "LastUpdatedOn" },
                    { "data": "Status" },
                    {
                        "className": 'view',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="View Task Details" class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                    }
                ],
                order: [
                    [3, "desc"]
                ],
                dom: 'frtip'
            });

            $taskList.find('tbody').on('click', 'td.view', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $sessionStorage.transferData = {
                    edit: false,
                    data: data
                };

                $state.go('app.taskdetail');
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