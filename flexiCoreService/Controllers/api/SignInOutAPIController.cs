using flexiCoreLibrary;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace flexiCoreService.Controllers.api
{
    public class SignInOutAPIController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage RetrieveShiftForTheDay(long staffID)
        {
            try
            {
                var shift = SignInOutPL.RetrieveShiftForTheDay(staffID);
                return Request.CreateResponse(HttpStatusCode.OK, shift);
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
        public HttpResponseMessage RetrieveOverTimeForApproval([FromBody]SearchFilter filter)
        {
            try
            {
                var overTimes = SignInOutPL.RetrieveOverTimeForApproval(filter);
                var result = new { data = overTimes };
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

        [HttpPut]
        public HttpResponseMessage ApproveOrDeclineOverTime([FromBody]ShiftSwapDto shiftOverTime)
        {
            try
            {
                ShiftConfigPL.ApproveShiftOverTime(shiftOverTime);
                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Request was {0} successfully.", shiftOverTime.ApprovalStatus.ToLower()));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> LogCancelShiftForApproval([FromBody]SignInOutDto clockIn)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = SignInOutPL.LogCancelShiftForApproval(clockIn);
                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendCancelShiftRequestMail(clockIn));
                        return Request.CreateResponse(HttpStatusCode.OK, "Your shift cancellation request has been logged for approval. You will be notified on the approval status.");
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

        [HttpPost]
        public HttpResponseMessage RetrieveShiftCancelRequests([FromBody]SearchFilter filter)
        {
            try
            {
                var leaveRequests = SignInOutPL.RetrieveShiftCancelRequests(filter);
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

        [HttpPut]
        public async Task<HttpResponseMessage> ApproveShiftCancellation([FromBody]SignInOutDto shiftCancelRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = SignInOutPL.ApproveShiftCancellation(shiftCancelRequest);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendCancelShiftApprovalMail(shiftCancelRequest));
                        return Request.CreateResponse(HttpStatusCode.OK, $"Shift cancellation request was successfully {shiftCancelRequest.ApprovalStatus.ToLower()}.");
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

        [HttpPost]
        public HttpResponseMessage ClockIn([FromBody]SignInOutDto clockIn)
        {
            try
            {
                var signIn = SignInOutPL.ClockIn(clockIn);
                return Request.CreateResponse(HttpStatusCode.OK, signIn);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public HttpResponseMessage ClockOut([FromBody]SignInOutDto clockIn)
        {
            try
            {
                var signIn = SignInOutPL.ClockOut(clockIn);
                return Request.CreateResponse(HttpStatusCode.OK, signIn);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public HttpResponseMessage RetrieveSignInOuts([FromBody]SearchFilter filter)
        {
            try
            {
                var signInOuts = SignInOutPL.RetrieveSignInOuts(filter);
                var result = new { data = signInOuts };
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

        [HttpPost]
        public HttpResponseMessage RetrieveStaffSignInOuts([FromBody]SearchFilter filter)
        {
            try
            {
                var signInOuts = SignInOutPL.RetrieveStaffSignInOuts(filter);
                var result = new { data = signInOuts };
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

        [HttpPost]
        public HttpResponseMessage RetrieveReportSummary([FromBody]SearchFilter filter)
        {
            try
            {
                var reports = SignInOutPL.RetrieveReportSummary(filter);
                var result = new { data = reports };
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

        [HttpPost]
        public HttpResponseMessage RetrieveReportDetail([FromBody]ReportSummaryDto summary)
        {
            try
            {
                var reports = SignInOutPL.RetrieveReportDetail(summary);
                var result = new { data = reports };
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

        [HttpPost]
        public HttpResponseMessage RetrieveStaffReportSummary([FromBody]SearchFilter filter)
        {
            try
            {
                var reports = SignInOutPL.RetrieveStaffReportSummary(filter);
                var result = new { data = reports };
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

        [HttpPost]
        public HttpResponseMessage RetrieveStaffReportDetail([FromBody]StaffReportSummaryDto summary)
        {
            try
            {
                var reports = SignInOutPL.RetrieveStaffReportDetail(summary);
                var result = new { data = reports };
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

    }
}
