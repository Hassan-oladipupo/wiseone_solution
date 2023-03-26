using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using flexiCoreLibrary.Dto;
using Newtonsoft.Json;
using System.Data.Entity.Core.Objects;

namespace flexiCoreLibrary.Data
{
    public class SignInOutDL
    {
        public static List<StaffShift> RetrieveShiftForTheWeek(long staffID)
        {
            try
            {
                var today = (DateUtil.Now()).Date;

                using (var context = new DataContext())
                {
                    var shifts = (from staffShift in context.StaffShifts.Include(s => s.ShiftConfig)
                                  join staffConfig in context.ShiftConfigs on staffShift.ShiftConfigID equals staffConfig.ID
                                  where staffShift.StaffID == staffID && (today.Date >= DbFunctions.TruncateTime(staffConfig.StartDate) && today.Date <= DbFunctions.TruncateTime(staffConfig.EndDate))
                                  select staffShift)
                                 .ToList();

                    return shifts;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffShift> RetrieveShiftsForTheWeek(long locationID)
        {
            try
            {
                var today = (DateUtil.Now()).Date;

                using (var context = new DataContext())
                {
                    var shifts = (from staffShift in context.StaffShifts
                                  join staffConfig in context.ShiftConfigs on staffShift.ShiftConfigID equals staffConfig.ID
                                  where staffConfig.LocationID == locationID &&
                                        today.Date >= DbFunctions.TruncateTime(staffConfig.StartDate) &&
                                        today.Date <= DbFunctions.TruncateTime(staffConfig.EndDate)
                                  select staffShift).ToList();


                    return shifts;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOutDto RetrieveStaffClockedInOut(long staffID, string date)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var clockInOut = context.SignInOuts
                                            .Where(s => s.StaffID == staffID && s.Date == date)
                                            .AsEnumerable()
                                            .Select(c => new SignInOutDto()
                                            {
                                                ID = c.ID,
                                                ClockedIn = c.ClockedIn,
                                                ClockedInTime = string.Format("{0:H.mm}", c.ClockedInTime),
                                                ClockedOut = c.ClockedOut,
                                                ClockedOutTime = string.Format("{0:H.mm}", c.ClockedOutTime),
                                                Absent = c.Absent,
                                                Reason = c.AbsentReason
                                            })
                                            .FirstOrDefault();
                    return clockInOut;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOut ClockedInAlready(long staffID, string date)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var clockIn = context.SignInOuts.FirstOrDefault(s => s.StaffID == staffID && s.Date == date);
                    return clockIn;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOut ClockIn(SignInOut clockIn)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.SignInOuts.Add(clockIn);
                            context.SaveChanges();

                            var shiftFeeds = context.StaffShiftFeeds.Where(s => s.Date == clockIn.Date && s.StaffID == clockIn.StaffID);

                            if (shiftFeeds.Any())
                            {
                                var staffShiftFeed = shiftFeeds.FirstOrDefault();
                                staffShiftFeed.ClockedIn = true;
                                staffShiftFeed.ClockedInTime = clockIn.ClockedInTime;

                                context.Entry(staffShiftFeed).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            db.Commit();
                            return clockIn;
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
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

        public static bool ClockOut(SignInOut clockIn)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var signIn = context.SignInOuts.Where(s => s.ID == clockIn.ID).FirstOrDefault();
                            signIn.ClockedOut = clockIn.ClockedOut;
                            signIn.ClockedOutTime = clockIn.ClockedOutTime;
                            signIn.ClockOutLong = clockIn.ClockOutLong;
                            signIn.ClockOutLat = clockIn.ClockOutLat;
                            context.Entry(signIn).State = EntityState.Modified;
                            context.SaveChanges();

                            var shiftFeeds = context.StaffShiftFeeds.Where(s => s.Date == signIn.Date && s.StaffID == signIn.StaffID);

                            if (shiftFeeds.Any())
                            {
                                var staffShiftFeed = shiftFeeds.FirstOrDefault();
                                staffShiftFeed.ClockedOut = true;
                                staffShiftFeed.ClockedOutTime = clockIn.ClockedOutTime;

                                context.Entry(staffShiftFeed).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            db.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
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

        public static List<SignInOut> RetrieveSignInOuts(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);

                    var signInOuts = (from signIn in context.SignInOuts
                                      join staff in context.Staffs on signIn.StaffID equals staff.ID
                                      where staff.LocationID == filter.LocationID &&
                                            signIn.ClockedOut == true &&
                                            signIn.SignInOutDate >= fromDate && signIn.SignInOutDate <= toDate
                                      select signIn)
                                      .Include(s => s.Staff.Role)
                                      .Distinct();

                    return signInOuts.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<SignInOut> RetrieveStaffSignInOuts(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);

                    var signInOuts = context.SignInOuts
                                            .Where(x => x.StaffID == filter.StaffID &&
                                                        x.ClockedOut == true &&
                                                        x.SignInOutDate >= fromDate && x.SignInOutDate <= toDate
                                                  ).ToList();



                    return signInOuts;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ApproveShiftCancellation(SignInOutDto shiftCancelRequest)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            if (shiftCancelRequest.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString())
                            {
                                var staffShift = context.StaffShifts.FirstOrDefault(s => s.ID == shiftCancelRequest.StaffShiftID);
                                var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(staffShift.Shift);
                                staffShifts.ForEach(shift =>
                                {
                                    if (shift.Date == shiftCancelRequest.Date)
                                    {
                                        shift.Reason = shiftCancelRequest.Reason;
                                        shift.Absent = true;
                                        shift.DeclineReason = string.Empty;
                                    }
                                });

                                staffShift.Shift = JsonConvert.SerializeObject(staffShifts);
                                context.Entry(staffShift).State = EntityState.Modified;
                                context.SaveChanges();

                                var signIn = new SignInOut
                                {
                                    BreakTimeDuration = shiftCancelRequest.BreakTimeDuration,
                                    ClockedIn = false,
                                    ClockedOut = false,
                                    Date = shiftCancelRequest.Date,
                                    Day = shiftCancelRequest.Day,
                                    FolderTimeDuration = shiftCancelRequest.FolderTimeDuration,
                                    From = shiftCancelRequest.From,
                                    RoomID = shiftCancelRequest.Room.ID,
                                    SignInOutDate = DateUtil.Now(),
                                    StaffID = shiftCancelRequest.Staff.ID,
                                    StaffShiftID = shiftCancelRequest.StaffShiftID,
                                    To = shiftCancelRequest.To,
                                    OverTime = shiftCancelRequest.OverTime,
                                    Absent = true,
                                    AbsentReason = shiftCancelRequest.Reason
                                };

                                context.SignInOuts.Add(signIn);
                                context.SaveChanges();

                                var shiftFeeds = context.StaffShiftFeeds.Where(s => s.Date == signIn.Date && s.StaffID == signIn.StaffID);

                                if (shiftFeeds.Any())
                                {
                                    var staffShiftFeed = shiftFeeds.FirstOrDefault();
                                    staffShiftFeed.Absent = true;
                                    staffShiftFeed.AbsentReason = shiftCancelRequest.Reason;

                                    context.Entry(staffShiftFeed).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                var staffShift = context.StaffShifts.FirstOrDefault(s => s.ID == shiftCancelRequest.StaffShiftID);
                                var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(staffShift.Shift);
                                staffShifts.ForEach(shift =>
                                {
                                    if (shift.Date == shiftCancelRequest.Date)
                                    {
                                        shift.Reason = shiftCancelRequest.Reason;
                                        shift.DeclineReason = shiftCancelRequest.DeclineReason;
                                        shift.Absent = false;
                                    }
                                });

                                staffShift.Shift = JsonConvert.SerializeObject(staffShifts);
                                context.Entry(staffShift).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            //Update shift swap Approval
                            var approval = context.Approvals.FirstOrDefault(a => a.ID == shiftCancelRequest.ApprovalID);
                            approval.Status = shiftCancelRequest.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString() ? Enums.ApprovalStatus.Approved.ToString() : Enums.ApprovalStatus.Declined.ToString();
                            approval.ApprovedOn = DateUtil.Now();
                            approval.DeclineReason = shiftCancelRequest.ApprovalStatus == Enums.ApprovalStatus.Declined.ToString() ? shiftCancelRequest.DeclineReason : "";

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

        public static void PrepareAbsentReport()
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var today = (DateUtil.Now()).Date;

                            var shiftFeeds = context.StaffShiftFeeds.Where(x =>
                                                       x.ClockedIn == false &&
                                                       x.ClockedOut == false &&
                                                       x.Reported == false
                                                        ).ToList();

                            if (shiftFeeds.Any())
                            {
                                shiftFeeds.ForEach(feed =>
                                {
                                    feed.Reported = true;
                                    context.Entry(feed).State = EntityState.Modified;
                                    context.SaveChanges();

                                    var exclude = new List<long> { 18, 19 };

                                    if(!exclude.Contains(feed.StaffID))
                                    {
                                        var absentReport = new Report
                                        {
                                            Date = feed.Date,
                                            DateReported = DateUtil.Now(),
                                            Day = feed.Day,
                                            From = feed.From,
                                            Reason = feed.AbsentReason ?? "System Generated Reason: Absent without shift cancellation request.",
                                            RoomName = feed.RoomName,
                                            LocationName = feed.LocationName,
                                            StaffID = feed.StaffID,
                                            StaffShiftID = feed.StaffShiftID,
                                            StaffName = feed.StaffName,
                                            To = feed.To,
                                            ReportType = Enums.ReportType.AbsentReport
                                        };
                                        context.AbsentReports.Add(absentReport);
                                        context.SaveChanges();
                                    }
                                    
                                });

                                transaction.Commit();
                            }
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

        public static void PrepareMissingClockOutReport()
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var today = (DateUtil.Now()).Date;

                            var shiftFeeds = context.StaffShiftFeeds.Where(x =>
                                                       x.ClockedIn == true &&
                                                       x.ClockedOut == false &&
                                                       x.Reported == false
                                                        ).ToList();

                            if (shiftFeeds.Any())
                            {
                                shiftFeeds.ForEach(feed =>
                                {
                                    feed.Reported = true;
                                    context.Entry(feed).State = EntityState.Modified;
                                    context.SaveChanges();

                                    var missingClockOutReport = new Report
                                    {
                                        Date = feed.Date,
                                        DateReported = DateUtil.Now(),
                                        Day = feed.Day,
                                        From = feed.From,
                                        Reason = "System Generated Reason: Staff did not clock out.",
                                        RoomName = feed.RoomName,
                                        LocationName = feed.LocationName,
                                        StaffID = feed.StaffID,
                                        StaffShiftID = feed.StaffShiftID,
                                        StaffName = feed.StaffName,
                                        To = feed.To,
                                        ReportType = Enums.ReportType.MissingClockOutReport
                                    };

                                    context.AbsentReports.Add(missingClockOutReport);
                                    context.SaveChanges();
                                });

                                transaction.Commit();
                            }
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

        public static List<Report> RetrieveAbsentReport(Enums.ReportType reportType = Enums.ReportType.AbsentReport)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var today = DateUtil.Now().ToString("dd/MM/yyyy");
                    var reports = new List<Report>();
                    if (reportType == Enums.ReportType.AbsentReport)
                    {
                        reports = context.AbsentReports
                                        .Where(x => x.ReportType == Enums.ReportType.AbsentReport &&
                                                    x.Date == today &&
                                                    x.MailSent == false)
                                        .ToList();

                    }
                    else
                    {
                        reports = context.AbsentReports
                                        .Where(x => x.ReportType == Enums.ReportType.MissingClockOutReport &&
                                                    x.Date == today &&
                                                    x.MailSent == false)
                                        .ToList();
                    }
                    return reports;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateReports(List<Report> reports)
        {
            using (var context = new DataContext())
            {
                reports.ForEach((report) =>
                {
                    var r = context.AbsentReports.FirstOrDefault(x => x.ID == report.ID);
                    r.MailSent = true;
                    context.Entry(r).State = EntityState.Modified;
                    context.SaveChanges();
                });
            }
        }

        public static List<Report> RetrieveReports(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);



                    var reportType = (Enums.ReportType)Enum.Parse(typeof(Enums.ReportType), filter.Type);

                    var reports = context.AbsentReports.Where(x => x.DateReported >= fromDate &&
                                                                   x.DateReported < toDate &&
                                                                   x.ReportType == reportType)
                                                       .ToList();
                    return reports;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ReportDto> RetrieveReportDetail(ReportSummaryDto summary)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var reportType = (Enums.ReportType)Enum.Parse(typeof(Enums.ReportType), summary.ReportType);

                    var reports = (from report in context.AbsentReports
                                   where report.ReportType == reportType && report.Date == summary.Date
                                   select new ReportDto
                                   {
                                       From = report.From.ToString(),
                                       To = report.To.ToString(),
                                       LocationName = report.LocationName,
                                       Reason = report.Reason,
                                       RoomName = report.RoomName,
                                       StaffName = report.StaffName
                                   }).ToList();

                    return reports;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffReportDto> RetrieveStaffReportDetail(StaffReportSummaryDto summary)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var reports = (from report in context.AbsentReports
                                   where summary.ReportIDs.Contains(report.StaffID)
                                   select new StaffReportDto
                                   {
                                       From = report.From.ToString(),
                                       To = report.To.ToString(),
                                       LocationName = report.LocationName,
                                       Reason = report.Reason,
                                       RoomName = report.RoomName,
                                       Date = report.Date
                                   }).ToList();

                    return reports;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CancelledShiftExists(long staffID, string date)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var signInOut = context.SignInOuts.FirstOrDefault(s => s.StaffID == staffID && s.Date == date && s.Absent == true);

                    return signInOut == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
