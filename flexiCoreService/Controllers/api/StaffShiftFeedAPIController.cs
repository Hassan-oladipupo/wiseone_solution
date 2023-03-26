using flexiCoreLibrary;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace flexiCoreService.Controllers.api
{
    public class StaffShiftFeedAPIController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage RetrieveShiftLatestFeeds(long locationID)
        {
            try
            {
                var latestFeeds = StaffShiftFeedPL.RetrieveShiftLatestFeeds(locationID);
                return Request.CreateResponse(HttpStatusCode.OK, new { data = latestFeeds });
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
        public HttpResponseMessage RetrieveServices()
        {
            try
            {
                var latestFeeds = FeedSettingPL.RetrieveServices();
                return Request.CreateResponse(HttpStatusCode.OK, new { data = latestFeeds });
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
