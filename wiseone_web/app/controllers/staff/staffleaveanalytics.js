'use strict';

app.controller('StaffLeaveAnalyticsCtrl', function($sessionStorage, $scope, $window, configFileService, confirmService, $state) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.staff = $sessionStorage.transferData.data;
    $scope.financialYears = [];
    $scope.financialYear = {};
    $scope.financialYearExists = false;
    $scope.financialYearFetched = false;
    $scope.leaveDetailsFetched = false;
    $scope.info = `Loading leave calendars for ${$scope.staff.Location.Name}`;
    $scope.leaveDetailsInfo = `Loading staff leave details`;
    $scope.staffLeave = {};
    $scope.date = {};
    $scope.numberOfNonLeaveTaken = 0;
    $scope.numberOfLeaveTaken = 0;
    $scope.numberOfLeaveRemaining = 0;

    $scope.init = function() {
        $scope.date = moment().format('LLLL');
        $scope.getLocationFinancialYear();
    }

    $scope.getLocationFinancialYear = function() {

        var uri = configFileService.apiHandler.retrieveLocationFinancialYears + '?locationId=' + $scope.staff.Location.ID;
        configFileService
            .get(uri)
            .success(function(data, status, headers) {

                $scope.financialYearFetched = true;
                $scope.financialYears = data.data;

                if (_.size($scope.financialYears) > 0) {

                    _.forEach($scope.financialYears, function(financialYear) {
                        financialYear.RawStartDate = moment(financialYear.StartDate, ['MM/DD/YYYY', 'DD/MM/YYYY']);
                    });
                    $scope.financialYears = _.orderBy($scope.financialYears, ['RawStartDate'], ['asc']);

                    $scope.financialYear = _.find($scope.financialYears, function(year) {
                        return _.isEqual(year.Status, 'Opened');
                    });
                } else {
                    $scope.financialYear = undefined;
                }

                if (!$scope.financialYear) {
                    $scope.financialYearExists = false;
                    $scope.info = `No leave calender found for location: ${$scope.staff.Location.Name}.`;
                } else {
                    $scope.financialYearExists = true;
                    $scope.getStaffLeaves($scope.financialYear);
                }
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
                $scope.loading = false;
            });

    };

    $scope.getStaffLeaves = function(financialYear) {

        $scope.leaveDetailsFetched = false;
        $scope.financialYear = financialYear;

        configFileService
            .get(`${configFileService.apiHandler.retrieveStaffLeavesByFinancialYear}?staffID=${$scope.staff.ID}&locationID=${$scope.staff.Location.ID}&financialYearID=${$scope.financialYear.ID}`)
            .success(function(data, status, headers) {
                $scope.leaveDetailsFetched = true;
                $scope.staffLeave = data;
                $scope.numberOfNonLeaveTaken = $scope.staffLeave.NumberOfNonLeaveTaken;
                $scope.numberOfLeaveTaken = $scope.staffLeave.NumberOfLeaveTaken;
                $scope.numberOfLeaveRemaining = $scope.staffLeave.NumberOfLeaveRemaining;

                $scope.setDatePicker();
                $scope.plotChart($scope.staffLeave);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
            });

    };

    $scope.setDatePicker = function() {
        $scope.searchValue = {
            dateRequested: {
                startDate: moment($scope.financialYear.StartDate, ['MM/DD/YYYY', 'DD/MM/YYYY']),
                endDate: moment($scope.financialYear.EndDate, ['MM/DD/YYYY', 'DD/MM/YYYY']),
                opts: {
                    minDate: moment($scope.financialYear.StartDate, ['MM/DD/YYYY', 'DD/MM/YYYY']),
                    maxDate: moment($scope.financialYear.EndDate, ['MM/DD/YYYY', 'DD/MM/YYYY']),
                    locale: {
                        applyClass: 'btn-green',
                        applyLabel: "Apply",
                        fromLabel: "From",
                        format: "DD/MM/YYYY",
                        toLabel: "To",
                        cancelLabel: 'Cancel',
                        customRangeLabel: 'Select date range'
                    },
                    eventHandlers: {
                        'apply.daterangepicker': function() {
                            $scope.filterStaffLeave();
                        }
                    }
                }
            },
            status: 'Pending'
        };

    };

    $scope.filterStaffLeave = function() {

        var staffLeave = angular.copy($scope.staffLeave),
            startDate = $scope.searchValue.dateRequested.startDate,
            endDate = $scope.searchValue.dateRequested.endDate;

        _.forEach(staffLeave.StaffLeave, function(sl, i) {

            var validRequestedDays = [];

            _.forEach(sl.RequestedDays, function(requestedDay) {

                var month = getMonthNo(requestedDay.Month);
                var date = moment(`${requestedDay.Year}-${month.toString()}-${requestedDay.Day.toString()}`, ["MM-DD-YYYY", "YYYY-MM-DD"]);

                if (startDate.diff(date) <= 0 && endDate.diff(date) >= 0) {
                    validRequestedDays.push(requestedDay);
                }

            });

            var noOfValidRequestedDays = _.size(validRequestedDays);
            if (noOfValidRequestedDays > 0) {
                sl.NumberOfLeaveDaysTaken = noOfValidRequestedDays;
                sl.RequestedDays = validRequestedDays;
            } else {
                sl.ToDelete = true;
            }

        });

        _.remove(staffLeave.StaffLeave, { ToDelete: true });

        var deductibleLeaves = _.filter(staffLeave.StaffLeave, { LeaveIsDeductible: true });
        var taken = 0;
        _.forEach(deductibleLeaves, function(leave) {
            taken += _.size(leave.RequestedDays);
        });

        var nonDeductibleLeaves = _.filter(staffLeave.StaffLeave, { LeaveIsDeductible: false });
        var nonTaken = 0;
        _.forEach(nonDeductibleLeaves, function(leave) {
            nonTaken += _.size(leave.RequestedDays);
        });

        staffLeave.NumberOfNonLeaveTaken = $scope.numberOfNonLeaveTaken = nonTaken;
        staffLeave.NumberOfLeaveTaken = $scope.numberOfLeaveTaken = taken;
        $scope.numberOfLeaveRemaining = $scope.staff.NumberOfLeaveDays - taken - $scope.staffLeave.BankHolidays.length;
        $scope.plotChart(staffLeave);

    }

    $scope.plotChart = function(staffLeave) {

        var staffLeaves = [];
        var staffLeavesPercentage = [];

        var leaveTotal = staffLeave.NumberOfNonLeaveTaken + staffLeave.NumberOfLeaveTaken;
        var deductibleLeaveDays = leaveTotal != 0 ? parseInt((staffLeave.NumberOfLeaveTaken / leaveTotal) * 100) : 0;
        var nonDeductibleLeaveDays = leaveTotal != 0 ? parseInt((staffLeave.NumberOfNonLeaveTaken / leaveTotal) * 100) : 0;

        _.forEach(staffLeave.StaffLeave, function(s, skey) {

            var leave = _.find(staffLeaves, { name: s.LeaveCriteria });

            if (!leave) {

                staffLeaves.push({
                    name: s.LeaveCriteria,
                    label: `${s.LeaveCriteria} - ${ s.NumberOfLeaveDaysTaken} days`,
                    value: s.NumberOfLeaveDaysTaken
                });

                var percent = parseInt((s.NumberOfLeaveDaysTaken / leaveTotal) * 100);

                staffLeavesPercentage.push({
                    name: s.LeaveCriteria,
                    label: `${s.LeaveCriteria} - ${percent}%`,
                    value: percent,
                    tempValue: s.NumberOfLeaveDaysTaken
                });

            } else {

                var leaveIdx = _.findIndex(staffLeaves, { name: s.LeaveCriteria });
                staffLeaves[leaveIdx].value += s.NumberOfLeaveDaysTaken;
                staffLeaves[leaveIdx].label = `${ staffLeaves[leaveIdx].name} - ${ staffLeaves[leaveIdx].value} days`;

                staffLeavesPercentage[leaveIdx].tempValue += s.NumberOfLeaveDaysTaken;
                staffLeavesPercentage[leaveIdx].value = parseInt((staffLeavesPercentage[leaveIdx].tempValue / leaveTotal) * 100);
                staffLeavesPercentage[leaveIdx].label = `${ staffLeavesPercentage[leaveIdx].name} - ${ staffLeavesPercentage[leaveIdx].value}%`;

            }
        });

        $scope.data = staffLeaves;
        $scope.dataPercentage = staffLeavesPercentage;
        $scope.dataCompare = [
            { label: `Non Deductible - ${nonDeductibleLeaveDays}%`, value: nonDeductibleLeaveDays },
            { label: `Deductible - ${deductibleLeaveDays}%`, value: deductibleLeaveDays }
        ];

        $scope.options = {
            chart: {
                type: 'pieChart',
                height: 500,
                x: function(d) { return d.label; },
                y: function(d) { return d.value; },
                showLabels: true,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        };

    }

    $scope.export = function() {
        /*  html2canvas(document.getElementById('exportthis')).then(function(canvas) {
             var data = canvas.toDataURL();
             var docDefinition = {
                 pageSize: 'A3',
                 content: [{
                     image: data,
                     width: 750,
                 }]
             };
             pdfMake.createPdf(docDefinition).download(`${$scope.staff.Name} - Leave Details.pdf`);
         }); */

        window.print();
    }

    $scope.goBack = function() {
        $window.history.back();
    }

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

    function getRandomColor() {
        var lum = -0.25;
        var hex = String('#' + Math.random().toString(16).slice(2, 8).toUpperCase()).replace(/[^0-9a-f]/gi, '');
        if (hex.length < 6) {
            hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
        }
        var rgb = "#",
            c, i;
        for (i = 0; i < 3; i++) {
            c = parseInt(hex.substr(i * 2, 2), 16);
            c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
            rgb += ("00" + c).substr(c.length);
        }
        return rgb;
    }

    function getMonthNo(month) {

        var monthNo = 0;

        switch (month) {
            case 'January':
                monthNo = 1;
                break;
            case 'February':
                monthNo = 2;
                break;
            case 'March':
                monthNo = 3;
                break;
            case 'April':
                monthNo = 4;
                break;
            case 'May':
                monthNo = 5;
                break;
            case 'June':
                monthNo = 6;
                break;
            case 'July':
                monthNo = 7;
                break;
            case 'August':
                monthNo = 8;
                break;
            case 'September':
                monthNo = 9;
                break;
            case 'October':
                monthNo = 10;
                break;
            case 'November':
                monthNo = 11;
                break;
            case 'December':
                monthNo = 12;
                break;
        }

        return monthNo;
    }

});