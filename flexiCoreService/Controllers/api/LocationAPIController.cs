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
    public class LocationAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveLocation([FromBody]LocationDto locationDto)
        {
            try
            {                
                var result = LocationPL.Save(locationDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Location added successfully.");
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
        public HttpResponseMessage UpdateLocation([FromBody]LocationDto locationDto)
        {
            try
            {
                var result = LocationPL.Update(locationDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Location updated successfully.");
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
        public HttpResponseMessage EnableOrDisableLocation([FromBody]LocationDto locationDto)
        {
            try
            {
                var result = LocationPL.EnableOrDisable(locationDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Location updated successfully.");
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
        public HttpResponseMessage RetrieveLocations()
        {
            try
            {
                var locations = LocationPL.RetrieveLocations();
                var returnedLocations = new { data = locations };
                return Request.CreateResponse(HttpStatusCode.OK, returnedLocations);
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
        public HttpResponseMessage RetrieveActiveLocations()
        {
            try
            {
                var locations = LocationPL.RetrieveActiveLocations();
                var returnedLocations = new { data = locations };
                return Request.CreateResponse(HttpStatusCode.OK, returnedLocations);
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
