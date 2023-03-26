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
    public class TaskAPIController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> SaveTask([FromBody]TaskDto taskDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = TaskPL.Save(taskDto);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendNewTaskMail(taskDto));
                        return Request.CreateResponse(HttpStatusCode.OK, "Task created successfully.");
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
        public async Task<HttpResponseMessage> UpdateTask([FromBody]TaskDto taskDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = TaskPL.Update(taskDto);

                    if (result)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(ct => Mail.SendNewTaskMail(taskDto));
                        return Request.CreateResponse(HttpStatusCode.OK, "Task updated successfully.");
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
        public async Task<HttpResponseMessage> SaveTaskUpdate([FromBody]TaskUpdateDto updateDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = TaskPL.SaveTaskUpdate(updateDto);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Task updated successfully.");
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
        public async Task<HttpResponseMessage> DeleteTask([FromBody]TaskDto taskDto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = TaskPL.Delete(taskDto);

                    if (result)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Task deleted successfully.");
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
        public HttpResponseMessage RetrieveTasks([FromBody] SearchFilter filter)
        {
            try
            {
                var tasks = TaskPL.RetrieveTasks(filter);
                var returnedTasks = new { data = tasks };
                return Request.CreateResponse(HttpStatusCode.OK, returnedTasks);
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
        public HttpResponseMessage RetrieveTasksForStaff(long staffID)
        {
            try
            {
                var tasks = TaskPL.RetrieveTasksForStaff(staffID);
                return Request.CreateResponse(HttpStatusCode.OK, tasks);
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
        public HttpResponseMessage RetrieveTaskDetails(long taskID)
        {
            try
            {
                var tasks = TaskPL.RetrieveTaskDetails(taskID);
                return Request.CreateResponse(HttpStatusCode.OK, tasks);
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
        public HttpResponseMessage RetrieveTaskStaff(long taskID)
        {
            try
            {
                var staffs = TaskPL.RetrieveTaskStaff(taskID);
                return Request.CreateResponse(HttpStatusCode.OK, staffs);
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
        public HttpResponseMessage RetrieveTaskUpdates(long taskID)
        {
            try
            {
                var updates = TaskPL.RetrieveTaskUpdates(taskID);
                return Request.CreateResponse(HttpStatusCode.OK, updates);
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
