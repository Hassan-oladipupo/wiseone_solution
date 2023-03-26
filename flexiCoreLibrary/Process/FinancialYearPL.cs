using flexiCoreLibrary.Extensions;
using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using flexiCoreLibrary.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Process
{
    public class FinancialYearPL
    {
        public static bool Save(FinancialYearDto financialYearDto)
        {
            try
            {
                var financialExists = FinancialYearDL.FinancialYearExists(financialYearDto.StartDate, financialYearDto.EndDate, financialYearDto.Location.ID);

                if (!financialExists)
                {
                    var financialYear = new FinancialYear
                    {
                        CreatedOn = DateUtil.Now(),
                        LocationID = financialYearDto.Location.ID,
                        EndDate = financialYearDto.EndDate.FormatDate(),
                        ExcludeMonths = JsonConvert.SerializeObject(financialYearDto.ExcludeMonths),
                        LeavePriorNotice = financialYearDto.LeavePriorNotice,
                        StartDate = financialYearDto.StartDate.FormatDate(),
                        Status = Enums.FinancialYearStatus.Closed.ToString(),
                        FinancialYearMonths = (from month in financialYearDto.Months
                                               select new FinancialYearMonth
                                               {
                                                   Exclude = month.Exclude,
                                                   Label = month.Label,
                                                   Month = month.Month,
                                                   Year = month.Year,
                                                   FinancialYearMonthDays = (from day in month.Days
                                                                             select new FinancialYearMonthDay
                                                                             {
                                                                                 Available = day.Available,
                                                                                 BankHoliday = day.BankHoliday,
                                                                                 Day = day.Day,
                                                                                 Name = day.Name,
                                                                                 NumberOfStaff = day.NumberOfStaff,
                                                                                 Staff = JsonConvert.SerializeObject(day.Staff)
                                                                             }).ToList()
                                               }).ToList()
                    };

                    return FinancialYearDL.Save(financialYear);
                }
                else
                {
                    var msg = string.Format("Financial Year {0}-{1} exists already for the selected location.", financialYearDto.StartDate.FormatDate(), financialYearDto.EndDate.FormatDate());

                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(FinancialYearDto financialYearDto)
        {
            try
            {
                var financialYear = new FinancialYear
                {
                    ID = financialYearDto.ID,
                };

                return FinancialYearDL.Delete(financialYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ToggleStatus(FinancialYearDto financialYearDto)
        {
            try
            {
                var financialYear = new FinancialYear
                {
                    ID = financialYearDto.ID,
                    LocationID = financialYearDto.Location.ID,
                    Status = financialYearDto.Status,
                };

                return FinancialYearDL.ToggleStatus(financialYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(FinancialYearDto financialYearDto)
        {
            try
            {
                var financialYear = new FinancialYear
                {
                    ID = financialYearDto.ID,
                    LeavePriorNotice = financialYearDto.LeavePriorNotice,
                };

                return FinancialYearDL.Update(financialYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LogLeaveRequestForApproval(LeaveRequestDto leaveRequest)
        {
            try
            {
                var dateRequested = DateUtil.Now();

                var leaveFirstDay = leaveRequest.RequestedDays.FirstOrDefault();

                int day = leaveFirstDay.Day;
                int month = Util.GetMonthNumber(leaveFirstDay.Month);
                int year = Convert.ToInt32(leaveFirstDay.Year);
                var leaveFirstDate = DateUtil.Now(new DateTime(year, month, day));

                var noticePeriod = (leaveFirstDate - dateRequested).TotalDays;
                var noticePeriodGiven = (double)(2 * leaveRequest.LeaveDaysTaken);

                if (noticePeriod >= noticePeriodGiven)
                {
                    //added by msalem 
                    leaveRequest.StaffLocation = new LocationDto()
                    {
                        ID = leaveRequest.StaffLocation.ID,
                        Name = leaveRequest.StaffLocation.Name
                    };

                    foreach (var item in leaveRequest.RequestedDays)
                    {
                        var listRequestdays =new List<StaffLeaveDto>();
                        foreach (var staff1 in item.Staff)
                        {
                            listRequestdays.Add(new StaffLeaveDto()
                            {
                                ID = staff1.ID,
                                StaffKnownAs = staff1.StaffKnownAs
                            });
                        }

                        item.Staff = listRequestdays;
                    }

                    var approval = new Approval
                    {
                        Obj = JsonConvert.SerializeObject(leaveRequest,new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                        }),
                        Type = Enums.ApprovalType.ApproveLeave.ToString(),
                        Status = Enums.ApprovalStatus.Pending.ToString(),
                        RequestedOn = dateRequested
                    };

                    return ApprovalDL.Save(approval);
                }
                else
                {
                    throw new Exception($"INVALID LEAVE REQUEST! Leave notice should be at least twice as long as the amount of holiday you want to take. You can read more at: https://www.nidirect.gov.uk/articles/taking-your-holidays. Hence, you need to give atleast {noticePeriodGiven} days notice period before your requested leave with start date of {leaveFirstDate.ToString("dddd, dd MMMM yyyy")} will be valid.");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LogCancelLeaveForApproval(StaffLeaveDto staffLeave)
        {
            try
            {
                //added by msalem 
                foreach (var item1 in staffLeave.RequestedDays)
                {
                    item1.Staff = null;
                    item1.NumberOfStaff = 0;
                    item1.FinancialYearMonthID = 0;
                    item1.FinancialYearMonth = null;
                    item1.Available = false;
                }

                var approval = new Approval
                {
                    Obj = JsonConvert.SerializeObject(staffLeave,new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                    }),
                    Type = Enums.ApprovalType.CancelLeave.ToString(),
                    Status = Enums.ApprovalStatus.Pending.ToString(),
                    RequestedOn = DateUtil.Now()
                };

                return ApprovalDL.Save(approval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeclineLeaveRequest(LeaveApprovalDto leaveApproval)
        {
            try
            {
                var approval = new Approval
                {
                    ID = leaveApproval.Approval.ApprovalID,
                    Status = Enums.ApprovalStatus.Declined.ToString(),
                    DeclineReason = leaveApproval.LeaveRequest.DeclineReason,
                    ApprovedOn = DateUtil.Now()
                };

                return ApprovalDL.Update(approval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeclineCancelLeaveRequest(StaffLeaveDto leaveApproval)
        {
            try
            {
                var approval = new Approval
                {
                    ID = leaveApproval.Approval.ApprovalID,
                    Status = Enums.ApprovalStatus.Declined.ToString(),
                    ApprovedOn = DateUtil.Now()
                };

                return ApprovalDL.Update(approval);
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
                return FinancialYearDL.ApproveLeaveRequest(leaveApproval);
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
                return FinancialYearDL.LogLeaveRequestForSecondLevelApproval(leaveApproval);
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
                return FinancialYearDL.SaveLeaveRequest(leaveRequest);
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
                return FinancialYearDL.ApproveCancelLeaveRequest(leaveApproval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteLeaveRequest(StaffLeaveDto staffLeave)
        {
            try
            {
                return FinancialYearDL.DeleteLeaveRequest(staffLeave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYearDto> RetrieveLocationFinancialYears(long locationId)
        {
            try
            {
                var fy = FinancialYearDL.RetrieveLocationFinancialYears(locationId);

                var financialYears = (from financialYear in fy
                                      select new FinancialYearDto
                                      {
                                          ID = financialYear.ID,
                                          LeavePriorNotice = financialYear.LeavePriorNotice,
                                          StartDate = financialYear.StartDate.FormatDate(),
                                          EndDate = financialYear.EndDate.FormatDate(),
                                          Status = financialYear.Status,
                                          Months = (from month in financialYear.FinancialYearMonths
                                                        //where month.Exclude == false
                                                    select new FinancialYearMonthDto
                                                    {
                                                        ID = month.ID,
                                                        Label = month.Label,
                                                        Month = month.Month,
                                                        Year = month.Year,
                                                        FinancialYearID = financialYear.ID,
                                                        Exclude = month.Exclude,
                                                        Days = (from day in month.FinancialYearMonthDays
                                                                select new FinancialYearMonthDayDto
                                                                {
                                                                    ID = day.ID,
                                                                    Available = day.Available,
                                                                    BankHoliday = day.BankHoliday,
                                                                    Day = day.Day,
                                                                    Name = day.Name,
                                                                    NumberOfStaff = day.NumberOfStaff,
                                                                    Month = month.Month,
                                                                    Year = month.Year.ToString(),
                                                                    Staff = day.Staff != "null" ? JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff) : new List<StaffLeaveDto>()
                                                                }).ToList()
                                                    }).ToList(),
                                          LeaveTypes = Utility.Util.GetLeaveTypes()
                                      }).ToList();

                return financialYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYearDto> RetrieveFinancialYears(string status)
        {
            try
            {
                var locations = LocationDL.RetrieveLocations();

                var fy = FinancialYearDL.RetrieveFinancialYears(status);

                var financialYears = (from financialYear in fy
                                      select new FinancialYearDto
                                      {
                                          ID = financialYear.ID,
                                          Location = new LocationDto
                                          {
                                              ID = locations.Where(l => l.ID == financialYear.LocationID).FirstOrDefault().ID,
                                              Name = locations.Where(l => l.ID == financialYear.LocationID).FirstOrDefault().Name,
                                          },
                                          CreatedOn = string.Format("{0:g}", financialYear.CreatedOn),
                                          EndDate = financialYear.EndDate.FormatDate(),
                                          ExcludeMonths = financialYear.ExcludeMonths != null ? JsonConvert.DeserializeObject<List<ExcludeMonthDto>>(financialYear.ExcludeMonths) : new List<ExcludeMonthDto>(),
                                          ExcludeMonthsDetails = GetExcludeMonthDetails(financialYear.ExcludeMonths),
                                          LeavePriorNotice = financialYear.LeavePriorNotice,
                                          StartDate = financialYear.StartDate.FormatDate(),
                                          Status = financialYear.Status,
                                          Months = (from month in financialYear.FinancialYearMonths
                                                    select new FinancialYearMonthDto
                                                    {
                                                        ID = month.ID,
                                                        Exclude = month.Exclude,
                                                        Label = month.Label,
                                                        Month = month.Month,
                                                        Year = month.Year,
                                                        Days = (from day in month.FinancialYearMonthDays
                                                                select new FinancialYearMonthDayDto
                                                                {
                                                                    ID = day.ID,
                                                                    Available = day.Available,
                                                                    BankHoliday = day.BankHoliday,
                                                                    Day = day.Day,
                                                                    Name = day.Name,
                                                                    NumberOfStaff = day.NumberOfStaff,
                                                                    Staff = day.Staff != "null" ? JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff) : new List<StaffLeaveDto>()
                                                                }).ToList()
                                                    }).ToList()
                                      }).ToList();

                return financialYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYearDto> CurrentFinancialYear(long locationID)
        {
            try
            {
                var fy = FinancialYearDL.CurrentFinancialYear(locationID);

                var financialYears = (from financialYear in fy
                                      select new FinancialYearDto
                                      {
                                          ID = financialYear.ID,
                                          LeavePriorNotice = financialYear.LeavePriorNotice,
                                          StartDate = financialYear.StartDate.FormatDate(),
                                          EndDate = financialYear.EndDate.FormatDate(),
                                          Months = (from month in financialYear.FinancialYearMonths
                                                        //where month.Exclude == false
                                                    select new FinancialYearMonthDto
                                                    {
                                                        ID = month.ID,
                                                        Label = month.Label,
                                                        Month = month.Month,
                                                        Year = month.Year,
                                                        Exclude = month.Exclude,
                                                        FinancialYearID = financialYear.ID,
                                                        Days = (from day in month.FinancialYearMonthDays
                                                                select new FinancialYearMonthDayDto
                                                                {
                                                                    ID = day.ID,
                                                                    Available = day.Available,
                                                                    BankHoliday = day.BankHoliday,
                                                                    Day = day.Day,
                                                                    Name = day.Name,
                                                                    NumberOfStaff = day.NumberOfStaff,
                                                                    Month = month.Month,
                                                                    Year = month.Year.ToString(),
                                                                    Staff = day.Staff != "null" ? JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff) : new List<StaffLeaveDto>()
                                                                }).ToList()
                                                    }).ToList(),
                                          LeaveTypes = Utility.Util.GetLeaveTypes()
                                      }).ToList();

                return financialYears;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYearDto> CurrentFinancialYears(List<FinancialYear> fy)
        {
            try
            {
                var financialYears = (from financialYear in fy
                                      select new FinancialYearDto
                                      {
                                          ID = financialYear.ID,
                                          LeavePriorNotice = financialYear.LeavePriorNotice,
                                          StartDate = financialYear.StartDate.FormatDate(),
                                          EndDate = financialYear.EndDate.FormatDate(),
                                          Location = new LocationDto() { ID = financialYear.LocationID },
                                          Months = (from month in financialYear.FinancialYearMonths
                                                    select new FinancialYearMonthDto
                                                    {
                                                        ID = month.ID,
                                                        Label = month.Label,
                                                        Month = month.Month,
                                                        Year = month.Year,
                                                        FinancialYearID = financialYear.ID,
                                                        Exclude = month.Exclude,
                                                        Days = (from day in month.FinancialYearMonthDays
                                                                select new FinancialYearMonthDayDto
                                                                {
                                                                    ID = day.ID,
                                                                    Available = day.Available,
                                                                    BankHoliday = day.BankHoliday,
                                                                    Day = day.Day,
                                                                    Name = day.Name,
                                                                    NumberOfStaff = day.NumberOfStaff,
                                                                    Month = month.Month,
                                                                    Year = month.Year.ToString(),
                                                                    Staff = day.Staff != "null" ? JsonConvert.DeserializeObject<List<StaffLeaveDto>>(day.Staff) : new List<StaffLeaveDto>()
                                                                }).ToList()
                                                    }).ToList(),
                                          LeaveTypes = Utility.Util.GetLeaveTypes()
                                      }).ToList();

                return financialYears;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FinancialYearDto> RetrieveOpenedClosedFinancialYears()
        {
            try
            {
                var locations = LocationDL.RetrieveLocations();

                var fy = FinancialYearDL.RetrieveOpenedClosedFinancialYears();

                var financialYears = (from financialYear in fy
                                      select new FinancialYearDto
                                      {
                                          ID = financialYear.ID,
                                          Location = new LocationDto
                                          {
                                              ID = locations.Where(l => l.ID == financialYear.LocationID).FirstOrDefault().ID,
                                              Name = locations.Where(l => l.ID == financialYear.LocationID).FirstOrDefault().Name,
                                          },
                                          CreatedOn = string.Format("{0:g}", financialYear.CreatedOn),
                                          EndDate = financialYear.EndDate.FormatDate(),
                                          ExcludeMonths = financialYear.ExcludeMonths != null ? JsonConvert.DeserializeObject<List<ExcludeMonthDto>>(financialYear.ExcludeMonths) : new List<ExcludeMonthDto>(),
                                          ExcludeMonthsDetails = GetExcludeMonthDetails(financialYear.ExcludeMonths),
                                          LeavePriorNotice = financialYear.LeavePriorNotice,
                                          StartDate = financialYear.StartDate.FormatDate(),
                                          Status = financialYear.Status,
                                      }).ToList();

                return financialYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<LeaveApprovalDto> RetrieveLeaveRequests(SearchFilter filter)
        {
            try
            {
                filter.Type = Enums.ApprovalType.ApproveLeave.ToString();

                var pendingApprovals = ApprovalDL.RetrieveFilteredApprovals(filter);

                var leaveApprovals = new List<LeaveApprovalDto>();

                if (pendingApprovals.Any())
                {
                    var staffs = StaffDL.RetrieveStaffRoleTypes();

                    var approvingStaffRoleType = staffs.FirstOrDefault(s => s.ID == filter.StaffID).Role.Type;
                    if (approvingStaffRoleType == Enums.RoleType.Operation.ToString())
                    {
                        pendingApprovals.ForEach(approval =>
                        {
                            var leaveRequest = JsonConvert.DeserializeObject<LeaveRequestDto>(approval.Obj);

                            if (string.IsNullOrEmpty(leaveRequest.LeaveType))
                            {
                                leaveRequest.LeaveType = "Annual";
                            }

                            leaveRequest.LeaveIsDeductible = Util.IsLeaveTypeDeductible(leaveRequest.LeaveType);

                            leaveRequest.LeaveTypeDescription = Util.GetLeaveTypeDescription(leaveRequest.LeaveType);

                            leaveRequest.DeclineReason = approval.DeclineReason;

                            var leaveApproval = new LeaveApprovalDto
                            {
                                LeaveRequest = leaveRequest,
                                Approval = new ApprovalDto
                                {
                                    ApprovalID = approval.ID,
                                    ApprovalStatus = approval.Status,
                                    RequestedOn = string.Format("{0:g}", approval.RequestedOn),
                                    ApprovedOn = approval.ApprovedOn != null ? string.Format("{0:g}", approval.ApprovedOn) : string.Empty
                                }
                            };

                            leaveApprovals.Add(leaveApproval);
                        });
                    }
                    else if (approvingStaffRoleType == Enums.RoleType.Coach.ToString())
                    {
                        pendingApprovals.ForEach(approval =>
                        {
                            var leaveRequest = JsonConvert.DeserializeObject<LeaveRequestDto>(approval.Obj);

                            var requestingStaffRoleType = staffs.FirstOrDefault(s => s.ID == leaveRequest.StaffID).Role.Type;

                            if (requestingStaffRoleType == Enums.RoleType.Others.ToString() && filter.LocationID == leaveRequest.StaffLocation.ID)
                            {
                                if (string.IsNullOrEmpty(leaveRequest.LeaveType))
                                {
                                    leaveRequest.LeaveType = "Annual";
                                }

                                leaveRequest.LeaveIsDeductible = Util.IsLeaveTypeDeductible(leaveRequest.LeaveType);

                                leaveRequest.LeaveTypeDescription = Util.GetLeaveTypeDescription(leaveRequest.LeaveType);

                                leaveRequest.DeclineReason = approval.DeclineReason;

                                var leaveApproval = new LeaveApprovalDto
                                {
                                    LeaveRequest = leaveRequest,
                                    Approval = new ApprovalDto
                                    {
                                        ApprovalID = approval.ID,
                                        ApprovalStatus = approval.Status,
                                        RequestedOn = string.Format("{0:g}", approval.RequestedOn),
                                        ApprovedOn = approval.ApprovedOn != null ? string.Format("{0:g}", approval.ApprovedOn) : string.Empty
                                    }
                                };

                                leaveApprovals.Add(leaveApproval);
                            }

                        });
                    }

                }

                return leaveApprovals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffLeaveDto> RetrieveCancelLeaveRequests(SearchFilter filter)
        {
            try
            {
                filter.Type = Enums.ApprovalType.CancelLeave.ToString();

                var pendingApprovals = ApprovalDL.RetrieveFilteredApprovals(filter);

                var leaveRequests = new List<StaffLeaveDto>();

                if (pendingApprovals.Any())
                {
                    var staffs = StaffDL.RetrieveStaffRoleTypes();

                    var approvingStaffRoleType = staffs.FirstOrDefault(s => s.ID == filter.StaffID).Role.Type;

                    if (approvingStaffRoleType == Enums.RoleType.Operation.ToString())
                    {
                        pendingApprovals.ForEach(approval =>
                        {
                            var leaveRequest = JsonConvert.DeserializeObject<StaffLeaveDto>(approval.Obj);
                            leaveRequest.Approval = new ApprovalDto
                            {
                                ApprovalID = approval.ID,
                                ApprovalStatus = approval.Status,
                                RequestedOn = string.Format("{0:g}", approval.RequestedOn),
                                ApprovedOn = approval.ApprovedOn != null ? string.Format("{0:g}", approval.ApprovedOn) : string.Empty
                            };

                            leaveRequests.Add(leaveRequest);
                        });
                    }
                    else if (approvingStaffRoleType == Enums.RoleType.Coach.ToString())
                    {
                        pendingApprovals.ForEach(approval =>
                        {
                            var leaveRequest = JsonConvert.DeserializeObject<StaffLeaveDto>(approval.Obj);

                            var requestingStaffRoleType = staffs.FirstOrDefault(s => s.ID == leaveRequest.StaffID).Role.Type;

                            if (requestingStaffRoleType == Enums.RoleType.Others.ToString() && filter.LocationID == leaveRequest.LocationID)
                            {
                                leaveRequest.Approval = new ApprovalDto
                                {
                                    ApprovalID = approval.ID,
                                    ApprovalStatus = approval.Status,
                                    RequestedOn = string.Format("{0:g}", approval.RequestedOn),
                                    ApprovedOn = approval.ApprovedOn != null ? string.Format("{0:g}", approval.ApprovedOn) : string.Empty
                                };

                                leaveRequests.Add(leaveRequest);
                            }
                        });
                    }


                }

                return leaveRequests;
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
                return FinancialYearDL.RetrieveStaffLeaves(staffID, locationID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffLeaveDetailsDto RetrieveStaffLeavesByFinancialYear(long staffID, long locationID, long financialYearId)
        {
            try
            {
                return FinancialYearDL.RetrieveStaffLeavesByFinancialYear(staffID, locationID, financialYearId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetExcludeMonthDetails(string months)
        {
            var em = string.Empty;

            var excludeMonths = months != null ? JsonConvert.DeserializeObject<List<ExcludeMonthDto>>(months) : new List<ExcludeMonthDto>();

            if (excludeMonths.Any())
            {
                excludeMonths.ForEach(m =>
                {
                    em += m.text + ",";
                });
            }

            return !string.IsNullOrEmpty(em) ? em.TrimEnd(',') : em;
        }

        public static void AutoLeave(Enums.AutoLeaveType leaveType = Enums.AutoLeaveType.Start)
        {
            try
            {
                if (leaveType == Enums.AutoLeaveType.Start)
                {
                    FinancialYearDL.AutoStartLeave();
                }
                else
                {
                    FinancialYearDL.AutoEndLeave();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
