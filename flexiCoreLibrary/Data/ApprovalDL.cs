using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class ApprovalDL
    {
        public static bool Save(Approval approval)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.Approvals.Add(approval);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(Approval approval)
        {
            try
            {
                using (var context = new DataContext())
                {

                    var existingApproval = context.Approvals
                                    .Where(t => t.ID == approval.ID)
                                    .FirstOrDefault();

                    if (existingApproval != null)
                    {
                        existingApproval.ApprovedOn = DateUtil.Now();
                        existingApproval.Status = approval.Status;
                        existingApproval.DeclineReason = approval.DeclineReason;

                        context.Entry(existingApproval).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Approval> RetrieveFilteredApprovals(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);

                    var approvals = context.Approvals
                                            .Where(a => a.RequestedOn >= fromDate &&
                                                        a.RequestedOn <= toDate).ToList();

                    if (approvals.Any())
                    {
                        if (!string.IsNullOrEmpty(filter.Type))
                        {
                            approvals = approvals.Where(c => c.Type == filter.Type).ToList();
                        }

                        if (!string.IsNullOrEmpty(filter.Status))
                        {
                            approvals = approvals.Where(c => c.Status == filter.Status).ToList();
                        }
                    }

                    return approvals;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal RetrievePendingLeaveDaysTaken(long staffID, long locationID, long financialYearID)
        {
            try
            {
                var pendingLeaveDays = 0.0m;

                var useSecondLevelApproval = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("UseSecondLevelApproval"));

                using (var context = new DataContext())
                {
                    var type = Enums.ApprovalType.ApproveLeave.ToString();

                    var statuses = new List<string>();
                    statuses.Add(Enums.ApprovalStatus.Pending.ToString());

                    if (useSecondLevelApproval)
                    {
                        statuses.Add(Enums.ApprovalStatus.Acknowledged.ToString());
                    }

                    var approvals = context.Approvals
                                            .Where(a => a.Type == type &&
                                                       statuses.Contains(a.Status)).ToList();

                    if (approvals.Any())
                    {
                        approvals.ForEach((approval) =>
                        {
                            var leaveRequest = JsonConvert.DeserializeObject<LeaveRequestDto>(approval.Obj);
                            if (leaveRequest.StaffID == staffID)
                            {
                                var leaveIsDeductible = Utility.Util.IsLeaveTypeDeductible(leaveRequest.LeaveType);
                                if (leaveIsDeductible)
                                {
                                    if (leaveRequest.FinancialYear != null)
                                    {
                                        if (leaveRequest.FinancialYear.ID == financialYearID)
                                        {
                                            pendingLeaveDays += leaveRequest.LeaveDaysTaken;
                                        }
                                    }
                                    else
                                    {
                                        var firstRequest = leaveRequest.RequestedDays.FirstOrDefault();
                                        var firstRequestYear = Convert.ToInt64(firstRequest.Year);
                                        var firstRequestMonth = firstRequest.Month;

                                        var leaveFinancialYearID = context.FinancialYearMonths.Where(m => m.Year == firstRequestYear && m.Month == firstRequestMonth).FirstOrDefault()?.FinancialYearID;

                                        if (leaveFinancialYearID == financialYearID)
                                        {
                                            pendingLeaveDays += leaveRequest.LeaveDaysTaken;
                                        }
                                    }
                                }
                            }

                        });
                    }

                    return pendingLeaveDays;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Approval RetrieveById(long id)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var approvals = context.Approvals
                                            .Where(f => f.ID == id);

                    return approvals.Any() ? approvals.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
