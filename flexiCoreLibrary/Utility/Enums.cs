using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary
{
    public class Enums
    {
        public enum NotificationType
        {
            StaffSignUpRequest,
            StaffSignUpApproval,
            ShiftSwapAvailability,
            ShiftSwapRequest,
            ShiftSwapApproval,
            LeaveApproval,
            LeaveRequest,
            CancelLeaveApproval,
            CancelLeaveRequest,
            NewShift,
            NewTask,
            TaskUpdate,
            ShiftOverTimeRequest,
            ShiftOverTimeApproval,
            ShiftSwap,                                 
            CancelShiftRequest,
            CancelShiftApproval,
            NewMessage           
        }

        public enum ServiceType
        {
            [Description("DAILY CLOCK IN")]
            DailySignIn,
            [Description("AUTO END LEAVE")]
            AutoEndLeave,
            [Description("AUTO START LEAVE")]
            AutoStartLeave,
            [Description("CREATE END OF DAY REPORT")]
            EndOfDayReport,
            [Description("SEND END OF DAY REPORT")]
            SendDailyReportMail,
            [Description("KEEP ALIVE")]
            KeepAlive
        }

        public enum RoleType
        {
            Operation,
            Coach,
            Others
        }

        public enum ReportType
        {
            AbsentReport,
            MissingClockOutReport
        }

        public enum AutoLeaveType
        {
            Start,
            End
        }

        public enum TaskStatus
        {
            Assigned,
            InProgess,
            Completed,
            Deleted
        }

        public enum EmploymentType
        {
            FullTime,
            TimeTerm,
            Bank
        }

        public enum LeaveType
        {
            None,
            Pending,
            Started,
            Cancelled,
            Completed
        }

        public enum ApprovingLocation
        {
            [Description("REQUESTING STAFF LOCATION")]
            RequestingStaffLocation,
            [Description("HEAD OFFICE LOCATION")]
            HeadOfficeLocation
        }

        public enum TaskType
        {
            Group,
            Individual
        }

        public enum ApprovalStatus
        {
            Pending,
            Acknowledged,
            Approved,
            Declined
        }

        public enum FinancialYearStatus
        {            
            Opened,
            Closed,
            Deleted
        }

        public enum ApprovalType
        {
            [Description("STAFF SIGN UP")]
            StaffSignUp,
            [Description("SHIFT SWAP")]
            ShiftSwap,
            [Description("APPROVE LEAVE")]
            ApproveLeave,
            [Description("CANCEL LEAVE")]
            CancelLeave,
            [Description("SHIFT OVER TIME")]
            ShiftOverTime,
            [Description("CANCEL SHIFT")]
            CancelShift,
            [Description("END OF DAY REPORT")]
            EndOfDayReport,
        }

        public enum ShiftSwapStatus
        {
            [Description("Logged for Shift Swap")]
            LoggedForShiftSwap,
            [Description("")]
            ShiftDeletedFromSwapping,
            [Description("Shift Swap Requested")]
            ShiftSwapRequested,
            [Description("Awaiting Shift Swap Approval")]
            AwaitingShiftSwapApproval,
            [Description("Shift Swap Approved")]
            ShiftSwapApproved,
            [Description("Shift Swap Declined")]
            ShiftSwapDeclined,
            [Description("Shift Swap Cancelled")]
            ShiftSwapCancelled,
            [Description("Shift Over Time Requested")]
            ShiftOverTimeRequested,
            [Description("Shift Over Time Approved")]
            ShiftOverTimeApproved,
            [Description("Shift Over Time Declined")]
            ShiftOverTimeDeclined,
            [Description("Shift Cancelled")]
            ShiftCancelled
        }

        public enum Status
        {
            InActive,
            Active
        }

        public enum ApprovingLocationTypes
        {
            RequestingStaffLocation,
            HeadOfficeLocation,
            OperationOfficeLocation
        }

        public static string GetDescription(Type enumType, string enumName)
        {
            // Get the Description attribute value for the enum value
            FieldInfo fi = enumType.GetField(enumName);
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumName;
            }
        }
    }
}
