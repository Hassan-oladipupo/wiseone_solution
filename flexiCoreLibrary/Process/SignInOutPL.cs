using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using flexiCoreLibrary.Utility;

namespace flexiCoreLibrary.Process
{
    public class SignInOutPL
    {
        public static ShiftDto RetrieveShiftForTheDay(long staffID)
        {
            try
            {
                var today = (DateUtil.Now()).DayOfWeek.ToString().ToLower();               

                var todaysShift = new ShiftDto();

                var weeklyShifts = SignInOutDL.RetrieveShiftForTheWeek(staffID);

                if (weeklyShifts.Any())
                {
                    foreach(var weeklyShift in weeklyShifts)
                    {
                        var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(weeklyShift.Shift);

                        var shift = staffShifts.FirstOrDefault(s => s.Configure == true && s.Day.ToLower() == today);

                        if(shift != null)
                        {
                            var clockInOutData = SignInOutDL.RetrieveStaffClockedInOut(staffID, shift.Date);

                            todaysShift = shift;
                            todaysShift.StaffID = weeklyShift.StaffID;
                            todaysShift.StaffShiftID = weeklyShift.ID;
                            todaysShift.DeclineReason = todaysShift.DeclineReason ?? string.Empty;
                            todaysShift.OverTimeData = shift.OverTime != 0.0m ? $"{(Convert.ToInt32(shift.OverTime) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(shift.OverTime) % 60).ToString().PadLeft(2, '0')}" : string.Empty;
                            todaysShift.ClockInOutData = clockInOutData;
                            break;
                        }                        
                    }                    
                }

                return todaysShift;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapDto> RetrieveOverTimeForApproval(SearchFilter filter)
        {
            try
            {
                var overTimes = new List<ShiftSwapDto>();

                var staffs = StaffDL.RetrieveStaff();

                filter.Type = Enums.ApprovalType.ShiftOverTime.ToString();
                var allOverTimes = ApprovalDL.RetrieveFilteredApprovals(filter);

                if (allOverTimes.Any())
                {
                    allOverTimes.ForEach(approval =>
                    {
                        var overTime = JsonConvert.DeserializeObject<ShiftSwapDto>(approval.Obj);
                        var staff = staffs.FirstOrDefault(s => s.ID == overTime.StaffID);
                        overTime.StaffName = string.Format("{0} {1}", staff.FirstName, staff.Surname);
                        overTime.OverTimeData = $"{(Convert.ToInt32(overTime.OverTime) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(overTime.OverTime) % 60).ToString().PadLeft(2, '0')}";
                        overTime.ApprovalID = approval.ID;
                        overTime.ApprovalStatus = approval.Status;
                        overTime.RequestedOn = string.Format("{0:g}", approval.RequestedOn.Value);

                        overTimes.Add(overTime);
                    });
                }

                return overTimes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LogCancelShiftForApproval(SignInOutDto clockIn)
        {
            try
            {
                var requestExists = false;

                var today = DateUtil.Now();

                SearchFilter filter = new SearchFilter()
                {
                    Type = Enums.ApprovalType.CancelShift.ToString(),
                    Status = Enums.ApprovalStatus.Pending.ToString(),
                    FromDate = new DateUtil { Day = today.Day, Month = today.Month, Year = today.Year},
                    ToDate = new DateUtil { Day = today.Day, Month = today.Month, Year = today.Year },
                };               

                var pendingApprovals = ApprovalDL.RetrieveFilteredApprovals(filter);                

                if (pendingApprovals.Any())
                {
                    foreach(var pendingApproval in pendingApprovals)
                    {
                        var shiftCancelRequest = JsonConvert.DeserializeObject<SignInOutDto>(pendingApproval.Obj);

                        if(shiftCancelRequest.Staff.ID == clockIn.Staff.ID && shiftCancelRequest.Date == clockIn.Date)
                        {
                            requestExists = true;
                            break;
                        }                        
                    }
                }

                if(!requestExists)
                {
                    //Add by msalem 
                    clockIn.Staff = new StaffDto()
                    {
                        ID = clockIn.Staff.ID,
                        Picture = clockIn.Staff.Picture,
                        KnownAs = clockIn.Staff.KnownAs,
                        Location = new LocationDto()
                        {
                            ID = clockIn.Staff.Location.ID,
                            Name = clockIn.Staff.Location.Name
                        }
                    };
                    clockIn.Room = new ClassRoomDto()
                    {
                        ID = clockIn.Room.ID,
                        Name = clockIn.Room.Name
                    };
                    var approval = new Approval
                    {
                        Obj = JsonConvert.SerializeObject(clockIn,new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                        }),
                        Type = Enums.ApprovalType.CancelShift.ToString(),
                        Status = Enums.ApprovalStatus.Pending.ToString(),
                        RequestedOn = DateUtil.Now()
                    };

                    return ApprovalDL.Save(approval);
                }
                else
                {
                    return true;
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
                return SignInOutDL.ApproveShiftCancellation(shiftCancelRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOutDto ClockIn(SignInOutDto clockIn)
        {
            try
            {
                var clockedIn = SignInOutDL.ClockedInAlready(clockIn.Staff.ID, clockIn.Date);
                if (clockedIn != null)
                {
                    clockIn.ID = clockedIn.ID;
                    clockIn.ClockedIn = clockedIn.ClockedIn;
                    clockIn.ClockedInTime = string.Format("{0:H.mm}", clockedIn.ClockedInTime);
                    return clockIn;
                }
                else
                {                    
                    var location = LocationDL.RetrieveShiftLocationByID(clockIn.StaffShiftID);

                    var lowerAcceptableLatitude = location.Latitude - 0.02m;
                    var higherAcceptableLatitude = location.Latitude + 0.02m;

                    var lowerAcceptableLongitude = location.Longitude - 0.02m;
                    var higherAcceptableLongitude = location.Longitude + 0.02m;
                    
                    if (clockIn.Staff.Location.Latitude >= lowerAcceptableLatitude && clockIn.Staff.Location.Latitude <= higherAcceptableLatitude && clockIn.Staff.Location.Longitude >= lowerAcceptableLongitude && clockIn.Staff.Location.Longitude <= higherAcceptableLongitude)
                    {
                        var shiftEndTime = TimeSpan.FromHours(Convert.ToDouble(clockIn.To));
                        var shiftStartTime = TimeSpan.FromHours(Convert.ToDouble(clockIn.From));
                        var allowedClockInTime = DateUtil.Now().TimeOfDay.Add(new TimeSpan(0, 18, 0));

                        if (allowedClockInTime >= shiftStartTime && allowedClockInTime <= shiftEndTime)
                        {
                            var signIn = new SignInOut
                            {
                                BreakTimeDuration = clockIn.BreakTimeDuration,
                                ClockedIn = true,
                                ClockedInTime = DateUtil.Now(),
                                ClockedOut = false,
                                Date = clockIn.Date,
                                Day = clockIn.Day,
                                FolderTimeDuration = clockIn.FolderTimeDuration,
                                From = clockIn.From,
                                RoomID = clockIn.Room.ID,
                                SignInOutDate = DateUtil.Now(),
                                StaffID = clockIn.Staff.ID,
                                StaffShiftID = clockIn.StaffShiftID,
                                To = clockIn.To,
                                OverTime = clockIn.OverTime,
                                LocationLat = location.Latitude,
                                LocationLong = location.Longitude,
                                ClockInLat = clockIn.Staff.Location.Latitude,
                                ClockInLong = clockIn.Staff.Location.Longitude
                            };

                            var savedClockIn = SignInOutDL.ClockIn(signIn);

                            if (savedClockIn.ID == 0)
                            {
                                throw new Exception("Clock In request failed. Kindly try again.");
                            }
                            else
                            {
                                clockIn.ID = savedClockIn.ID;
                                clockIn.ClockedIn = savedClockIn.ClockedIn;
                                clockIn.ClockedInTime = string.Format("{0:H.mm}", savedClockIn.ClockedInTime);

                                return clockIn;
                            }
                        }
                        else if (allowedClockInTime > shiftEndTime)
                        {
                            throw new Exception(string.Format("Your shift time is exceeded. Allowed clock in time is between {0} HRS and {1} HRS", clockIn.From, clockIn.To));
                        }
                        else
                        {
                            throw new Exception("Allowed clock in time starts from at least 15 minutes to your shift start time");
                        }
                    }
                    else
                    {
                        throw new Exception($"Clock in: {clockIn.Staff.Location.Latitude}, {clockIn.Staff.Location.Longitude}  not allowed outside {location.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOutDto ClockOut(SignInOutDto clockOut)
        {
            try
            {
                var location = LocationDL.RetrieveShiftLocationByID(clockOut.StaffShiftID);

                var lowerAcceptableLatitude = location.Latitude - 0.02m;
                var higherAcceptableLatitude = location.Latitude + 0.02m;

                var lowerAcceptableLongitude = location.Longitude - 0.02m;
                var higherAcceptableLongitude = location.Longitude + 0.02m;

                if (clockOut.Staff.Location.Latitude >= lowerAcceptableLatitude && clockOut.Staff.Location.Latitude <= higherAcceptableLatitude && clockOut.Staff.Location.Longitude >= lowerAcceptableLongitude && clockOut.Staff.Location.Longitude <= higherAcceptableLongitude)
                {
                    var signOut = new SignInOut
                    {
                        ID = clockOut.ID,
                        ClockedOut = true,
                        ClockedOutTime = DateUtil.Now(),
                        ClockOutLat = clockOut.Staff.Location.Latitude,
                        ClockOutLong = clockOut.Staff.Location.Longitude,
                    };

                    var clockedOut = SignInOutDL.ClockOut(signOut);

                    if (clockedOut)
                    {
                        clockOut.ClockedOut = signOut.ClockedOut;
                        clockOut.ClockedOutTime = string.Format("{0:H.mm}", signOut.ClockedOutTime);
                    }
                    else
                    {
                        throw new Exception("Clock out request failed. Kindly try again.");
                    }

                    return clockOut;
                }
                else
                {
                    throw new Exception($"Clock in: {clockOut.Staff.Location.Latitude}, {clockOut.Staff.Location.Longitude}  not allowed outside {location.Name}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffSignInDto> RetrieveSignInOuts(SearchFilter filter)
        {
            try
            {
                var staffSignIns = new List<StaffSignInDto>();

                var signInOuts = SignInOutDL.RetrieveSignInOuts(filter);

                if (signInOuts.Any())
                {
                    var uniqueStaffIDs = signInOuts.Select(x => x.StaffID).Distinct().ToList();
                    uniqueStaffIDs.ForEach(staffID =>
                    {
                        var expectedTotalHours = new TimeSpan(0, 0, 0);
                        var actualTotalHours = new TimeSpan(0, 0, 0);
                        var overTimeTotalHours = 0.0M;

                        var staffSignInOuts = signInOuts.Where(x => x.StaffID == staffID).ToList();
                        staffSignInOuts.ForEach(sio =>
                        {
                            var shiftEndTime = TimeSpan.FromHours(Convert.ToDouble(sio.To));
                            var shiftStartTime = TimeSpan.FromHours(Convert.ToDouble(sio.From));
                            var clockInTime = sio.ClockedInTime.Value.TimeOfDay;
                            var clockOutTime = sio.ClockedOutTime.Value.TimeOfDay;

                            var expectedTotalHour = shiftEndTime.Subtract(shiftStartTime);
                            expectedTotalHours = expectedTotalHours.Add(expectedTotalHour);

                            var startTime = clockInTime > shiftStartTime ? clockInTime : shiftStartTime;
                            var endTime = clockOutTime > shiftEndTime ? shiftEndTime : clockOutTime;
                            var actualTotalHour = endTime.Subtract(startTime);
                            actualTotalHours += actualTotalHour;

                            overTimeTotalHours += sio.OverTime;
                        });

                        var staff = staffSignInOuts.FirstOrDefault().Staff;
                        var staffSignIn = new StaffSignInDto
                        {
                            Staff = new StaffDto
                            {
                                ID = staff.ID,
                                Surname = staff.Surname,
                                MiddleName = staff.MiddleName,
                                FirstName = staff.FirstName,
                                StaffID = staff.StaffID,
                                Role = new RoleDto { Name = staff.Role.Name }
                            },
                            ExpectedTotalHours = $"{(int)expectedTotalHours.TotalHours}.{expectedTotalHours.Minutes}",
                            ActualTotalHours = $"{(int)actualTotalHours.TotalHours}.{actualTotalHours.Minutes}",
                            OverTimeTotalHours = $"{(Convert.ToInt32(overTimeTotalHours) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(overTimeTotalHours) % 60).ToString().PadLeft(2, '0')}"
                        };
                        staffSignIns.Add(staffSignIn);
                    });
                }

                return staffSignIns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffSignInDetailDto> RetrieveStaffSignInOuts(SearchFilter filter)
        {
            try
            {
                var staffSignIns = new List<StaffSignInDetailDto>();

                var signInOuts = SignInOutDL.RetrieveStaffSignInOuts(filter);

                if (signInOuts.Any())
                {
                    signInOuts.ForEach(sio =>
                    {
                        var approved = Enums.ApprovalStatus.Approved.ToString();

                        var staffSignIn = new StaffSignInDetailDto();
                        staffSignIn.Date = sio.Date;
                        staffSignIn.ShiftEndTime = sio.To.ToString();
                        staffSignIn.ShiftStartTime = sio.From.ToString();
                        staffSignIn.ClockInTime = string.Format("{0:H.mm}", sio.ClockedInTime);
                        staffSignIn.ClockOutTime = string.Format("{0:H.mm}", sio.ClockedOutTime);
                        staffSignIn.ApprovedOverTime = $"{(Convert.ToInt32(sio.OverTime) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(sio.OverTime) % 60).ToString().PadLeft(2, '0')}";

                        staffSignIns.Add(staffSignIn);
                    });
                }

                return staffSignIns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<SignInOutDto> RetrieveShiftCancelRequests(SearchFilter filter)
        {
            try
            {
                filter.Type = Enums.ApprovalType.CancelShift.ToString();

                var pendingApprovals = ApprovalDL.RetrieveFilteredApprovals(filter);

                var shiftCancelRequests = new List<SignInOutDto>();

                if (pendingApprovals.Any())
                {
                    pendingApprovals.ForEach(approval =>
                    {
                        var shiftCancelRequest = JsonConvert.DeserializeObject<SignInOutDto>(approval.Obj);

                        var shiftLocation = LocationDL.RetrieveShiftLocationByID(shiftCancelRequest.StaffShiftID);

                        if (shiftLocation.ID == filter.LocationID)
                        {
                            shiftCancelRequest.StaffName = $"{shiftCancelRequest.Staff.FirstName} {shiftCancelRequest.Staff.MiddleName} {shiftCancelRequest.Staff.Surname}";
                            shiftCancelRequest.DeclineReason = approval.DeclineReason ?? string.Empty;
                            shiftCancelRequest.ApprovalID = approval.ID;
                            shiftCancelRequest.ApprovalStatus = approval.Status;
                            shiftCancelRequest.RequestedOn = string.Format("{0:g}", approval.RequestedOn);
                            shiftCancelRequest.ApprovedOn = approval.ApprovedOn != null ? string.Format("{0:g}", approval.ApprovedOn) : string.Empty;
                            // added by msalem
                            shiftCancelRequest.Staff.Picture = shiftCancelRequest.Staff.Picture.GetPicture();
                            

                            shiftCancelRequests.Add(shiftCancelRequest);
                        }

                    });
                }

                return shiftCancelRequests;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void PrepareEndOfDayReport(Enums.ReportType reportType = Enums.ReportType.AbsentReport)
        {
            try
            {
                if (reportType == Enums.ReportType.AbsentReport)
                {
                    SignInOutDL.PrepareAbsentReport();
                }
                else
                {
                    SignInOutDL.PrepareMissingClockOutReport();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ReportSummaryDto> RetrieveReportSummary(SearchFilter filter)
        {
            try
            {
                var summaryReports = new List<ReportSummaryDto>();

                var reports = SignInOutDL.RetrieveReports(filter);

                if (reports.Any())
                {
                    reports.ForEach((r) =>
                    {
                        var reportIdx = summaryReports.FindIndex(s => s.Date.Equals(r.Date));
                        if (reportIdx == -1)
                        {
                            var summaryReport = new ReportSummaryDto
                            {
                                Date = r.Date,
                                Day = r.Day,
                                NumberOfStaff = 1,
                                ReportType = filter.Type
                            };
                            summaryReports.Add(summaryReport);
                        }
                        else
                        {
                            summaryReports[reportIdx].NumberOfStaff += 1;
                        }
                    });
                }

                return summaryReports;
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
                var reports = SignInOutDL.RetrieveReportDetail(summary);
                return reports;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffReportSummaryDto> RetrieveStaffReportSummary(SearchFilter filter)
        {
            try
            {
                var summaryReports = new List<StaffReportSummaryDto>();

                var reports = SignInOutDL.RetrieveReports(filter);

                if (reports.Any())
                {
                    reports.ForEach((r) =>
                    {
                        var reportIdx = summaryReports.FindIndex(s => s.StaffID == r.StaffID);
                        if (reportIdx == -1)
                        {
                            var summaryReport = new StaffReportSummaryDto
                            {
                                StaffID = r.StaffID,
                                AbsentTimes = 1,
                                StaffName = r.StaffName
                            };
                            summaryReport.ReportIDs.Add(r.StaffID);
                            summaryReports.Add(summaryReport);
                        }
                        else
                        {
                            summaryReports[reportIdx].AbsentTimes += 1;
                            summaryReports[reportIdx].ReportIDs.Add(r.StaffID);
                        }
                    });
                }

                return summaryReports;
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
                var reports = SignInOutDL.RetrieveStaffReportDetail(summary);
                return reports;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
