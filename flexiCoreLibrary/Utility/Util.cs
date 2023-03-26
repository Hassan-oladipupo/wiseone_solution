using flexiCoreLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Utility
{
    public class Util
    {
        public static void RunServicesInParallel(params Action[] tasks)
        {
            // Initialize the reset events to keep track of completed threads
            ManualResetEvent[] resetEvents = new ManualResetEvent[tasks.Length];

            // Launch each method in it's own thread
            for (int i = 0; i < tasks.Length; i++)
            {
                resetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback((object index) =>
                {
                    int taskIndex = (int)index;

                    // Execute the method
                    tasks[taskIndex]();

                    // Tell the calling thread that we're done
                    resetEvents[taskIndex].Set();
                }), i);
            }

            // Wait for all threads to execute
            WaitHandle.WaitAll(resetEvents);
        }

        public static List<LeaveTypeDto> GetLeaveTypes()
        {
            return new List<LeaveTypeDto>()
            {
                new LeaveTypeDto() { Type= "Annual", Name = "Annual Leave", Deductible = true, IsFullTimeOnly = true },
                new LeaveTypeDto() { Type= "Compassionate", Name = "Compassionate Leave", Deductible = true, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Unpaid", Name = "Unpaid Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Bereavement", Name = "Bereavement Leave", Deductible = true, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Parental", Name = "Parental Leave", Deductible = true, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Sickness", Name = "Sickness Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Adoptive", Name = "Adoptive Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Carers", Name = "Carers Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "JuryService", Name = "Jury Service Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Study", Name = "Study Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "HospitalAppointment", Name = "Hospital Appointment Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "DentalAppointment", Name = "Dental Appointment Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Maternity", Name = "Maternity Leave", Deductible = false, IsFullTimeOnly = false },
                new LeaveTypeDto() { Type= "Paternity", Name = "Paternity Leave", Deductible = true, IsFullTimeOnly = false },
            };
        }

        public static List<string> GetDeductibleLeaveTypes()
        {
            var leaveTypes = (from leaveType in GetLeaveTypes()
                                           where leaveType.Deductible == true
                                           select leaveType.Type).ToList();

            return leaveTypes;
        }

        public static List<string> GetNonDeductibleLeaveTypes()
        {
            var leaveTypes = (from leaveType in GetLeaveTypes()
                                        where leaveType.Deductible == false
                                        select leaveType.Type).ToList();

            return leaveTypes;
        }

        public static string GetLeaveTypeDescription(string leaveType)
        {
            var leaveTypeDescription = string.Empty;

            switch (leaveType)
            {
                case "Annual":
                    leaveTypeDescription = "Annual Leave";
                    break;
                case "Compassionate":
                    leaveTypeDescription = "Compassionate Leave";
                    break;
                case "Unpaid":
                    leaveTypeDescription = "Unpaid Leave";
                    break;
                case "Bereavement":
                    leaveTypeDescription = "Bereavement Leave";
                    break;
                case "Parental":
                    leaveTypeDescription = "Parental Leave";
                    break;
                case "Sickness":
                    leaveTypeDescription = "Sickness Leave";
                    break;
                case "Adoptive":
                    leaveTypeDescription = "Adoptive Leave";
                    break;
                case "Carers":
                    leaveTypeDescription = "Carers Leave";
                    break;
                case "JuryService":
                    leaveTypeDescription = "Jury Service Leave";
                    break;
                case "Study":
                    leaveTypeDescription = "Study Leave";
                    break;
                case "HospitalAppointment":
                    leaveTypeDescription = "Hospital Appointment Leave";
                    break;
                case "DentalAppointment":
                    leaveTypeDescription = "Dental Appointment Leave";
                    break;
                case "Maternity":
                    leaveTypeDescription = "Maternity Leave";
                    break;
                case "Paternity":
                    leaveTypeDescription = "Paternity Leave";
                    break;
                default:
                    leaveTypeDescription = "Annual";
                    break;
            }

            return leaveTypeDescription;
        }

        public static bool IsLeaveTypeDeductible(string leaveType)
        {
            var result = GetLeaveTypes().FirstOrDefault(l => l.Type == leaveType).Deductible;
            return result;
        }

        public static Enums.LeaveType GetLeaveStatusType(List<FinancialYearMonthDayDto> requestedDays)
        {
            var leaveType = Enums.LeaveType.Pending;

            var today = DateUtil.Now().Date;

            var leaveStartDate = GetLeaveStartDate(requestedDays).Date;

            var leaveEndDate = GetLeaveEndDate(requestedDays).Date;

            if (today < leaveStartDate)
            {
                leaveType = Enums.LeaveType.Pending;
            }
            else if (today >= leaveStartDate && today < leaveEndDate)
            {
                leaveType = Enums.LeaveType.Started;
            }
            else if (today >= leaveEndDate)
            {
                if (today == leaveEndDate)
                {
                    var time = (DateUtil.Now()).Hour;

                    leaveType = time >= 21 ? Enums.LeaveType.Completed : Enums.LeaveType.Started;
                }
                else
                {
                    leaveType = Enums.LeaveType.Completed;
                }
            }

            return leaveType;
        }

        private static DateTime GetLeaveStartDate(List<FinancialYearMonthDayDto> requestedDays)
        {
            var dates = GetRequestedDates(requestedDays);
            dates.Sort((a, b) => a.CompareTo(b));
            return dates.FirstOrDefault();
        }

        private static DateTime GetLeaveEndDate(List<FinancialYearMonthDayDto> requestedDays)
        {
            var dates = GetRequestedDates(requestedDays);
            dates.Sort((a, b) => b.CompareTo(a));
            return dates.FirstOrDefault();
        }

        private static List<DateTime> GetRequestedDates(List<FinancialYearMonthDayDto> RequestedDays)
        {
            var dates = new List<DateTime>();

            RequestedDays.ForEach((requestedDay) =>
            {
                var year = Convert.ToInt32(requestedDay.Year);
                var month = GetMonthNumber(requestedDay.Month);
                var day = requestedDay.Day;

                var temp = new DateTime(year, month, day);

                var date = DateUtil.Now(temp);
                dates.Add(date);
            });

            return dates;
        }

        public static int GetMonthNumber(string month)
        {
            var result = 1;

            switch (month)
            {
                case "January":
                    result = 1;
                    break;
                case "February":
                    result = 2;
                    break;
                case "March":
                    result = 3;
                    break;
                case "April":
                    result = 4;
                    break;
                case "May":
                    result = 5;
                    break;
                case "June":
                    result = 6;
                    break;
                case "July":
                    result = 7;
                    break;
                case "August":
                    result = 8;
                    break;
                case "September":
                    result = 9;
                    break;
                case "October":
                    result = 10;
                    break;
                case "November":
                    result = 11;
                    break;
                case "December":
                    result = 12;
                    break;
                default:
                    result = 1;
                    break;
            }

            return result;
        }
    }
}
