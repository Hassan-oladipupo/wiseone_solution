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
    public class MessageAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveMessage([FromBody]MessageDto messageDto)
        {
            try
            {
                var result = MessagePL.SaveMessage(messageDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Message sent successfully.");
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
        public HttpResponseMessage RetrieveStaffConversations(long staffID)
        {
            try
            {
                var conversations = MessagePL.RetrieveStaffConversations(staffID);
                return Request.CreateResponse(HttpStatusCode.OK, conversations);
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
