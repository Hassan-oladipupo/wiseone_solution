using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Process
{
    public class TaskPL
    {
        public static bool Save(TaskDto taskDto)
        {
            try
            {
                var task = new Model.Task
                {
                    CreatedBy = taskDto.CreatedBy,
                    DateofCompletion = DateUtil.GetDate(taskDto.DateofCompletion),
                    Details = taskDto.Details,
                    Status = taskDto.Status,
                    Subject = taskDto.Subject,
                    TaskLeader = taskDto.TaskLeader,
                    Type = taskDto.TaskStaffs.Count() == 1 ? Enums.TaskType.Individual.ToString() : Enums.TaskType.Group.ToString(),
                    TaskStaffs = (from taskStaff in taskDto.TaskStaffs
                                  select new TaskStaff
                                  {
                                      StaffID = taskStaff.Staff.ID,
                                      TaskLeader = taskStaff.Staff.TaskLeader
                                  }).ToList(),
                    LastUpdatedBy = taskDto.LastUpdatedBy,
                    LastUpdatedOn = DateUtil.Now(),
                    CreatedOn = DateUtil.Now()
                };

                if (TaskDL.TaskExists(task.Subject, task.ID))
                {
                    throw new Exception(string.Format("Task with subject: {0} exists already", task.Subject));
                }
                else
                {
                    return TaskDL.Save(task);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(TaskDto taskDto)
        {
            try
            {
                if (TaskDL.TaskExists(taskDto.Subject, taskDto.ID))
                {
                    throw new Exception(string.Format("Task with subject: {0} exists already", taskDto.Subject));
                }
                else
                {
                    return TaskDL.Update(taskDto);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveTaskUpdate(TaskUpdateDto updateDto)
        {
            try
            {
                return TaskDL.SaveTaskUpdate(updateDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(TaskDto taskDto)
        {
            try
            {
                var task = new Model.Task
                {
                    ID = taskDto.ID,
                    LastUpdatedBy = taskDto.LastUpdatedBy,
                };

                return TaskDL.Delete(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<TaskDto> RetrieveTasks(SearchFilter filter)
        {
            try
            {
                var tasks = TaskDL.RetrieveTasks(filter);

                var returnedTasks = (from task in tasks
                                     select new TaskDto
                                     {
                                         ID = task.ID,
                                         CreatedBy = task.CreatedBy,
                                         CreatedOn = string.Format("{0:g}", task.CreatedOn),
                                         DateofCompletionStr = string.Format("{0:g}", task.DateofCompletion),
                                         Details = task.Details,
                                         LastUpdatedBy = task.LastUpdatedBy,
                                         LastUpdatedOn = string.Format("{0:g}", task.LastUpdatedOn),
                                         Status = task.Status,
                                         Subject = task.Subject,
                                         Type = task.Type,
                                         TaskLeader = task.TaskLeader
                                     }).ToList();

                return returnedTasks;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static TaskDto RetrieveTaskDetails(long taskID)
        {
            try
            {
                var task = TaskDL.RetrieveTaskDetails(taskID);

                var taskDetail = new TaskDto
                {
                    ID = task.ID,
                    DateofCompletionStr = string.Format("{0:dddd, MMMM d, yyyy}", task.DateofCompletion),
                    Details = task.Details,
                    Status = task.Status,
                    Subject = task.Subject,
                    Type = task.Type,
                    TaskLeader = task.TaskLeader,
                    TaskStaffs = (from ts in task.TaskStaffs
                                  select new TaskStaffDto
                                  {
                                      ID = ts.ID,
                                      Staff = new StaffDto
                                      {
                                          ID = ts.Staff.ID,
                                          Email = ts.Staff.Email,
                                          FirstName = ts.Staff.FirstName,
                                          KnownAs = ts.Staff.KnownAs,
                                          Location = new LocationDto { ID = ts.Staff.Location.ID, Name = ts.Staff.Location.Name },
                                          MiddleName = ts.Staff.MiddleName,
                                          Role = new RoleDto { ID = ts.Staff.Role.ID, Name = ts.Staff.Role.Name },
                                          Surname = ts.Staff.Surname,
                                          Telephone = ts.Staff.Telephone,
                                          Username = ts.Staff.Username,
                                          Status = ts.Staff.Status,
                                          LeaveType = ts.Staff.LeaveType.ToString(),
                                          ExistingStaff = true
                                      },
                                      TaskLeader = ts.TaskLeader
                                  }).ToList(),
                    TaskUpdates = (from tu in task.TaskUpdates
                                   select new TaskUpdateDto
                                   {
                                       ID = tu.ID,
                                       ReportedBy = tu.ReportedBy,
                                       ReportedOn = string.Format("{0:g}", tu.ReportedOn),
                                       Status = tu.Status,
                                       Update = tu.Update
                                   }).ToList()
                };

                return taskDetail;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<TaskDto> RetrieveTasksForStaff(long staffID)
        {
            try
            {
                var tasks = TaskDL.RetrieveTasksForStaff(staffID);

                var returnedTasks = (from task in tasks
                                     select new TaskDto
                                     {
                                         ID = task.ID,
                                         CreatedBy = task.CreatedBy,
                                         CreatedOn = string.Format("{0:g}", task.CreatedOn),
                                         DateofCompletionStr = string.Format("{0:dddd, MMMM d, yyyy}", task.DateofCompletion),
                                         Details = task.Details,
                                         LastUpdatedBy = task.LastUpdatedBy,
                                         LastUpdatedOn = string.Format("{0:g}", task.LastUpdatedOn),
                                         Status = task.Status,
                                         Subject = task.Subject,
                                         Type = task.Type,
                                         TaskLeader = task.TaskLeader
                                     }).ToList();

                return returnedTasks;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<TaskStaffDto> RetrieveTaskStaff(long taskID)
        {
            try
            {
                var taskStaff = TaskDL.RetrieveTaskStaff(taskID);

                var staffs = (from ts in taskStaff
                              select new TaskStaffDto
                              {
                                  ID = ts.ID,
                                  Task = new TaskDto
                                  {
                                      ID = taskID
                                  },
                                  Staff = new StaffDto
                                  {
                                      ID = ts.Staff.ID,
                                      Email = ts.Staff.Email,
                                      FirstName = ts.Staff.FirstName,
                                      KnownAs = ts.Staff.KnownAs,
                                      Location = new LocationDto { ID = ts.Staff.Location.ID, Name = ts.Staff.Location.Name },
                                      MiddleName = ts.Staff.MiddleName,
                                      Role = new RoleDto { ID = ts.Staff.Role.ID, Name = ts.Staff.Role.Name },
                                      Surname = ts.Staff.Surname,
                                      Telephone = ts.Staff.Telephone,
                                      Username = ts.Staff.Username,
                                      Status = ts.Staff.Status,
                                      LeaveType = ts.Staff.LeaveType.ToString(),
                                      ExistingStaff = true
                                  },
                                  TaskLeader = ts.TaskLeader
                              }).ToList();

                return staffs;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<TaskUpdateDto> RetrieveTaskUpdates(long taskID)
        {
            try
            {
                var taskUpdates = TaskDL.RetrieveTaskUpdates(taskID);

                var updates = (from tu in taskUpdates
                               select new TaskUpdateDto
                               {
                                   ID = tu.ID,
                                   ReportedBy = tu.ReportedBy,
                                   ReportedOn = string.Format("{0:g}", tu.ReportedOn),
                                   Status = tu.Status,
                                   Update = tu.Update
                               }).ToList();

                return updates;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
