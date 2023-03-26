using flexiCoreLibrary;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Process;
using flexiCoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace flexiCoreService.Controllers.api
{
    public class StaffAPIController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> LogStaffSignUp([FromBody]StaffDto staff)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = StaffPL.LogStaffSignUp(staff);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendStaffSignUpRequestMail(staff));
                        return Request.CreateResponse(HttpStatusCode.OK, "Your sign up request has been logged successfully for approval. An email will be sent to you once your request has been approved.");
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
        public async Task<HttpResponseMessage> ApproveStaffSignUp([FromBody]StaffDto staff)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = StaffPL.ApproveStaffSignUp(staff);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendNewUserMail(staff));
                        return Request.CreateResponse(HttpStatusCode.OK, "Sign up request approved successfully.");
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
        public HttpResponseMessage UpdateToken([FromBody]StaffDto staff)
        {
            try
            {
                StaffPL.UpdateToken(staff);                
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateStaff([FromBody]StaffDto staff)
        {
            try
            {
                var result = StaffPL.UpdateStaff(staff);

                if (result)
                {                    
                    return Request.CreateResponse(HttpStatusCode.OK, "Staff updated successfully.");
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
        public HttpResponseMessage EnableOrDisableStaff([FromBody]StaffDto staff)
        {
            try
            {
                var result = StaffPL.EnableOrDisable(staff);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Staff updated successfully.");
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
        public HttpResponseMessage CancelLeave([FromBody]StaffDto staff)
        {
            try
            {
                var result = StaffPL.CancelLeave(staff);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Staff leave cancelled successfully.");
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
        public HttpResponseMessage DeclineStaffSignUp([FromBody]StaffDto staff)
        {
            try
            {
                var result = StaffPL.DeclineStaffSignUp(staff);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Staff sign up declined successfully.");
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
        public HttpResponseMessage RetrieveSignUpRequests([FromBody]SearchFilter filter)
        {
            try
            {
                var signUpRequests = StaffPL.RetrieveSignUpRequests(filter);
                var result = new { data = signUpRequests };
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
        public HttpResponseMessage RetrieveStaffs()
        {
            try
            {
                var staffs = StaffPL.RetrieveStaffs();
                var result = new { data = staffs };
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
        public HttpResponseMessage RetrieveActiveStaffs()
        {
            try
            {
                var staffs = StaffPL.RetrieveActiveStaffs();
                var result = new { data = staffs };
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
        public async Task<HttpResponseMessage> RetrieveActiveStaffOnLeave()
        {
            try
            {
                var staffs = await new StaffPL().RetrieveActiveStaffOnLeave();
                var result = new { data = staffs };
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
        public HttpResponseMessage RetrieveActiveStaffsInLocation(long locationId)
        {
            try
            {
                var staffs = StaffPL.RetrieveActiveStaffsInLocation(locationId);
                var result = new { data = staffs };
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
        public HttpResponseMessage ChangePassword([FromBody]PasswordModel changePassword)
        {
            try
            {
                string password = PasswordHash.MD5Hash(changePassword.Password);
                string username = changePassword.Username;
                bool result = StaffPL.ChangePassword(username, password);
                return result.Equals(true) ? Request.CreateResponse(HttpStatusCode.OK, "Successful") : Request.CreateResponse(HttpStatusCode.BadRequest, "Failed");
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody]PasswordModel changePassword)
        {
            try
            {
                return await Task.Run(() =>
                {
                    string username = changePassword.Username;
                    var staff = StaffPL.RetrieveStaffByUsername(username);
                    if (staff != null)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendForgotPasswordMail(staff));
                        return Request.CreateResponse(HttpStatusCode.OK, staff.Email);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid username");
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
        public HttpResponseMessage ConfirmUsername([FromBody]PasswordModel changePassword)
        {
            try
            {
                var user = StaffPL.RetrieveStaffByID(changePassword.StaffId);
                if (user != null)
                {
                    //Add a mail for password reset
                    object response = new
                    {
                        Status = "Successful",
                        Username = user.Username,
                        Role = user.Role
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid username");
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public HttpResponseMessage AuthenticateStaff([FromBody]PasswordModel passwordModel)
        {
            try
            {
                string password = PasswordHash.MD5Hash(passwordModel.Password);

                var staffObj = StaffPL.AuthenticateStaff(passwordModel.Username, password, passwordModel.Token);

                if (staffObj != null && staffObj.ID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, staffObj);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Authentication failed! Invalid Username/Password");
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
