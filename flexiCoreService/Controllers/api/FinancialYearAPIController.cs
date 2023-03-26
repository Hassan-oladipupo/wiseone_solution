using flexiCoreLibrary;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace flexiCoreService.Controllers.api
{
    public class FinancialYearAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveFinancialYear([FromBody]FinancialYearDto financialYearDto)
        {
            try
            {
                var result = FinancialYearPL.Save(financialYearDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Financial year saved successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateFinancialYear([FromBody]FinancialYearDto financialYearDto)
        {
            try
            {
                var result = FinancialYearPL.Update(financialYearDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Financial year updated successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public HttpResponseMessage DeleteFinancialYear([FromBody]FinancialYearDto financialYearDto)
        {
            try
            {
                var result = FinancialYearPL.Delete(financialYearDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Financial year deleted successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public HttpResponseMessage ToggleFinancialYear([FromBody]FinancialYearDto financialYearDto)
        {
            try
            {
                var result = FinancialYearPL.ToggleStatus(financialYearDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Financial year updated successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> LogLeaveRequestForApproval([FromBody]LeaveRequestDto leaveRequestDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(leaveRequestDto.LeaveType))
                    {
                        leaveRequestDto.RequestedDays.Sort((a, b) => a.ID.CompareTo(b.ID));
                        var result = FinancialYearPL.LogLeaveRequestForApproval(leaveRequestDto);

                        if (result)
                        {
                            HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendLeaveRequestMail(leaveRequestDto));

                            return Request.CreateResponse(HttpStatusCode.OK, "Your leave request has been logged for approval. You will be notified on the approval status.");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, $"Hi {leaveRequestDto.StaffName}, your leave request did not go through because you are using an outdated version of Wise1ne. Please download the latest version of Wise1ne and re-submit your leave request.");
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> LogCancelLeaveForApproval([FromBody]StaffLeaveDto staffLeaveDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.LogCancelLeaveForApproval(staffLeaveDto);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendCancelLeaveRequestMail(staffLeaveDto));

                        return Request.CreateResponse(HttpStatusCode.OK, "Your leave cancellation request has been logged for approval. You will be notified on the approval status.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> DeclineLeaveRequest([FromBody]LeaveApprovalDto leaveApprovalDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.DeclineLeaveRequest(leaveApprovalDto);

                    if (result)
                    {
                        leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Declined.ToString();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendLeaveApprovalMail(leaveApprovalDto));

                        return Request.CreateResponse(HttpStatusCode.OK, "Leave request was successfully declined and the reason for declining the request has been sent to the staff..");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> ApproveLeaveRequest([FromBody]LeaveApprovalDto leaveApprovalDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var useSecondLevelApproval = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("UseSecondLevelApproval"));

                    if(useSecondLevelApproval)
                    {
                        var result = FinancialYearPL.LogLeaveRequestForSecondLevelApproval(leaveApprovalDto);

                        if (result)
                        {
                            leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Acknowledged.ToString();
                            HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendSecondLeaveApprovalMail(leaveApprovalDto));

                            return Request.CreateResponse(HttpStatusCode.OK, "Leave request successfully logged for second level approval.");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                        }
                    }
                    else
                    {
                        var result = FinancialYearPL.ApproveLeaveRequest(leaveApprovalDto);

                        if (result)
                        {
                            leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Approved.ToString();
                            HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendLeaveApprovalMail(leaveApprovalDto));

                            return Request.CreateResponse(HttpStatusCode.OK, "Leave request was successfully approved.");
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                        }
                    }
                });                

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> SecondLevelApproveLeaveRequest([FromBody]LeaveApprovalDto leaveApprovalDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.ApproveLeaveRequest(leaveApprovalDto);

                    if (result)
                    {
                        leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Approved.ToString();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendLeaveApprovalMail(leaveApprovalDto));

                        return Request.CreateResponse(HttpStatusCode.OK, "Leave request was successfully approved.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> SaveLeaveRequest([FromBody]LeaveRequestDto leaveRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    leaveRequest.RequestedDays.Sort((a, b) => a.ID.CompareTo(b.ID));
                    var result = FinancialYearPL.SaveLeaveRequest(leaveRequest);

                    if (result)
                    {
                        //HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendLeaveCreationMail(leaveRequest));

                        return Request.CreateResponse(HttpStatusCode.OK, "Staff Leave was successfully created.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> DeclineCancelLeaveRequest([FromBody]StaffLeaveDto leaveApprovalDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.DeclineCancelLeaveRequest(leaveApprovalDto);

                    if (result)
                    {
                        leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Declined.ToString();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendCancelLeaveApprovalMail(leaveApprovalDto));

                        return Request.CreateResponse(HttpStatusCode.OK, "Leave cancellation request was successfully declined.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> ApproveCancelLeaveRequest([FromBody]StaffLeaveDto leaveApprovalDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.ApproveCancelLeaveRequest(leaveApprovalDto);

                    if (result)
                    {
                        leaveApprovalDto.Approval.ApprovalStatus = Enums.ApprovalStatus.Approved.ToString();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendCancelLeaveApprovalMail(leaveApprovalDto));

                        return Request.CreateResponse(HttpStatusCode.OK, "Leave cancellation request was successfully approved.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> DeleteLeaveRequest([FromBody]StaffLeaveDto staffLeave)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = FinancialYearPL.DeleteLeaveRequest(staffLeave);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendDeleteLeaveMail(staffLeave));

                        return Request.CreateResponse(HttpStatusCode.OK, "Staff leave was successfully cancelled.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                    }
                });

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveLocationFinancialYears(long locationId)
        {
            try
            {
                var financialYears = FinancialYearPL.RetrieveLocationFinancialYears(locationId);
                var returnedFinancialYears = new { data = financialYears };
                return Request.CreateResponse(HttpStatusCode.OK, returnedFinancialYears);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveFinancialYears(string status)
        {
            try
            {
                var financialYears = FinancialYearPL.RetrieveFinancialYears(status);
                var returnedFinancialYears = new { data = financialYears };
                return Request.CreateResponse(HttpStatusCode.OK, returnedFinancialYears);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveStaffLeaves(long staffID, long locationID)
        {
            try
            {
                var staffLeave = FinancialYearPL.RetrieveStaffLeaves(staffID, locationID);
                return Request.CreateResponse(HttpStatusCode.OK, staffLeave);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveStaffLeavesByFinancialYear(long staffID, long locationID, long financialYearID)
        {
            try
            {
                var staffLeave = FinancialYearPL.RetrieveStaffLeavesByFinancialYear(staffID, locationID, financialYearID);
                return Request.CreateResponse(HttpStatusCode.OK, staffLeave);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveOpenedClosedFinancialYears()
        {
            try
            {
                var financialYears = FinancialYearPL.RetrieveOpenedClosedFinancialYears();
                var returnedFinancialYears = new { data = financialYears };
                return Request.CreateResponse(HttpStatusCode.OK, returnedFinancialYears);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage CurrentFinancialYear(long locationID)
        {
            try
            {
                var financialYear = FinancialYearPL.CurrentFinancialYear(locationID);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = financialYear });
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> RetrieveLeaveRequests([FromBody]SearchFilter filter)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var leaveRequests = FinancialYearPL.RetrieveLeaveRequests(filter);
                    var result = new { data = leaveRequests };
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                catch (Exception ex)
                {
                    ErrorHandler.WriteError(ex);
                    var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                    response.ReasonPhrase = ex.Message;
                    return response;
                }
            });
        }

        [HttpPost]
        public HttpResponseMessage RetrieveCancelLeaveRequests([FromBody]SearchFilter filter)
        {
            try
            {
                var leaveRequests = FinancialYearPL.RetrieveCancelLeaveRequests(filter);
                var result = new { data = leaveRequests };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage RetrieveStaffLeaveStatus(long staffID)
        {
            try
            {
                var staff = StaffPL.RetrieveStaffByID(staffID);
                return Request.CreateResponse(HttpStatusCode.OK, staff);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateLeaveStatus([FromBody]StaffDto staff)
        {
            try
            {
                var result = StaffPL.UpdateLeaveStatus(staff);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Leave status updated successfully.");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed");
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }
    }
}
