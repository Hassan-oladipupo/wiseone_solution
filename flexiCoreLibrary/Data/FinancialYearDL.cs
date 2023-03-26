using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Extensions;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class FinancialYearDL
    {
        public static bool Save(FinancialYear financialYear)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.FinancialYears.Add(financialYear);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool FinancialYearExists(string startDate, string endDate, long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var deleted = Enums.FinancialYearStatus.Deleted.ToString();

                    var financialYears = context.FinancialYears
                                                .Where(f => f.Status != deleted &&
                                                            f.StartDate == startDate &&
                                                            f.EndDate == endDate &&
                                                            f.LocationID == locationID)
                                                .ToList();

                    return financialYears.Any() ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(FinancialYear financialYear)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingFY = new FinancialYear();

                    existingFY = context.FinancialYears
                                    .Where(t => t.ID == financialYear.ID)
                                    .FirstOrDefault();

                    if (existingFY != null)
                    {
                        existingFY.Status = Enums.FinancialYearStatus.Deleted.ToString();

                        context.Entry(existingFY).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ToggleStatus(FinancialYear financialYear)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var opened = Enums.FinancialYearStatus.Opened.ToString();

                    if (financialYear.Status == opened)
                    {
                        var openedFinancialYears = context.FinancialYears
                                                         .Where(t => t.Status == opened &&
                                                                     t.LocationID == financialYear.LocationID);

                        if (openedFinancialYears.Any())
                        {
                            throw new Exception("Another financial year is currently opened, close it and try again.");
                        }
                    }


                    var existingFY = new FinancialYear();
                    existingFY = context.FinancialYears
                                    .Where(t => t.ID == financialYear.ID)
                                    .FirstOrDefault();

                    if (existingFY != null)
                    {
                        existingFY.Status = financialYear.Status;

                        context.Entry(existingFY).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(FinancialYear financialYear)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingFY = new FinancialYear();

                    existingFY = context.FinancialYears
                                    .Where(t => t.ID == financialYear.ID)
                                    .FirstOrDefault();

                    if (existingFY != null)
                    {
                        existingFY.LeavePriorNotice = financialYear.LeavePriorNotice;

                        context.Entry(existingFY).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveLeaveRequest(LeaveRequestDto leaveRequest)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            leaveRequest.RequestedDays.ForEach(requestedDay =>
                            {
                                var day = context.FinancialYearMonthDays
                                                    .Where(d => d.ID == requestedDay.ID)
                                                    .FirstOrDefault();

                                day.NumberOfStaff += 1;
                                if (!string.IsNullOrEmpty(day.Staff) && day.Staff != "null")
                                {
                                    var staffLeaves = JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff);
                                    var staffLeave = new StaffLeaveDto
                                    {
                                        StaffEmail = leaveRequest.StaffEmail,
                                        StaffKnownAs = leaveRequest.StaffKnownAs,
                                        StaffName = leaveRequest.StaffName,
                                        StaffUsername = leaveRequest.StaffUsername,
                                        StaffID = leaveRequest.StaffID,
                                        LocationID = leaveRequest.StaffLocation.ID,
                                        LocationName = leaveRequest.StaffLocation.Name
                                    };

                                    staffLeaves.Add(staffLeave);
                                    day.Staff = JsonConvert.SerializeObject(staffLeaves);
                                }
                                else
                                {
                                    var staffLeave = new List<StaffLeaveDto>();
                                    staffLeave.Add(new StaffLeaveDto
                                    {
                                        StaffEmail = leaveRequest.StaffEmail,
                                        StaffKnownAs = leaveRequest.StaffKnownAs,
                                        StaffName = leaveRequest.StaffName,
                                        StaffUsername = leaveRequest.StaffUsername,
                                        StaffID = leaveRequest.StaffID,
                                        LocationID = leaveRequest.StaffLocation.ID,
                                        LocationName = leaveRequest.StaffLocation.Name
                                    });

                                    day.Staff = JsonConvert.SerializeObject(staffLeave);
                                }

                                context.Entry(day).State = EntityState.Modified;
                                context.SaveChanges();
                            });

                            var leaveType = Utility.Util.GetLeaveStatusType(leaveRequest.RequestedDays);

                            var leave = new StaffLeave
                            {
                                StaffEmail = leaveRequest.StaffEmail,
                                StaffKnownAs = leaveRequest.StaffKnownAs,
                                StaffName = leaveRequest.StaffName,
                                StaffUsername = leaveRequest.StaffUsername,
                                StaffID = leaveRequest.StaffID,
                                ApprovedOn = string.Format("{0:g}", DateUtil.Now()),
                                FinancialYearEndDate = leaveRequest.FinancialYear.EndDate,
                                FinancialYearID = leaveRequest.FinancialYear.ID,
                                FinancialYearStartDate = leaveRequest.FinancialYear.StartDate,
                                LocationID = leaveRequest.StaffLocation.ID,
                                LocationName = leaveRequest.StaffLocation.Name,
                                NumberOfLeaveDaysTaken = leaveRequest.LeaveDaysTaken,
                                RequestedDays = JsonConvert.SerializeObject(leaveRequest.RequestedDays),
                                RequestedOn = string.Format("{0:g}", DateUtil.Now()),
                                LeaveType = leaveType,
                                LeaveCriteria = leaveRequest.LeaveType
                            };

                            context.StaffLeaves.Add(leave);
                            context.SaveChanges();

                            var staff = context.Staffs.FirstOrDefault(s => s.ID == leaveRequest.StaffID);
                            staff.LeaveType = leaveType == Enums.LeaveType.Completed ? Enums.LeaveType.None : leaveType;

                            context.Entry(staff).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LogLeaveRequestForSecondLevelApproval(LeaveApprovalDto leaveApproval)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var approval = context.Approvals
                                                    .Where(a => a.ID == leaveApproval.Approval.ApprovalID)
                                                    .FirstOrDefault();

                            approval.Status = Enums.ApprovalStatus.Acknowledged.ToString();
                            approval.DeclineReason = leaveApproval.LeaveRequest.DeclineReason;
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ApproveLeaveRequest(LeaveApprovalDto leaveApproval)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            leaveApproval.LeaveRequest.RequestedDays.ForEach(requestedDay =>
                            {
                                var day = context.FinancialYearMonthDays
                                                    .Where(d => d.ID == requestedDay.ID)
                                                    .FirstOrDefault();

                                day.NumberOfStaff += 1;
                                if (!string.IsNullOrEmpty(day.Staff) && day.Staff != "null")
                                {
                                    var staffLeaves = JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff);
                                    var staffLeave = new StaffLeaveDto
                                    {
                                        StaffEmail = leaveApproval.LeaveRequest.StaffEmail,
                                        StaffKnownAs = leaveApproval.LeaveRequest.StaffKnownAs,
                                        StaffName = leaveApproval.LeaveRequest.StaffName,
                                        StaffUsername = leaveApproval.LeaveRequest.StaffUsername,
                                        StaffID = leaveApproval.LeaveRequest.StaffID,
                                        LocationID = leaveApproval.LeaveRequest.StaffLocation.ID,
                                        LocationName = leaveApproval.LeaveRequest.StaffLocation.Name
                                    };

                                    staffLeaves.Add(staffLeave);
                                    day.Staff = JsonConvert.SerializeObject(staffLeaves);
                                }
                                else
                                {
                                    var staffLeave = new List<StaffLeaveDto>();
                                    staffLeave.Add(new StaffLeaveDto
                                    {
                                        StaffEmail = leaveApproval.LeaveRequest.StaffEmail,
                                        StaffKnownAs = leaveApproval.LeaveRequest.StaffKnownAs,
                                        StaffName = leaveApproval.LeaveRequest.StaffName,
                                        StaffUsername = leaveApproval.LeaveRequest.StaffUsername,
                                        StaffID = leaveApproval.LeaveRequest.StaffID,
                                        LocationID = leaveApproval.LeaveRequest.StaffLocation.ID,
                                        LocationName = leaveApproval.LeaveRequest.StaffLocation.Name
                                    });

                                    day.Staff = JsonConvert.SerializeObject(staffLeave);
                                }

                                context.Entry(day).State = EntityState.Modified;
                                context.SaveChanges();
                            });

                            var leaveType = Utility.Util.GetLeaveStatusType(leaveApproval.LeaveRequest.RequestedDays);

                            var leave = new StaffLeave
                            {
                                StaffEmail = leaveApproval.LeaveRequest.StaffEmail,
                                StaffKnownAs = leaveApproval.LeaveRequest.StaffKnownAs,
                                StaffName = leaveApproval.LeaveRequest.StaffName,
                                StaffUsername = leaveApproval.LeaveRequest.StaffUsername,
                                StaffID = leaveApproval.LeaveRequest.StaffID,
                                ApprovedOn = string.Format("{0:g}", DateUtil.Now()),
                                FinancialYearEndDate = leaveApproval.FinancialYear.EndDate,
                                FinancialYearID = leaveApproval.FinancialYear.ID,
                                FinancialYearStartDate = leaveApproval.FinancialYear.StartDate,
                                LocationID = leaveApproval.LeaveRequest.StaffLocation.ID,
                                LocationName = leaveApproval.LeaveRequest.StaffLocation.Name,
                                NumberOfLeaveDaysTaken = leaveApproval.LeaveRequest.LeaveDaysTaken,
                                RequestedDays = JsonConvert.SerializeObject(leaveApproval.LeaveRequest.RequestedDays),
                                RequestedOn = leaveApproval.Approval.RequestedOn,
                                LeaveType = leaveType,
                                LeaveCriteria = leaveApproval.LeaveRequest.LeaveType
                            };

                            context.StaffLeaves.Add(leave);
                            context.SaveChanges();

                            var staff = context.Staffs.FirstOrDefault(s => s.ID == leaveApproval.LeaveRequest.StaffID);
                            staff.LeaveType = leaveType == Enums.LeaveType.Completed ? Enums.LeaveType.None : leaveType;

                            context.Entry(staff).State = EntityState.Modified;
                            context.SaveChanges();

                            var approval = context.Approvals
                                                    .Where(a => a.ID == leaveApproval.Approval.ApprovalID)
                                                    .FirstOrDefault();

                            approval.Status = Enums.ApprovalStatus.Approved.ToString();
                            approval.DeclineReason = leaveApproval.LeaveRequest.DeclineReason;
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ApproveCancelLeaveRequest(StaffLeaveDto leaveApproval)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            leaveApproval.RequestedDays.ForEach(requestedDay =>
                            {
                                var day = context.FinancialYearMonthDays
                                                    .Where(d => d.ID == requestedDay.ID)
                                                    .FirstOrDefault();

                                day.NumberOfStaff -= 1;

                                var staffLeaves = JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff);

                                var staff = staffLeaves.FirstOrDefault(s => s.StaffID == leaveApproval.StaffID);
                                staffLeaves.Remove(staff);

                                day.Staff = JsonConvert.SerializeObject(staffLeaves);

                                context.Entry(day).State = EntityState.Modified;
                                context.SaveChanges();
                            });

                            var leave = context.StaffLeaves.FirstOrDefault(x => x.ID == leaveApproval.ID);
                            leave.LeaveType = Enums.LeaveType.Cancelled;

                            context.Entry(leave).State = EntityState.Modified;
                            context.SaveChanges();


                            var approval = context.Approvals
                                                    .Where(a => a.ID == leaveApproval.Approval.ApprovalID)
                                                    .FirstOrDefault();

                            approval.Status = Enums.ApprovalStatus.Approved.ToString();
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteLeaveRequest(StaffLeaveDto leaveApproval)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            leaveApproval.RequestedDays.ForEach(requestedDay =>
                            {
                                var day = context.FinancialYearMonthDays
                                                    .Where(d => d.ID == requestedDay.ID)
                                                    .FirstOrDefault();

                                day.NumberOfStaff -= 1;

                                var staffLeaves = JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff);

                                var staff = staffLeaves.FirstOrDefault(s => s.StaffID == leaveApproval.StaffID);
                                staffLeaves.Remove(staff);

                                day.Staff = JsonConvert.SerializeObject(staffLeaves);

                                context.Entry(day).State = EntityState.Modified;
                                context.SaveChanges();
                            });

                            var leave = context.StaffLeaves.FirstOrDefault(x => x.ID == leaveApproval.ID);
                            leave.LeaveType = Enums.LeaveType.Cancelled;

                            context.Entry(leave).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal GetNumberOfLeaveDays(long staffID, long financialYearID)
        {
            try
            {
                decimal leaveDays = 0.0m;

                var deductibleLeaveTypes = Utility.Util.GetDeductibleLeaveTypes();

                using (var context = new DataContext())
                {
                    var leaves = context.StaffLeaves
                                       .Where(s => s.StaffID == staffID &&
                                                   s.FinancialYearID == financialYearID &&
                                                   s.LeaveType != Enums.LeaveType.Cancelled &&
                                                   deductibleLeaveTypes.Contains(s.LeaveCriteria));

                    if (leaves.Any())
                    {
                        leaveDays = (from leave in leaves
                                     select leave.NumberOfLeaveDaysTaken).Sum();
                    }
                }

                return leaveDays;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffLeaveDetailsDto RetrieveStaffLeaves(long staffID, long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffLeave = new StaffLeaveDetailsDto();

                    staffLeave.NumberOfLeaveTaken = 0;
                    staffLeave.StaffLeave = new List<StaffLeaveDto>();

                    var opened = Enums.FinancialYearStatus.Opened.ToString();
                    var financialYear = context.FinancialYears
                                                .Where(f => f.Status == opened && f.LocationID == locationID)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .FirstOrDefault();

                    if (financialYear != null)
                    {
                        var leaves = context.StaffLeaves
                                                .Where(s => s.StaffID == staffID &&
                                                            s.FinancialYearID == financialYear.ID &&
                                                            s.LeaveType != Enums.LeaveType.Cancelled)
                                                .ToList();

                        if (leaves.Any())
                        {
                            var deductibleLeaveTypes = Utility.Util.GetDeductibleLeaveTypes();

                            var nonDeductibleLeaveTypes = Utility.Util.GetNonDeductibleLeaveTypes();

                            var leaveDays = (from leave in leaves
                                             where deductibleLeaveTypes.Contains(leave.LeaveCriteria)
                                             select leave.NumberOfLeaveDaysTaken).Sum();

                            staffLeave.NumberOfLeaveTaken = leaveDays;

                            var nonLeaveDays = (from leave in leaves
                                                where nonDeductibleLeaveTypes.Contains(leave.LeaveCriteria)
                                                select leave.NumberOfLeaveDaysTaken).Sum();

                            staffLeave.NumberOfNonLeaveTaken = nonLeaveDays;

                            staffLeave.StaffLeave = (from leave in leaves
                                                     select new StaffLeaveDto
                                                     {
                                                         ID = leave.ID,
                                                         FinancialYearID = leave.FinancialYearID,
                                                         LocationID = leave.LocationID,
                                                         LocationName = leave.LocationName,
                                                         StaffEmail = leave.StaffEmail,
                                                         StaffID = leave.StaffID,
                                                         StaffKnownAs = leave.StaffKnownAs,
                                                         StaffName = leave.StaffName,
                                                         StaffUsername = leave.StaffUsername,
                                                         RequestedOn = leave.RequestedOn,
                                                         ApprovedOn = leave.ApprovedOn,
                                                         NumberOfLeaveDaysTaken = leave.NumberOfLeaveDaysTaken,
                                                         RequestedDays = JsonConvert.DeserializeObject<List<FinancialYearMonthDayDto>>(leave.RequestedDays),
                                                         LeaveType = leave.LeaveType.ToString(),
                                                         LeaveTypeDescription = Utility.Util.GetLeaveTypeDescription(leave.LeaveCriteria),
                                                         LeaveIsDeductible = Utility.Util.IsLeaveTypeDeductible(leave.LeaveCriteria),
                                                         LeaveCriteria = leave.LeaveCriteria
                                                     }).ToList();
                        }

                        var staff = StaffDL.RetrieveBasicStaffByID(staffID);

                        staffLeave.FinancialYear = $"{financialYear.StartDate.FormatDate()} to {financialYear.EndDate.FormatDate()}";

                        staffLeave.BankHolidays = GetBankHolidaysForStaff(staff, financialYear);

                        staffLeave.NumberOfLeaveRemaining = staff.NumberOfLeaveDays - staffLeave.NumberOfLeaveTaken - staffLeave.BankHolidays.Count();
                    }
                    else
                    {
                        staffLeave = null;
                    }

                    return staffLeave;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffLeaveDetailsDto RetrieveStaffLeavesByFinancialYear(long staffID, long locationID, long financialYearID)
        {
            try
            {
                var pendingLeaveDaysTaken = ApprovalDL.RetrievePendingLeaveDaysTaken(staffID, locationID, financialYearID);

                using (var context = new DataContext())
                {
                    var staffLeave = new StaffLeaveDetailsDto();

                    staffLeave.NumberOfLeaveTaken = 0;
                    staffLeave.StaffLeave = new List<StaffLeaveDto>();

                    var financialYear = context.FinancialYears
                                                .Where(f => f.ID == financialYearID)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .FirstOrDefault();

                    if (financialYear != null)
                    {
                        var leaves = context.StaffLeaves
                                                .Where(s => s.StaffID == staffID &&
                                                            s.FinancialYearID == financialYear.ID &&
                                                            s.LeaveType != Enums.LeaveType.Cancelled)
                                                .ToList();

                        if (leaves.Any())
                        {
                            var deductibleLeaveTypes = Utility.Util.GetDeductibleLeaveTypes();

                            var nonDeductibleLeaveTypes = Utility.Util.GetNonDeductibleLeaveTypes();

                            var leaveDays = (from leave in leaves
                                             where deductibleLeaveTypes.Contains(leave.LeaveCriteria)
                                             select leave.NumberOfLeaveDaysTaken).Sum();

                            staffLeave.NumberOfLeaveTaken = leaveDays;

                            var nonLeaveDays = (from leave in leaves
                                                where nonDeductibleLeaveTypes.Contains(leave.LeaveCriteria)
                                                select leave.NumberOfLeaveDaysTaken).Sum();

                            staffLeave.NumberOfNonLeaveTaken = nonLeaveDays;

                            staffLeave.StaffLeave = (from leave in leaves
                                                     select new StaffLeaveDto
                                                     {
                                                         ID = leave.ID,
                                                         FinancialYearID = leave.FinancialYearID,
                                                         LocationID = leave.LocationID,
                                                         LocationName = leave.LocationName,
                                                         StaffEmail = leave.StaffEmail,
                                                         StaffID = leave.StaffID,
                                                         StaffKnownAs = leave.StaffKnownAs,
                                                         StaffName = leave.StaffName,
                                                         StaffUsername = leave.StaffUsername,
                                                         RequestedOn = leave.RequestedOn,
                                                         ApprovedOn = leave.ApprovedOn,
                                                         NumberOfLeaveDaysTaken = leave.NumberOfLeaveDaysTaken,
                                                         RequestedDays = JsonConvert.DeserializeObject<List<FinancialYearMonthDayDto>>(leave.RequestedDays),
                                                         LeaveType = leave.LeaveType.ToString(),
                                                         LeaveTypeDescription = Utility.Util.GetLeaveTypeDescription(leave.LeaveCriteria),
                                                         LeaveIsDeductible = Utility.Util.IsLeaveTypeDeductible(leave.LeaveCriteria),
                                                         LeaveCriteria = leave.LeaveCriteria
                                                     }).ToList();
                        }

                        var staff = StaffDL.RetrieveBasicStaffByID(staffID);

                        staffLeave.FinancialYear = $"{financialYear.StartDate.FormatDate()} to {financialYear.EndDate.FormatDate()}";

                        staffLeave.BankHolidays = GetBankHolidaysForStaff(staff, financialYear);

                        staffLeave.NumberOfPendingLeaveTaken = pendingLeaveDaysTaken;

                        staffLeave.NumberOfLeaveRemaining = staff.NumberOfLeaveDays - staffLeave.NumberOfLeaveTaken - staffLeave.BankHolidays.Count() - pendingLeaveDaysTaken;

                        staffLeave.NumberOfLeaveDays = staff.NumberOfLeaveDays;
                    }
                    else
                    {
                        staffLeave = null;
                    }

                    return staffLeave;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> GetBankHolidaysForStaff(StaffDto staff, FinancialYear financialYear)
        {
            var staffBankHolidays = new List<string>();

            List<Tuple<DateTime, string>> bankHolidays = new List<Tuple<DateTime, string>>();

            List<Tuple<DateTime, string>> _bankHolidays = new List<Tuple<DateTime, string>>();

            foreach (var month in financialYear.FinancialYearMonths)
            {
                foreach (var day in month.FinancialYearMonthDays)
                {
                    if (day.BankHoliday && month.FinancialYearID == financialYear.ID)
                    {
                        var _bankHolidayDate = new DateUtil();
                        _bankHolidayDate.Day = day.Day;
                        _bankHolidayDate.Month = Utility.Util.GetMonthNumber(month.Month);
                        _bankHolidayDate.Year = Convert.ToInt32(month.Year);

                        var bankHolidayDate = DateUtil.GetDate(_bankHolidayDate);

                        _bankHolidays.Add(new Tuple<DateTime, string>(bankHolidayDate, $"{day.Name} {day.Day} {month.Month}, {month.Year}"));
                    }
                }
            }

            _bankHolidays.OrderBy(bh => bh.Item1);

            if(!string.IsNullOrEmpty(staff.StartDate))
            {
                var _startDates = staff.StartDate.Split('/');
                var _startDate = new DateUtil()
                {
                    Day = Convert.ToInt32(_startDates[0]),
                    Month = Convert.ToInt32(_startDates[1]),
                    Year = Convert.ToInt32(_startDates[2]),
                };

                var startDate = DateUtil.GetDate(_startDate);
                if(startDate < _bankHolidays[0].Item1)
                {
                    bankHolidays = _bankHolidays;
                }
                else
                {
                    bankHolidays = (from bh in _bankHolidays
                                    where startDate < bh.Item1
                                    select bh).ToList();
                }
            }
            else
            {
                bankHolidays = (from bh in _bankHolidays
                                select bh).ToList();
            }

            if (!string.IsNullOrEmpty(staff.EndDate))
            {
                var _endDates = staff.EndDate.Split('/');
                var _endDate = new DateUtil()
                {
                    Day = Convert.ToInt32(_endDates[0]),
                    Month = Convert.ToInt32(_endDates[1]),
                    Year = Convert.ToInt32(_endDates[2]),
                };

                var endDate = DateUtil.GetDate(_endDate);
                if (endDate < _bankHolidays[_bankHolidays.Count - 1].Item1)
                {
                    bankHolidays = (from bh in bankHolidays
                                    where endDate > bh.Item1
                                    select bh).ToList();
                }               
            }

            staffBankHolidays = (from bh in bankHolidays
                                 select bh.Item2).ToList();

            return staffBankHolidays;
        }

        public static List<FinancialYear> RetrieveOpenedClosedFinancialYears()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var statuses = new List<string>();
                    statuses.Add(Enums.FinancialYearStatus.Opened.ToString());
                    statuses.Add(Enums.FinancialYearStatus.Closed.ToString());

                    var financialYears = context.FinancialYears
                                                .Where(f => statuses.Contains(f.Status))
                                                .ToList();
                    return financialYears;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYear> RetrieveLocationFinancialYears(long locationId)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var financialYears = context.FinancialYears
                                                .Where(f => f.LocationID == locationId)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .ToList();
                    return financialYears;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYear> RetrieveFinancialYears(string status)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var financialYears = context.FinancialYears
                                                .Where(f => f.Status == status)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .ToList();
                    return financialYears;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYear> CurrentFinancialYear(long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var opened = Enums.FinancialYearStatus.Opened.ToString();
                    var financialYears = context.FinancialYears
                                                .Where(f => f.Status == opened &&
                                                            f.LocationID == locationID)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .ToList();
                    return financialYears;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYear> CurrentFinancialYears()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var opened = Enums.FinancialYearStatus.Opened.ToString();
                    var financialYears = context.FinancialYears
                                                .Where(f => f.Status == opened)
                                                .Include(f => f.FinancialYearMonths.Select(m => m.FinancialYearMonthDays))
                                                .ToList();
                    return financialYears;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AutoStartLeave()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffLeaves = (from staffLeave in context.StaffLeaves
                                       where staffLeave.LeaveType == Enums.LeaveType.Pending
                                       select staffLeave).ToList();

                    if (staffLeaves.Any())
                    {
                        var today = DateUtil.Now();

                        var year = today.Year;
                        var month = String.Format("{0:MMMM}", today);
                        var day = today.Day;

                        staffLeaves.ForEach((staffLeave) =>
                        {
                            var requestedDays = JsonConvert.DeserializeObject<List<FinancialYearMonthDayDto>>(staffLeave.RequestedDays).OrderBy(d => d.Day).ToList();

                            var firstDay = requestedDays.FirstOrDefault();

                            if (year == Convert.ToInt32(firstDay.Year) && month.ToLower() == firstDay.Month.ToLower() && day == firstDay.Day)
                            {
                                staffLeave.LeaveType = Enums.LeaveType.Started;
                                context.Entry(staffLeave).State = EntityState.Modified;
                                context.SaveChanges();

                                var staff = context.Staffs.FirstOrDefault(s => s.ID == staffLeave.StaffID);
                                staff.LeaveType = Enums.LeaveType.Started;
                                context.Entry(staff).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AutoEndLeave()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffLeaves = (from staffLeave in context.StaffLeaves
                                       where staffLeave.LeaveType == Enums.LeaveType.Started
                                       select staffLeave).ToList();

                    if (staffLeaves.Any())
                    {
                        var today = DateUtil.Now();

                        var year = today.Year;
                        var month = String.Format("{0:MMMM}", today);
                        var day = today.Day;


                        staffLeaves.ForEach((staffLeave) =>
                        {
                            var requestedDays = JsonConvert.DeserializeObject<List<FinancialYearMonthDayDto>>(staffLeave.RequestedDays).OrderBy(d => d.Day).ToList();

                            var lastDay = requestedDays.LastOrDefault();

                            if (year == Convert.ToInt32(lastDay.Year) && month.ToLower() == lastDay.Month.ToLower() && day == lastDay.Day)
                            {
                                staffLeave.LeaveType = Enums.LeaveType.Completed;
                                context.Entry(staffLeave).State = EntityState.Modified;
                                context.SaveChanges();

                                var staff = context.Staffs.FirstOrDefault(s => s.ID == staffLeave.StaffID);
                                staff.LeaveType = Enums.LeaveType.None;
                                context.Entry(staff).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
