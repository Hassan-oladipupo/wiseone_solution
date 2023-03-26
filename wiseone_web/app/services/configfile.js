app.service('configFileService', ['toaster', '$http', function(toaster, $http) {

    //#region AutoComplete JS
    this.websiteUrl = 'http://localhost/wiseone/wiseoneservice/';
    //this.websiteUrl = 'http://192.168.43.245/wiseone/wiseoneservice/';
    //this.websiteUrl = 'http://192.168.0.2/wiseone/wiseoneservice/';
    //this.websiteUrl = 'https://adminservice.wise1ne.com/';
    //#endregion AutoComplete JS

    this.displayMessage = function(type, title, text) {
        toaster.pop(type, title, text);
    };

    this.post = function(url, data) {
        return $http({
            url: url,
            method: "POST",
            data: data,
            headers: {
                'App-Version': '0.0.5',
            }
        });
    };

    this.put = function(url, data) {
        return $http.put(url, data);
    };

    this.delete = function(url, data) {
        return $http({
            url: url,
            method: "DELETE",
            params: data
        });
    };

    this.get = function(url) {
        return $http.get(url);
    };

    this.get = function(url, data) {
        return $http({
            url: url,
            method: "GET",
            params: data
        });
    };

    this.taskStatus = {
        Assigned: 'Assigned',
        ReAssigned: 'ReAssigned',
        InProgress: 'InProgress',
        PutOnHold: 'PutOnHold',
        Completed: 'Completed',
        Cancelled: 'Cancelled'
    };

    this.apiHandler = {
        saveFunction: this.websiteUrl + 'api/FunctionAPI/SaveFunction',
        updateFunction: this.websiteUrl + 'api/FunctionAPI/UpdateFunction',
        retrieveFunctions: this.websiteUrl + 'api/FunctionAPI/RetrieveFunctions',

        saveRole: this.websiteUrl + 'api/RoleAPI/SaveRole',
        updateRole: this.websiteUrl + 'api/RoleAPI/UpdateRole',
        enableOrDisableRole: this.websiteUrl + 'api/RoleAPI/EnableOrDisableRole',
        retrieveRoles: this.websiteUrl + 'api/RoleAPI/RetrieveRoles',
        retrieveActiveRoles: this.websiteUrl + 'api/RoleAPI/RetrieveActiveRoles',

        saveLocation: this.websiteUrl + 'api/LocationAPI/SaveLocation',
        updateLocation: this.websiteUrl + 'api/LocationAPI/UpdateLocation',
        enableOrDisableLocation: this.websiteUrl + 'api/LocationAPI/EnableOrDisableLocation',
        retrieveLocations: this.websiteUrl + 'api/LocationAPI/RetrieveLocations',
        retrieveActiveLocations: this.websiteUrl + 'api/LocationAPI/RetrieveActiveLocations',

        saveClassRoom: this.websiteUrl + 'api/ClassRoomAPI/SaveClassRoom',
        updateClassRoom: this.websiteUrl + 'api/ClassRoomAPI/UpdateClassRoom',
        enableOrDisableClassRoom: this.websiteUrl + 'api/ClassRoomAPI/EnableOrDisableClassRoom',
        retrieveClassRooms: this.websiteUrl + 'api/ClassRoomAPI/RetrieveClassRooms',
        retrieveActiveClassRooms: this.websiteUrl + 'api/ClassRoomAPI/RetrieveActiveClassRooms',
        retrieveActiveClassRoomsInLocation: this.websiteUrl + 'api/ClassRoomAPI/RetrieveActiveClassRoomsInLocation',

        retrieveSignUpRequests: this.websiteUrl + 'api/StaffAPI/RetrieveSignUpRequests',
        approveStaffSignUp: this.websiteUrl + 'api/StaffAPI/ApproveStaffSignUp',
        updateStaff: this.websiteUrl + 'api/StaffAPI/UpdateStaff',
        enableOrDisableStaff: this.websiteUrl + 'api/StaffAPI/EnableOrDisableStaff',
        declineStaffSignUp: this.websiteUrl + 'api/StaffAPI/DeclineStaffSignUp',
        retrieveStaffs: this.websiteUrl + 'api/StaffAPI/RetrieveStaffs',
        retrieveActiveStaffs: this.websiteUrl + 'api/StaffAPI/RetrieveActiveStaffs',
        retrieveActiveStaffOnLeave: this.websiteUrl + 'api/StaffAPI/RetrieveActiveStaffOnLeave',
        retrieveActiveStaffsInLocation: this.websiteUrl + 'api/StaffAPI/RetrieveActiveStaffsInLocation',
        cancelLeave: this.websiteUrl + 'api/StaffAPI/CancelLeave',

        saveConfiguration: this.websiteUrl + 'api/ApproverEmailAPI/SaveConfiguration',
        updateConfiguration: this.websiteUrl + 'api/ApproverEmailAPI/UpdateConfiguration',
        deleteConfiguration: this.websiteUrl + 'api/ApproverEmailAPI/DeleteConfiguration',
        retrieveConfigurations: this.websiteUrl + 'api/ApproverEmailAPI/RetrieveConfigurations',
        retrieveSecondLevelConfigurations: this.websiteUrl + 'api/ApproverEmailAPI/RetrieveSecondLevelConfigurations',
        saveSecondLevelConfiguration: this.websiteUrl + 'api/ApproverEmailAPI/SaveSecondLevelConfiguration',
        deleteSecondLevelConfiguration: this.websiteUrl + 'api/ApproverEmailAPI/DeleteSecondLevelConfiguration',

        saveShiftConfiguration: this.websiteUrl + 'api/ShiftConfigurationAPI/SaveShiftConfiguration',
        updateShiftConfiguration: this.websiteUrl + 'api/ShiftConfigurationAPI/UpdateShiftConfiguration',
        retrieveShiftConfigurations: this.websiteUrl + 'api/ShiftConfigurationAPI/RetrieveShiftConfigurations',
        retrieveShiftSwapForApproval: this.websiteUrl + 'api/ShiftConfigurationAPI/RetrieveShiftSwapForApproval',
        approveShiftSwap: this.websiteUrl + 'api/ShiftConfigurationAPI/ApproveShiftSwap',
        declineShiftSwapApproval: this.websiteUrl + 'api/ShiftConfigurationAPI/DeclineShiftSwapApproval',
        deleteStaffShiftConfiguration: this.websiteUrl + 'api/ShiftConfigurationAPI/DeleteStaffShiftConfiguration',
        retrieveRoomStaffLocation: this.websiteUrl + 'api/ShiftConfigurationAPI/RetrieveRoomStaffLocation',
        retrieveLocationRoomStaffLocation: this.websiteUrl + 'api/ShiftConfigurationAPI/RetrieveLocationRoomStaffLocation',

        saveFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/SaveFinancialYear',
        retrieveFinancialYears: this.websiteUrl + 'api/FinancialYearAPI/RetrieveFinancialYears',
        retrieveLocationFinancialYears: this.websiteUrl + 'api/FinancialYearAPI/RetrieveLocationFinancialYears',
        retrieveOpenedClosedFinancialYears: this.websiteUrl + 'api/FinancialYearAPI/RetrieveOpenedClosedFinancialYears',
        updateFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/UpdateFinancialYear',
        deleteFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/DeleteFinancialYear',
        toggleFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/ToggleFinancialYear',
        retrieveLeaveRequests: this.websiteUrl + 'api/FinancialYearAPI/RetrieveLeaveRequests',
        declineLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/DeclineLeaveRequest',
        approveLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/ApproveLeaveRequest',
        secondLevelApproveLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/SecondLevelApproveLeaveRequest',
        retrieveCancelLeaveRequests: this.websiteUrl + 'api/FinancialYearAPI/RetrieveCancelLeaveRequests',
        declineCancelLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/DeclineCancelLeaveRequest',
        approveCancelLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/ApproveCancelLeaveRequest',
        currentFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/CurrentFinancialYear',
        retrieveStaffLeaves: this.websiteUrl + 'api/FinancialYearAPI/RetrieveStaffLeaves',
        retrieveStaffLeavesByFinancialYear: this.websiteUrl + 'api/FinancialYearAPI/RetrieveStaffLeavesByFinancialYear',
        saveLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/SaveLeaveRequest',
        deleteLeaveRequest: this.websiteUrl + 'api/FinancialYearAPI/DeleteLeaveRequest',

        saveTask: this.websiteUrl + 'api/TaskAPI/SaveTask',
        updateTask: this.websiteUrl + 'api/TaskAPI/UpdateTask',
        deleteTask: this.websiteUrl + 'api/TaskAPI/DeleteTask',
        retrieveTasks: this.websiteUrl + 'api/TaskAPI/RetrieveTasks',
        retrieveTaskStaff: this.websiteUrl + 'api/TaskAPI/RetrieveTaskStaff',
        retrieveTaskUpdates: this.websiteUrl + 'api/TaskAPI/RetrieveTaskUpdates',

        retrieveOverTimeForApproval: this.websiteUrl + 'api/SignInOutAPI/RetrieveOverTimeForApproval',
        approveOrDeclineOverTime: this.websiteUrl + 'api/SignInOutAPI/ApproveOrDeclineOverTime',
        retrieveSignInOuts: this.websiteUrl + 'api/SignInOutAPI/RetrieveSignInOuts',
        retrieveStaffSignInOuts: this.websiteUrl + 'api/SignInOutAPI/RetrieveStaffSignInOuts',
        retrieveShiftCancelRequests: this.websiteUrl + 'api/SignInOutAPI/RetrieveShiftCancelRequests',
        approveShiftCancellation: this.websiteUrl + 'api/SignInOutAPI/ApproveShiftCancellation',
        retrieveReportSummary: this.websiteUrl + 'api/SignInOutAPI/RetrieveReportSummary',
        retrieveReportDetail: this.websiteUrl + 'api/SignInOutAPI/RetrieveReportDetail',
        retrieveStaffReportSummary: this.websiteUrl + 'api/SignInOutAPI/RetrieveStaffReportSummary',
        retrieveStaffReportDetail: this.websiteUrl + 'api/SignInOutAPI/RetrieveStaffReportDetail',

        retrieveShiftLatestFeeds: this.websiteUrl + 'api/StaffShiftFeedAPI/RetrieveShiftLatestFeeds',
        retrieveServices: this.websiteUrl + 'api/StaffShiftFeedAPI/RetrieveServices',

        login: this.websiteUrl + 'api/StaffAPI/AuthenticateStaff',
        changePassword: this.websiteUrl + 'api/StaffAPI/ChangePassword',
        forgotPassword: this.websiteUrl + 'api/StaffAPI/ForgotPassword',
        confirmName: this.websiteUrl + 'api/StaffAPI/ConfirmUsername',
    };

}]);