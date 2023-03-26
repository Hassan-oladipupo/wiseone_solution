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
    public class ClassRoomAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveClassRoom([FromBody]ClassRoomDto classroomDto)
        {
            try
            {                
                var result = ClassRoomPL.Save(classroomDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Room added successfully.");
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
        public HttpResponseMessage UpdateClassRoom([FromBody]ClassRoomDto classroomDto)
        {
            try
            {
                var result = ClassRoomPL.Update(classroomDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Room updated successfully.");
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
        public HttpResponseMessage EnableOrDisableClassRoom([FromBody]ClassRoomDto classroomDto)
        {
            try
            {
                var result = ClassRoomPL.EnableOrDisable(classroomDto);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Room updated successfully.");
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
        public HttpResponseMessage RetrieveClassRooms()
        {
            try
            {
                var classrooms = ClassRoomPL.RetrieveClassRooms();
                var returnedClassrooms = new { data = classrooms };
                return Request.CreateResponse(HttpStatusCode.OK, returnedClassrooms);
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
        public HttpResponseMessage RetrieveActiveClassRooms()
        {
            try
            {
                var classrooms = ClassRoomPL.RetrieveActiveClassRooms();
                var returnedClassrooms = new { data = classrooms };
                return Request.CreateResponse(HttpStatusCode.OK, returnedClassrooms);
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
        public HttpResponseMessage RetrieveActiveClassRoomsInLocation(long locationId)
        {
            try
            {
                var classrooms = ClassRoomPL.RetrieveActiveClassRoomsInLocation(locationId);
                var returnedClassrooms = new { data = classrooms };
                return Request.CreateResponse(HttpStatusCode.OK, returnedClassrooms);
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
