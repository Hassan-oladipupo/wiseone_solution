using flexiCoreLibrary;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace flexiCoreService.Controllers.api
{
    public class ApproverEmailAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveConfiguration([FromBody]ApproverEmailDto email)
        {
            try
            {
                var result = ApproverEmailPL.Save(email);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Notification rule saved successfully.");
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
        public HttpResponseMessage SaveSecondLevelConfiguration([FromBody]SecondLevelApproverEmailDto email)
        {
            try
            {
                var result = ApproverEmailPL.SaveSecondLevelEmail(email);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Approver saved successfully.");
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
        public HttpResponseMessage UpdateConfiguration([FromBody]ApproverEmailDto email)
        {
            try
            {
                var result = ApproverEmailPL.Update(email);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Notification rule updated successfully.");
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
        public HttpResponseMessage DeleteConfiguration([FromBody]ApproverEmailDto email)
        {
            try
            {
                var result = ApproverEmailPL.Delete(email);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Notification rule deleted successfully.");
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
        public HttpResponseMessage DeleteSecondLevelConfiguration([FromBody]SecondLevelApproverEmailDto email)
        {
            try
            {
                var result = ApproverEmailPL.DeleteSecondLevelEmail(email);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Approver deleted successfully.");
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

        [HttpGet]
        public HttpResponseMessage RetrieveConfigurations()
        {
            try
            {
                var email = ApproverEmailPL.RetrieveEmails();
                var returnedEmails = new { data = email };
                return Request.CreateResponse(HttpStatusCode.OK, returnedEmails);
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
        public HttpResponseMessage RetrieveSecondLevelConfigurations()
        {
            try
            {
                var email = ApproverEmailPL.RetrieveSecondLevelEmails();
                var returnedEmails = new { data = email };
                return Request.CreateResponse(HttpStatusCode.OK, returnedEmails);
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
