app.controller('CreateShiftCtrl', function($scope, configFileService, confirmService, $modal, $sessionStorage, WizardHandler) {

    $scope.shift = new shiftdto();

    $scope.utility = {
        locations: [],
        staffLocation: {},
        locationsRetrieved: false,
        staffs: [],
        staffsRetrieved: false,
        rooms: [],
        locationRooms: [],
        selectedStaffs: [],
        shiftdays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'],
        weekNo: 0,
        searchText: '',
        staffOnShift: []
    };

    $scope.shiftdate = {
        startDate: moment(),
        endDate: moment(),
        opts: {
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
            .get(configFileService.apiHandler.retrieveRoomStaffLocation)
            .success(function(data, status, headers) {
                $scope.utility.rooms = data.Rooms;
                getActiveStaffs(data.Staffs);
                getLocations(data.Locations);
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Shift Management', data);
            });
    }

    function getActiveStaffs(staffList) {

        _.forEach(staffList, function(s, skey) {

            var staff = {
                ID: s.ID,
                ShiftConfigured: false,
                AutoUpdate: false,
                KnownAs: s.KnownAs,
                Name: s.FirstName + ' ' + s.Surname,
                Username: s.Username,
                Email: s.Email,
                Telephone: s.Telephone,
                Leave: s.Leave,
                Shift: [],
                Location: s.Location
            };

            _.forEach($scope.utility.shiftdays, function(day, dkey) {
                staff.Shift.push({
                    Day: day,
                    Configure: true,
                    From: 7,
                    To: 19,
                    Room: null,
                    Done: false,
                    HasSupervision: false,
                    BreakTimeDuration: 30,
                    FolderTimeDuration: 0,
                    OverTime: 0
                });

            });

            $scope.utility.staffs.push(staff);
        });
    }

    function getLocations(locationList) {
        $scope.utility.locations = locationList;
        $scope.shift.Location = $sessionStorage.wiseOneUser.Location;
        $scope.utility.staffLocation = $sessionStorage.wiseOneUser.Location;
        $scope.utility.locationsRetrieved = true;
        $scope.getStaff();
        $scope.getStaffLocation();
    }

    $scope.getStaffLocation = function() {

        $scope.utility.staffLocation = $scope.shift.Location;

        $scope.utility.locationRooms = _.filter($scope.utility.rooms, function(room) {
            return _.isEqual(room.Location.ID, $scope.shift.Location.ID);
        });

        $scope.getStaff();

    };

    $scope.getStaff = function() {

        $scope.utility.selectedStaffs = _.filter($scope.utility.staffs, function(staff) {
            return _.isEqual(staff.Location.ID, $scope.utility.staffLocation.ID);
        });

        $scope.utility.searchText = '';
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
            headerText: 'Create Shift Confirmation',
            bodyText: `Are you sure you want to create this Shift for ${$scope.shift.Location.Name}?`
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
                        StaffID: staff.ID,
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

                    shiftConfig.CreatedBy = $sessionStorage.wiseOneUser.Username;
                    shiftConfig.EndDate = formatDate($scope.shiftdate.endDate.toDate());
                    shiftConfig.Location = $scope.shift.Location;
                    shiftConfig.StaffShifts = staffShifts;
                    shiftConfig.StartDate = formatDate($scope.shiftdate.startDate.toDate());
                    shiftConfig.WeekName = $scope.shift.WeekName;
                    shiftConfig.GeneralInformation = $scope.shift.GeneralInformation;

                    $scope.loading = true;
                    configFileService
                        .post(configFileService.apiHandler.saveShiftConfiguration, shiftConfig)
                        .success(function(data, status, headers) {
                            configFileService.displayMessage('success', 'Shift Management', data);
                            $scope.loading = false;
                            shiftConfig = new shiftdto();
                            resetWizard();
                        })
                        .error(function(data, status, headers) {
                            configFileService.displayMessage('error', 'Shift Management', data);
                            $scope.loading = false;
                        });

                } else {
                    redirectWizard();
                    configFileService.displayMessage('error', 'Shift Management', 'There are no shifts to save.');
                }
            } else {
                redirectWizard();
                var uniqStaffs = _.uniq(shiftsWithoutRoom);
                var staffToDisplay = '';
                _.forEach(uniqStaffs, function(s, skey) {
                    staffToDisplay += s.Name + ', ';
                });
                configFileService.displayMessage('error', 'Shift Management', 'Some staff have shift configured but have no shift room set for them for some of the days. Do check and try again. The staff are: ' + staffToDisplay);
            }

        });
    };

    //Watch for date changes
    $scope.$watch('shiftdate', function(newDate) {
        $scope.shift.WeekName = 'Week ' + moment(newDate.startDate, "DD/MM/YYYY").week();
        $scope.utility.weekNo = moment(newDate.startDate, "DD/MM/YYYY").week();
    }, false);

    $scope.replicateShift = function(staff, shift) {
        for (i = 0; i < staff.Shift.length; i++) {
            if (!_.isEqual(shift.Day, staff.Shift[i].Day)) {
                if (staff.Shift[i].Configure) {
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

    var resetWizard = function() {

        $scope.utility.locations = [];
        $scope.utility.locationsRetrieved = false;
        $scope.utility.staffs = [];
        $scope.staffsRetrieved = false;
        $scope.utility.rooms = [];
        $scope.utility.locationRooms = [];
        $scope.utility.selectedStaffs = [];

        $scope.init();

        var wizard = WizardHandler.wizard();
        if (wizard) {
            wizard.goTo(0);
        }
    }

    var redirectWizard = function() {

        var wizard = WizardHandler.wizard();
        if (wizard) {
            wizard.goTo(0);
        }
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