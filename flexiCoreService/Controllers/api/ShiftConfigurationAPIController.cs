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
    public class ShiftConfigurationAPIController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> SaveShiftConfiguration([FromBody]ShiftConfigDto configDto)
        {
            try
            {
                var result = await new ShiftConfigPL().Save(configDto);

                if (result)
                {
                    HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendNewShiftMail(configDto));                   
                    return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift for week: {0} created successfully.", configDto.WeekName));
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
        public async Task<HttpResponseMessage> UpdateShiftConfiguration([FromBody]ShiftConfigDto configDto)
        {
            try
            {
                var result = await new ShiftConfigPL().Update(configDto);

                if (result)
                {
                    HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendNewShiftMail(configDto));
                    return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift for week: {0} updated successfully.", configDto.WeekName));
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
        public HttpResponseMessage DeleteStaffShiftConfiguration([FromBody]StaffShiftDto staffShiftConfiguration)
        {
            try
            {
                var result = ShiftConfigPL.DeleteStaffShiftConfiguration(staffShiftConfiguration);

                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, $"Shift deleted successfully for {staffShiftConfiguration.StaffName}.");
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
        public HttpResponseMessage RetrieveShiftConfigurations(SearchFilter filter)
        {
            try
            {
                var configs = ShiftConfigPL.RetrieveShiftConfigurations(filter);
                var returnedConfigs = new { data = configs };
                return Request.CreateResponse(HttpStatusCode.OK, returnedConfigs);
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
        public HttpResponseMessage RetrieveStaffShiftSettings(long staffID)
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveStaffShiftSettings(staffID);
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

        [HttpGet]
        public HttpResponseMessage RetrieveWeeklyShift(long staffID, long configID)
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveWeeklyShift(staffID, configID);
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

        [HttpGet]
        public HttpResponseMessage RetrieveRoomStaffLocation()
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveRoomStaffLocation();
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

        [HttpGet]
        public HttpResponseMessage RetrieveLocationRoomStaffLocation(long locationId)
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveLocationRoomStaffLocation(locationId);
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


        #region---------- SHIFT SWAP -----------
        [HttpPost]
        public async Task<HttpResponseMessage> SaveShiftSwapRequest([FromBody]ShiftSwapDto shiftSwapDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.SaveShiftSwapRequest(shiftSwapDto);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Your shift for: {0} is now available for swapping by any interested co-worker.", shiftSwapDto.Date));
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
        public async Task<HttpResponseMessage> SaveShiftSwapOptions([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.SaveShiftSwapOptions(shiftSwapRequest);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request was successful. Request is now awaiting acceptance from " + shiftSwapRequest.ToShift.StaffKnownAs));
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
        public async Task<HttpResponseMessage> DeclineShiftSwapApproval([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.DeclineShiftSwapApproval(shiftSwapRequest);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request declined successfully."));
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
        public async Task<HttpResponseMessage> ApproveShiftSwap([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.ApproveShiftSwap(shiftSwapRequest);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request approved successfully."));
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
        public async Task<HttpResponseMessage> AcceptShiftSwap([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.AcceptShiftSwap(shiftSwapRequest);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendShiftSwapRequestMail(shiftSwapRequest));
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request successfully saved and logged for approval."));
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
        public async Task<HttpResponseMessage> CancelShiftSwap([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.CancelShiftSwap(shiftSwapRequest);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request was cancelled successfully"));
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
        public async Task<HttpResponseMessage> DeclineShiftSwap([FromBody]ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.DeclineShiftSwap(shiftSwapRequest);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Shift swap request was declined successfully"));
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
        public async Task<HttpResponseMessage> DeleteShiftSwap([FromBody]ShiftSwapDto shiftSwapDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = ShiftConfigPL.DeleteShiftSwap(shiftSwapDto);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Your shift for: {0} is no longer available for shift swap.", shiftSwapDto.Date));
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
        public HttpResponseMessage RetrieveAvailableShiftSwaps([FromBody] ShiftSwapDto shiftDto)
        {
            try
            {
                var shifts = ShiftConfigPL.RetrieveAvailableShiftSwaps(shiftDto);
                return Request.CreateResponse(HttpStatusCode.OK, shifts);
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
        public HttpResponseMessage RetrieveStaffShiftSwaps(string username)
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveStaffShiftSwaps(username);
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

        [HttpGet]
        public HttpResponseMessage RetrieveShiftSwapRequests(long staffID)
        {
            try
            {
                var shift = ShiftConfigPL.RetrieveShiftSwapRequests(staffID);
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
        public HttpResponseMessage RetrieveShiftSwapForApproval([FromBody]SearchFilter filter)
        {
            try
            {
                var swapRequests = ShiftConfigPL.RetrieveShiftSwapForApproval(filter);
                var result = new { data = swapRequests };
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
        #endregion

        #region Shift Overtime
        [HttpPost]
        public async Task<HttpResponseMessage> SaveShiftOverTimeRequest([FromBody]ShiftSwapDto shiftSwapDto)
        {
            try
            {
                return await Task.Run(() =>{
                    var result = ShiftConfigPL.SaveShiftOverTimeRequest(shiftSwapDto);
                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Your requested shift overtime for: {0} has been saved successfully and now awaiting approval.", shiftSwapDto.Date));
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
        #endregion
    }
}
