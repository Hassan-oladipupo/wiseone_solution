app.controller('ViewShiftCtrl', function($scope, configFileService, $modal, $sessionStorage, $state) {

    var $shiftList = $('#shiftList');

    $scope.searchValue = {
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
    };

    $scope.init = function() {
        search();
    };

    $scope.searchShifts = function() {
        search();
    };

    var search = function() {

        var startDate = formatDate($scope.searchValue.dateRequested.startDate.toDate());
        var endDate = formatDate($scope.searchValue.dateRequested.endDate.toDate());

        var uri = configFileService.apiHandler.retrieveShiftConfigurations;

        var searchData = {
            FromDate: startDate,
            ToDate: endDate
        };

        $scope.loading = true;
        configFileService
            .post(uri, searchData)
            .success(function(data, status, headers) {
                $scope.loading = false;
                initShiftList(data.data);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Shift Management', data);
                $scope.loading = false;
            });

    };

    var initShiftList = function(shifts) {

        $shiftList.find('tfoot').find('th').each(function() {
            var title = $(this).text();
            if (title != "")
                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        });

        if ($.fn.DataTable.isDataTable('#shiftList')) {

            var table = $shiftList.DataTable();
            table.clear().draw();
            table.rows.add(shifts);
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

            var table = $shiftList.DataTable({
                processing: false,
                data: shifts,
                columns: [
                    { "data": "StartDateStr" },
                    { "data": "EndDateStr" },
                    { "data": "WeekName" },
                    { "data": "Location.Name" },
                    { "data": "CreatedBy" },
                    { "data": "CreatedOn" },
                    { "data": "LastUpdatedOn" },
                    {
                        "className": 'view',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i title="View Shift Details" class = "fa fa-list themeprimary bordered-themeprimary dt-icon"></i>'
                    },
                    {
                        "data": "ID",
                        "visible": false
                    },
                ],
                order: [
                    [8, "desc"],
                    [3, "asc"]
                ],
                dom: 'Bfrtip',
                buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', {
                    extend: 'pdfHtml5',
                    orientation: 'landscape',
                    pageSize: 'A4'
                }]
            });

            $shiftList.find('tbody').on('click', 'td.view', function() {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var data = row.data();

                $sessionStorage.transferData = {
                    edit: false,
                    data: data
                };

                $state.go('app.shiftdetail');
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