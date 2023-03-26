app.controller('CreateShiftCtrl', function($scope, configFileService, confirmService, $modal, $sessionStorage, WizardHandler) {

    $scope.init = function() {

        $scope.leave = new leavedto();
        $scope.financialYear = $scope.leave.financialYear;

        $scope.disabled = true;
        $scope.financialYear.Location = new locationdto();

        configFileService
            .get(configFileService.apiHandler.retrieveActiveLocations)
            .success(function(data, status, headers) {
                $scope.Locations = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Leave Management', data);
            });

    };

    $scope.getMonths = function(month) {
        var months = [
            { text: 'JANUARY' },
            { text: 'FEBRUARY' },
            { text: 'MARCH' },
            { text: 'APRIL' },
            { text: 'MAY' },
            { text: 'JUNE' },
            { text: 'JULY' },
            { text: 'AUGUST' },
            { text: 'SEPTEMBER' },
            { text: 'OCTOBER' },
            { text: 'NOVEMBER' },
            { text: 'DECEMBER' }
        ];

        return _.filter(months, function(m, mkey) {
            return _.includes(m.text, month.toUpperCase());
        });
    };

    $scope.$watch('financialYear', function(financialYear) {

        var sd = financialYear.startDate.format('YYYY-MM-DD');
        var ed = financialYear.endDate.format('YYYY-MM-DD');

        if (!_.isEqual(sd, ed)) {

            var months = [];
            var month = {};
            var startDate = angular.copy(financialYear.startDate);
            var dateStart = angular.copy(financialYear.startDate);
            var dateEnd = angular.copy(financialYear.endDate);
            var dn = {};
            var c = '';
            var day = {};

            while (dateEnd > dateStart) {

                var days = [];
                var numberOfDays = moment(dateStart.format('YYYY-MM')).daysInMonth();
                var counter = 1;

                var d = startDate.format('YYYY-MM');
                var s = dateStart.format('YYYY-MM');
                var e = dateEnd.format('YYYY-MM');

                if (_.isEqual(s, d)) {

                    dn = startDate.format('DD');

                    while (counter <= numberOfDays) {

                        c = _.padStart(counter.toString(), '2', '0');
                        day = {
                            Available: counter < parseInt(dn) ? false : true,
                            Day: counter,
                            Name: moment(dateStart.format('YYYY-MM') + '-' + c).format('ddd'),
                            BankHoliday: false
                        };
                        days.push(day);

                        counter++;

                    }

                    month = {
                        Month: dateStart.format('MMMM'),
                        Year: dateStart.format('YYYY'),
                        Days: days,
                        Exclude: false,
                        Label: dateStart.format('MMMM') + ' - ' + dateStart.format('YYYY')
                    };

                } else if (_.isEqual(s, e)) {

                    dn = dateEnd.format('DD');

                    while (counter <= numberOfDays) {

                        c = _.padStart(counter.toString(), '2', '0');
                        day = {
                            Available: counter > parseInt(dn) ? false : true,
                            Day: counter,
                            Name: moment(dateStart.format('YYYY-MM') + '-' + c).format('ddd'),
                            BankHoliday: false
                        };
                        days.push(day);

                        counter++;

                    }

                    month = {
                        Month: dateStart.format('MMMM'),
                        Year: dateStart.format('YYYY'),
                        Days: days,
                        Exclude: false,
                        Label: dateStart.format('MMMM') + ' - ' + dateStart.format('YYYY')
                    };

                } else {

                    while (counter <= numberOfDays) {

                        c = _.padStart(counter.toString(), '2', '0');
                        day = {
                            Available: true,
                            Day: counter,
                            Name: moment(dateStart.format('YYYY-MM') + '-' + c).format('ddd'),
                            BankHoliday: false
                        };
                        days.push(day);

                        counter++;

                    }

                    month = {
                        Month: dateStart.format('MMMM'),
                        Year: dateStart.format('YYYY'),
                        Days: days,
                        Exclude: false,
                        Label: dateStart.format('MMMM') + ' - ' + dateStart.format('YYYY')
                    };

                }

                var checkMonth = _.size(_.filter(month.Days, { Available: true }));
                if (checkMonth > 0) {
                    months.push(month);
                }

                dateStart.add(1, 'month');
            }

            $scope.financialYear.months = months;
            $scope.financialYear.selectedMonth = months[0];
        }

    }, false);

    $scope.save = function() {

        if (!_.isEmpty($scope.financialYear.months)) {

            if ($scope.financialYear.Location != undefined && $scope.financialYear.Location.ID != 0) {

                var fyear = $scope.financialYear.startDate.format('YYYY-MM-DD') + ' - ' + $scope.financialYear.endDate.format('YYYY-MM-DD');

                var modalOptions = {
                    headerText: 'Leave Configuration: ' + fyear + '?',
                    bodyText: 'Are you sure you want to save the configuration for this financial year? Once saved, modifications will only be available for prior notice of leave.'
                };

                confirmService.showModal({}, modalOptions).then(function() {

                    _.forEach($scope.financialYear.excludeMonths, function(em, emKey) {
                        _.forEach($scope.financialYear.months, function(m, mkey) {
                            if (_.isEqual(m.Month.toUpperCase(), em.text.toUpperCase())) {
                                $scope.financialYear.months[mkey].Exclude = true;
                            }
                        });
                    });

                    var fy = {
                        EndDate: $scope.financialYear.endDate.format('YYYY-MM-DD'),
                        ExcludeMonths: $scope.financialYear.excludeMonths,
                        LeavePriorNotice: $scope.financialYear.LeavePriorNotice,
                        StartDate: $scope.financialYear.startDate.format('YYYY-MM-DD'),
                        Location: {
                            ID: $scope.financialYear.Location.ID
                        },
                        Months: $scope.financialYear.months
                    };

                    $scope.loading = true;

                    configFileService
                        .post(configFileService.apiHandler.saveFinancialYear, fy)
                        .success(function(data, status, headers) {
                            configFileService.displayMessage('success', 'Leave Management', data);
                            $scope.loading = false;
                            $scope.reset();
                        })
                        .error(function(data, status, headers) {
                            configFileService.displayMessage('info', 'Leave Management', data);
                            $scope.loading = false;
                        });

                });
            } else {
                configFileService.displayMessage('info', 'Leave Management', 'Location is required.');
            }
        } else {
            configFileService.displayMessage('info', 'Leave Management', 'There are no months configured');
        }
    };

    $scope.clear = function() {
        $scope.leave = new leavedto();
        $scope.financialYear = $scope.leave.financialYear;
    };

    $scope.reset = function() {
        $scope.financialYear.Location = new locationdto();

        var wizard = WizardHandler.wizard();
        if (wizard) {
            wizard.goTo(0);
        }
    };


    $scope.removeExcludeMonths = function() {

        if (!_.isEmpty($scope.financialYear.months)) {
            _.forEach($scope.financialYear.excludeMonths, function(em, emKey) {

                var idx = _.findIndex($scope.financialYear.months, function(m, mkey) {
                    return _.isEqual(m.Month.toUpperCase(), em.text);
                });

                if (idx != -1) {
                    $scope.financialYear.months[idx].Exclude = true;
                }
            });
        } else {
            configFileService.displayMessage('info', 'Leave Management', 'Financial year date range is required.');
        }
    }

    $scope.showEntries = function() {

        $scope.dateRange = 'Between ' + $scope.financialYear.startDate.format('MMMM Do YYYY') + ' and ' + $scope.financialYear.endDate.format('MMMM Do YYYY');

        $scope.bankHolidays = [];

        _.forEach($scope.financialYear.months, function(m, mkey) {
            _.forEach(m.Days, function(d, dkey) {
                if (d.BankHoliday) {
                    var holiday = d.Name + ' ' + d.Day + ', ' + m.Month + ' ' + m.Year;
                    $scope.bankHolidays.push(holiday);
                }
            });
        });
    }

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