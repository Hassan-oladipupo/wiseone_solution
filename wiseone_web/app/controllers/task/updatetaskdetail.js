app.controller('UpdateTaskDetailCtrl', function($sessionStorage, $scope, $window, configFileService, WizardHandler) {

    if (!$sessionStorage.transferData) {
        $window.history.back();
        return;
    }

    $scope.task = $sessionStorage.transferData.data;

    $scope.staffs = [];
    $scope.staff = {
        selectedValue: {}
    };
    $scope.selectedStaff = [];
    $scope.dateOfCompletion = '';

    $scope.date = {
        completion: moment($scope.task.DateofCompletionStr, "DD/MM/YYYY"),
        opts: {
            locale: {
                applyClass: 'btn-green',
                format: "DD/MM/YYYY",
                cancelLabel: 'Cancel'
            },
            singleDatePicker: true,
            drops: 'up'
        }
    };

    $scope.opts = {
        locale: {
            applyClass: 'btn-green',
            applyLabel: "Apply",
            fromLabel: "From",
            format: "DD/MM/YYYY",
            toLabel: "To",
            cancelLabel: 'Cancel',
            customRangeLabel: 'Custom range'
        },
        ranges: {
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()]
        }
    };


    $scope.init = function() {

        $scope.disabled = true;

        configFileService
            .get(configFileService.apiHandler.retrieveActiveStaffs)
            .success(function(data, status, headers) {
                $scope.staffs = data.data;
                $scope.disabled = !$scope.disabled;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
            });

        configFileService
            .get(configFileService.apiHandler.retrieveTaskStaff + '?taskID=' + $scope.task.ID)
            .success(function(data, status, headers) {
                $scope.selectedStaff = [];
                _.forEach(data, function(taskStaff) {
                    taskStaff.Staff.TaskLeader = taskStaff.TaskLeader;
                    $scope.selectedStaff.push(taskStaff.Staff);
                });
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
            });

    };

    $scope.taskDetailsValidation = function() {

        if (!_.isEmpty($scope.task.Subject) && !_.isEmpty($scope.task.Details)) {
            return true;
        } else {
            configFileService.displayMessage('info', 'Task Management', 'All the fields are required.');
            return false;
        }

    };

    $scope.selectedStaffValidation = function() {

        if (_.size($scope.selectedStaff) > 0) {

            if (_.size($scope.selectedStaff) == 1) {
                $scope.selectedStaff[0].TaskLeader = true;
                return true;
            } else {

                var taskLeaderExists = false;
                _.forEach($scope.selectedStaff, function(staff) {
                    if (staff.TaskLeader) {
                        taskLeaderExists = true;
                    }
                });

                if (taskLeaderExists) {
                    return true;
                } else {
                    configFileService.displayMessage('info', 'Task Management', 'A task leader is required. Select a staff.');
                    return false;
                }
            }

        } else {
            configFileService.displayMessage('info', 'Task Management', 'Add staff for this Task.');
            return false;
        }

    };

    $scope.addStaff = function() {

        if ($scope.staff.selectedValue.ID) {
            var staffExists = _.find($scope.selectedStaff, function(s, skey) {
                return _.isEqual(s.ID, $scope.staff.selectedValue.ID);
            });

            if (!staffExists) {

                $scope.staff.selectedValue.TaskLeader = false;
                $scope.staff.selectedValue.ExistingStaff = false;
                $scope.selectedStaff.push($scope.staff.selectedValue);

            }
        }

    };

    $scope.removeStaff = function(staff) {

        _.remove($scope.selectedStaff, function(s) {
            return _.isEqual(s.ID, staff.ID);
        });

    };

    $scope.save = function() {

        $scope.loading = true;

        _.forEach($scope.selectedStaff, function(staff) {
            $scope.task.TaskStaffs.push({
                Staff: staff
            });
        });

        var taskLeader = _.find($scope.selectedStaff, { TaskLeader: true });

        $scope.task.LastUpdatedBy = $sessionStorage.wiseOneUser.FirstName + ' ' + $sessionStorage.wiseOneUser.Surname;
        $scope.task.DateofCompletion = formatDate($scope.date.completion.toDate());
        $scope.task.DateofCompletionStr = $scope.dateOfCompletion;
        $scope.task.TaskLeader = taskLeader.FirstName + ' ' + taskLeader.Surname;

        configFileService
            .post(configFileService.apiHandler.updateTask, $scope.task)
            .success(function(data, status, headers) {
                configFileService.displayMessage('success', 'Task Management', data);
                $scope.loading = false;
            })
            .error(function(data, status, headers) {
                configFileService.displayMessage('info', 'Task Management', data);
                $scope.loading = false;
            });

    };

    $scope.$watch('date.completion', function(d) {
        $scope.dateOfCompletion = d.format('dddd, MMM Do YYYY');
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

    };

});