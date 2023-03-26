app.controller('ShiftDetailCtrl', function($sessionStorage, $scope, $window, configFileService, confirmService) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.shift = $sessionStorage.transferData.data;

    $scope.edit = $sessionStorage.transferData.edit;

    $scope.utility = {
        locations: [],
        locationsRetrieved: false,
        staffLocation: {},
        staffs: [],
        staffsRetrieved: false,
        locationRooms: [],
        selectedStaffs: [],
        shiftdays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
        weekNo: 0,
        searchText: ''
    };

    $scope.shiftdate = {
        startDate: moment($scope.shift.StartDateStr, "DD/MM/YYYY"),
        endDate: moment($scope.shift.EndDateStr, "DD/MM/YYYY"),
        opts: {
            minDate: moment($scope.shift.StartDateStr, "DD/MM/YYYY"),
            maxDate: moment($scope.shift.StartDateStr, "DD/MM/YYYY").add(4, "days"),
            showWeekNumbers: true,
            dateLimit: {
                days: '4'
            },
            locale: {
                applyClass: 'btn-success',
                applyLabel: "Apply",
                fromLabel: "From",
                format: "DD/MM/YYYY",
                toLabel: "To",
                cancelLabel: 'Cancel',
            },
        }
    };

    $scope.init = function() {
        getRoomStaffLocation();
    };

    function getRoomStaffLocation() {
        configFileService
            .get(configFileService.apiHandler.retrieveLocationRoomStaffLocation + '?locationId=' + $scope.shift.Location.ID)
            .success(function(data, status, headers) {
                $scope.utility.locationRooms = data.Rooms;
                getActiveStaffs(data.Staffs);
                getLocations(data.Locations);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Shift Management', data);
            });
    }

    function getActiveStaffs(staffList) {

        _.forEach(staffList, function(s, skey) {

            var staff = {};

            var staffExists = _.find($scope.shift.StaffShifts, { StaffID: s.ID });
            if (staffExists) {

                staff.ID = staffExists.ID;
                staff.StaffID = staffExists.StaffID;
                staff.ShiftConfigured = true;
                staff.AutoUpdate = false;
                staff.KnownAs = s.KnownAs;
                staff.Name = s.FirstName + ' ' + s.Surname;
                staff.Username = s.Username;
                staff.Email = s.Email;
                staff.Telephone = s.Telephone;
                staff.Leave = s.Leave;
                staff.Shift = staffExists.Shift;
                staff.Location = s.Location;
                staff.Existing = true;

            } else {

                staff = {
                    StaffID: s.ID,
                    ShiftConfigured: false,
                    AutoUpdate: false,
                    KnownAs: s.KnownAs,
                    Name: s.FirstName + ' ' + s.Surname,
                    Username: s.Username,
                    Email: s.Email,
                    Telephone: s.Telephone,
                    Leave: s.Leave,
                    Shift: [],
                    Location: s.Location,
                    Existing: false
                };

                _.forEach($scope.utility.shiftdays, function(day, dkey) {
                    var date = moment().day(day).week($scope.utility.weekNo).format('DD/MM/YYYY');
                    staff.Shift.push({
                        Day: day,
                        Date: date,
                        Configure: false,
                        From: 7,
                        To: 19,
                        Room: null,
                        Done: false,
                        HasSupervision: false,
                        BreakTimeDuration: 30,
                        FolderTimeDuration: 0
                    });
                });
            }

            $scope.utility.staffs.push(staff);
        });
    }

    function getLocations(locationList) {
        $scope.utility.locations = locationList;
        $scope.utility.staffLocation = $scope.shift.Location;
        $scope.utility.locationsRetrieved = true;
        $scope.getStaff();
    }

    $scope.getStaff = function() {

        $scope.utility.selectedStaffs = _.filter($scope.utility.staffs, function(staff) {
            return _.isEqual(staff.Location.ID, $scope.utility.staffLocation.ID);
        });

        $scope.utility.searchText = '';
    };

    $scope.disableDaySelection = function(date) {
        if (moment() > moment(date, "DD/MM/YYYY"))
            return true;
        else
            return false;
    };

    $scope.deleteShift = function(s) {

        var modalOptions = {
            headerText: 'Delete Shift',
            bodyText: `Are you sure you want to delete the entire Shift set up for ${s.Name}?`
        };

        confirmService.showModal({}, modalOptions).then(function() {

            var staffShift = {
                ID: s.ID,
                StaffID: s.StaffID,
                StaffUsername: s.Username,
                StaffEmail: s.Email,
                StaffName: s.Name,
                StaffKnownAs: s.KnownAs,
                StaffTelephone: s.Telephone,
                Shift: s.Shift
            };

            $scope.loading = true;

            configFileService
                .post(configFileService.apiHandler.deleteStaffShiftConfiguration, staffShift)
                .success(function(data, status, headers) {

                    configFileService.displayMessage('success', 'Shift Management', data);
                    $scope.loading = false;
                    s.ShiftConfigured = false;
                    _.forEach(s.Shift, function(shift, shiftKey) {
                        s.Shift[shiftKey].Date = null;
                        s.Shift[shiftKey].Configure = true;
                        s.Shift[shiftKey].From = 7;
                        s.Shift[shiftKey].To = 19;
                        s.Shift[shiftKey].Room = null;
                        s.Shift[shiftKey].Done = false;
                        s.Shift[shiftKey].HasSupervision = false;
                        s.Shift[shiftKey].BreakTimeDuration = 30;
                        s.Shift[shiftKey].FolderTimeDuration = 0;
                    });

                })
                .error(function(data, status, headers) {

                    configFileService.displayMessage('info', 'Shift Management', data);
                    $scope.loading = false;

                });

        });

    };

    $scope.setSummary = function() {

        var endDate = formatStrDate($scope.shiftdate.endDate.toDate());
        var startDate = formatStrDate($scope.shiftdate.startDate.toDate());

        $scope.utility.staffOnShift = [];

        _.forEach($scope.utility.staffs, function(staff, skey) {

            var staffShift = {};

            if (staff.ShiftConfigured) {

                staffShift.Name = staff.Name;

                //======================Monday Shift=====================
                var mondayShift = _.find(staff.Shift, { Day: "Monday" });

                if (mondayShift && mondayShift.Configure) {

                    staffShift.MondayConfigured = true;

                    if (mondayShift.Room) {
                        var shiftFrom = getFormattedTime(mondayShift.From);
                        var shiftTo = getFormattedTime(mondayShift.To);
                        staffShift.Monday = `${shiftFrom} - ${shiftTo}`;
                        staffShift.MondayRoom = mondayShift.Room.Name;
                        staffShift.MondayRoomSelected = true;

                    } else {
                        staffShift.MondayRoomSelected = false;
                    }

                } else {
                    staffShift.MondayConfigured = false;
                }

                //======================Tuesday Shift======================
                var tuesdayShift = _.find(staff.Shift, { Day: "Tuesday" });

                if (tuesdayShift && tuesdayShift.Configure) {

                    staffShift.TuesdayConfigured = true;

                    if (tuesdayShift.Room) {

                        var shiftFrom = getFormattedTime(tuesdayShift.From);
                        var shiftTo = getFormattedTime(tuesdayShift.To);
                        staffShift.Tuesday = `${shiftFrom} - ${shiftTo}`;
                        staffShift.TuesdayRoom = tuesdayShift.Room.Name;
                        staffShift.TuesdayRoomSelected = true;

                    } else {
                        staffShift.TuesdayRoomSelected = false;
                    }

                } else {
                    staffShift.TuesdayConfigured = false;
                }

                //======================Wednesday Shift========================
                var wednesdayShift = _.find(staff.Shift, { Day: "Wednesday" });

                if (wednesdayShift && wednesdayShift.Configure) {

                    staffShift.WednesdayConfigured = true;

                    if (wednesdayShift.Room) {

                        var shiftFrom = getFormattedTime(wednesdayShift.From);
                        var shiftTo = getFormattedTime(wednesdayShift.To);
                        staffShift.Wednesday = `${shiftFrom} - ${shiftTo}`;
                        staffShift.WednesdayRoom = wednesdayShift.Room.Name;
                        staffShift.WednesdayRoomSelected = true;

                    } else {
                        staffShift.WednesdayRoomSelected = false;
                    }

                } else {
                    staffShift.WednesdayConfigured = false;
                }

                //======================Thursday Shift=======================
                var thursdayShift = _.find(staff.Shift, { Day: "Thursday" });

                if (thursdayShift && thursdayShift.Configure) {

                    staffShift.ThursdayConfigured = true;

                    if (thursdayShift.Room) {

                        var shiftFrom = getFormattedTime(thursdayShift.From);
                        var shiftTo = getFormattedTime(thursdayShift.To);
                        staffShift.Thursday = `${shiftFrom} - ${shiftTo}`;
                        staffShift.ThursdayRoom = thursdayShift.Room.Name;
                        staffShift.ThursdayRoomSelected = true;

                    } else {
                        staffShift.ThursdayRoomSelected = false;
                    }

                } else {
                    staffShift.ThursdayConfigured = false;
                }

                //======================Friday Shift=====================
                var fridayShift = _.find(staff.Shift, { Day: "Friday" });

                if (fridayShift && fridayShift.Configure) {

                    staffShift.FridayConfigured = true;

                    if (fridayShift.Room) {

                        var shiftFrom = getFormattedTime(fridayShift.From);
                        var shiftTo = getFormattedTime(fridayShift.To);
                        staffShift.Friday = `${shiftFrom} - ${shiftTo}`;
                        staffShift.FridayRoom = fridayShift.Room.Name;
                        staffShift.FridayRoomSelected = true;

                    } else {
                        staffShift.FridayRoomSelected = false;
                    }

                } else {
                    staffShift.FridayConfigured = false;
                }

                $scope.utility.staffOnShift.push(staffShift);
            }
        });


    };


    $scope.save = function() {

        var modalOptions = {
            headerText: 'Update Shift Confirmation',
            bodyText: `Are you sure you want to update the Shift for ${$scope.shift.Location.Name}?`
        };

        confirmService.showModal({}, modalOptions).then(function() {

            var shiftConfig = new shiftdto();
            var staffShifts = [];
            var shiftsWithoutRoom = [];

            _.forEach($scope.utility.staffs, function(staff, skey) {

                if (staff.ShiftConfigured) {

                    _.forEach(staff.Shift, function(shift, shiftKey) {

                        if (shift.Configure && !shift.Room) {
                            shiftsWithoutRoom.push(staff);
                        }

                        staff.Shift[shiftKey].Date = moment().day(staff.Shift[shiftKey].Day).week($scope.utility.weekNo).format('DD/MM/YYYY');

                    });

                    var staffShift = {
                        ID: staff.ID,
                        StaffID: staff.StaffID,
                        StaffUsername: staff.Username,
                        StaffEmail: staff.Email,
                        StaffName: staff.Name,
                        StaffKnownAs: staff.KnownAs,
                        StaffTelephone: staff.Telephone,
                        Shift: staff.Shift
                    };
                    staffShifts.push(staffShift);
                }

            });

            if (_.isEmpty(shiftsWithoutRoom)) {

                if (!_.isEmpty(staffShifts)) {

                    shiftConfig.ID = $scope.shift.ID;
                    shiftConfig.EndDate = formatDate($scope.shiftdate.endDate.toDate());
                    shiftConfig.StaffShifts = staffShifts;
                    shiftConfig.Location = $scope.shift.Location;
                    shiftConfig.StartDate = formatDate($scope.shiftdate.startDate.toDate());
                    shiftConfig.GeneralInformation = $scope.shift.GeneralInformation;

                    $scope.loading = true;
                    configFileService
                        .post(configFileService.apiHandler.updateShiftConfiguration, shiftConfig)
                        .success(function(data, status, headers) {
                            configFileService.displayMessage('success', 'Shift Management', data);
                            $scope.loading = false;
                        })
                        .error(function(data, status, headers) {
                            configFileService.displayMessage('info', 'Shift Management', data);
                            $scope.loading = false;
                        });

                } else {
                    configFileService.displayMessage('info', 'Shift Management', 'There are no staff shift to save.');
                }
            } else {
                var uniqStaffs = _.uniq(shiftsWithoutRoom);
                var staffToDisplay = '';
                _.forEach(uniqStaffs, function(s, skey) {
                    staffToDisplay += s.Name + ', ';
                });
                configFileService.displayMessage('info', 'Shift Management', 'Some staff have shift configured but have no shift room set for them for some of the days. Do check and try again. The staff are: ' + staffToDisplay);
            }

        });
    };

    $scope.replicateShift = function(staff, shift) {
        for (i = 0; i < staff.Shift.length; i++) {
            if (!_.isEqual(shift.Day, staff.Shift[i].Day)) {
                var disabled = $scope.disableDaySelection(staff.Shift[i].Date);
                if (staff.Shift[i].Configure && !disabled) {
                    staff.Shift[i].From = shift.From;
                    staff.Shift[i].To = shift.To;
                    staff.Shift[i].HasSupervision = shift.HasSupervision;
                    staff.Shift[i].FolderTimeDuration = shift.FolderTimeDuration;
                    staff.Shift[i].BreakTimeDuration = shift.BreakTimeDuration;
                    staff.Shift[i].Room = shift.Room;
                }
            }
        }
    };

    $scope.export = function() {

        var endDate = formatStrDate($scope.shiftdate.endDate.toDate());
        var startDate = formatStrDate($scope.shiftdate.startDate.toDate());

        var staffOnShift = [
            ['Staff', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
        ];

        _.forEach($scope.utility.staffs, function(staff, skey) {

            var staffShift = [];

            if (staff.ShiftConfigured) {

                staffShift.push(staff.Name);

                var mondayShift = _.find(staff.Shift, { Day: "Monday" });
                if (mondayShift && mondayShift.Configure && mondayShift.Room) {
                    var shiftFrom = getFormattedTime(mondayShift.From);
                    var shiftTo = getFormattedTime(mondayShift.To);
                    staffShift.push(`${shiftFrom} - ${shiftTo}`);
                } else {
                    staffShift.push(`---`);
                }

                var tuesdayShift = _.find(staff.Shift, { Day: "Tuesday" });
                if (tuesdayShift && tuesdayShift.Configure && tuesdayShift.Room) {
                    var shiftFrom = getFormattedTime(tuesdayShift.From);
                    var shiftTo = getFormattedTime(tuesdayShift.To);
                    staffShift.push(`${shiftFrom} - ${shiftTo}`);
                } else {
                    staffShift.push(`---`);
                }

                var wednesdayShift = _.find(staff.Shift, { Day: "Wednesday" });
                if (wednesdayShift && wednesdayShift.Configure && wednesdayShift.Room) {
                    var shiftFrom = getFormattedTime(wednesdayShift.From);
                    var shiftTo = getFormattedTime(wednesdayShift.To);
                    staffShift.push(`${shiftFrom} - ${shiftTo}`);
                } else {
                    staffShift.push(`---`);
                }

                var thursdayShift = _.find(staff.Shift, { Day: "Thursday" });
                if (thursdayShift && thursdayShift.Configure && thursdayShift.Room) {
                    var shiftFrom = getFormattedTime(thursdayShift.From);
                    var shiftTo = getFormattedTime(thursdayShift.To);
                    staffShift.push(`${shiftFrom} - ${shiftTo}`);
                } else {
                    staffShift.push(`---`);
                }

                var fridayShift = _.find(staff.Shift, { Day: "Friday" });
                if (fridayShift && fridayShift.Configure && fridayShift.Room) {
                    var shiftFrom = getFormattedTime(fridayShift.From);
                    var shiftTo = getFormattedTime(fridayShift.To);
                    staffShift.push(`${shiftFrom} - ${shiftTo}`);
                } else {
                    staffShift.push(`---`);
                }

                staffOnShift.push(staffShift);
            }
        });

        setTimeout(() => {
            var docDefinition = {
                footer: {
                    columns: [
                        { text: 'WISE1NE', alignment: 'right', margin: [0, 0, 10, 10], }
                    ]
                },
                pageSize: 'A4',
                pageOrientation: 'landscape',
                content: [{
                    text: `${$scope.shift.Location.Name} `,
                    style: 'header'
                }, {
                    text: `Week: ${startDate} - ${endDate}`,
                    style: 'subheader'
                }, {
                    table: {
                        headerRows: 1,
                        widths: [100, 100, 100, 100, 100, 100],
                        body: staffOnShift
                    }
                }],
                styles: {
                    header: {
                        fontSize: 16,
                        bold: true,
                        margin: [0, 0, 0, 10],
                        color: '#0622a1'
                    },
                    subheader: {
                        fontSize: 14,
                        bold: true,
                        margin: [0, 10, 0, 5],
                        color: '#0622a1'
                    },
                },
            };
            pdfMake.createPdf(docDefinition).download(`${$scope.shift.Location.Name} - Shift Set Up.pdf`);
        }, 50);

    }

    function getFormattedTime(time) {

        var formattedTime = '';
        var timeFormat = time < 12 ? 'AM' : 'PM';
        time = time < 12 ? time : time - 12;
        var strTime = time.toFixed(2).toString();
        if (strTime.includes('.')) {
            var timeParts = strTime.split('.', 2)
            formattedTime = `${timeParts[0]}.${timeParts[1].padEnd(2, '0')} ${timeFormat}`;
        } else {
            formattedTime = `${time} ${timeFormat}`;
        }
        return formattedTime;

    }

    //Watch for date changes
    $scope.$watch('shiftdate', function(newDate) {
        $scope.shift.WeekName = 'Week ' + moment(newDate.startDate, "DD/MM/YYYY").week();
        $scope.utility.weekNo = moment(newDate.startDate, "DD/MM/YYYY").week();
    }, false);

    $scope.goBack = function() {
        $window.history.back();
    };

    $scope.$on('$destroy', function() {
        $sessionStorage.transferData = {};
    });

    var formatDate = function(date) {

        var day = date.getDate(); // yields date
        var month = date.getMonth() + 1; // yields month (add one as '.getMonth()' is zero indexed)
        var year = date.getFullYear(); // yields year
        var hour = date.getHours(); // yields hours 
        var minute = date.getMinutes(); // yields minutes
        var second = date.getSeconds();

        var dateUtil = { Day: day, Month: month, Year: year };
        return dateUtil;

    }

    var formatStrDate = function(date) {

        var day = date.getDate(); // yields date
        var month = date.getMonth() + 1; // yields month (add one as '.getMonth()' is zero indexed)
        var year = date.getFullYear(); // yields year
        var hour = date.getHours(); // yields hours 
        var minute = date.getMinutes(); // yields minutes
        var second = date.getSeconds();

        var dateUtil = `${day}/${month}/${year}`;
        return dateUtil;

    }

    //Watch for staff changes        
    /* $scope.$watch('staffs', function(modifiedStaffs, oldStaffs) {

        for (i = 0; i < oldStaffs.length; i++) {
            if (oldStaffs[i].ShiftConfigured && oldStaffs[i].AutoUpdate) {
                for (j = 0; j < oldStaffs[i].Shift.length; j++) {
                    if (oldStaffs[i].Shift[j].Configure) {
                        if (_.size(modifiedStaffs) > 0) {
                            var staffId = oldStaffs[i].ID;
                            var oldShift = oldStaffs[i].Shift[j];
                            var modifiedShift = modifiedStaffs[i].Shift[j];
                            if (!_.isEqual(modifiedShift.Room, oldShift.Room) || modifiedShift.From != oldShift.From || modifiedShift.To != oldShift.To || modifiedShift.HasSupervision != oldShift.HasSupervision || modifiedShift.BreakTimeDuration != oldShift.BreakTimeDuration || modifiedShift.FolderTimeDuration != oldShift.FolderTimeDuration) {
                                autoUpdate(modifiedShift, modifiedStaffs[i].Shift);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }, true); */

    /* var autoUpdate = function(shift, shifts) {
        for (i = 0; i < shifts.length; i++) {
            if (shifts[i].Configure && shift.Day != shifts[i].Day) {
                shifts[i].From = shift.From;
                shifts[i].To = shift.To;
                shifts[i].HasSupervision = shift.HasSupervision;
                shifts[i].FolderTimeDuration = shift.FolderTimeDuration;
                shifts[i].BreakTimeDuration = shift.BreakTimeDuration;
                shifts[i].Room = shift.Room;
            }
        }
    } */

});