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
    public class RoleAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveRole([FromBody]RoleDto role)
        {
            try
            {
                if (!role.Functions.Any())
                {
                    throw new Exception("The Functions field is required.");
                }

                bool result = RolePL.Save(role);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Role added successfully.");
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
        public HttpResponseMessage UpdateRole([FromBody]RoleDto role)
        {
            try
            {
                if (!role.Functions.Any())
                {
                    throw new Exception("The Functions field is required.");
                }

                bool result = RolePL.Update(role);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Role updated successfully.");
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
        public HttpResponseMessage EnableOrDisableRole([FromBody]RoleDto role)
        {
            try
            {
                bool result = RolePL.EnableOrDisable(role);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Role updated successfully.");
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
        public HttpResponseMessage RetrieveRoles()
        {
            try
            {
                var roles = RolePL.RetrieveRoles();
                var returnedRoles = new { data = roles };
                return Request.CreateResponse(HttpStatusCode.OK, returnedRoles);
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
        public HttpResponseMessage RetrieveActiveRoles()
        {
            try
            {
                var roles = RolePL.RetrieveActiveRoles();
                var returnedRoles = new { data = roles };
                return Request.CreateResponse(HttpStatusCode.OK, returnedRoles);
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
