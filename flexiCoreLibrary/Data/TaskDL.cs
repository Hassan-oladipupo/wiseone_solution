using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using flexiCoreLibrary.Dto;

namespace flexiCoreLibrary.Data
{
    public class TaskDL
    {
        public static bool Save(Model.Task task)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.Tasks.Add(task);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
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
                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var task = context.Tasks.Where(t => t.ID == taskDto.ID).FirstOrDefault();

                            task.Subject = taskDto.Subject;
                            task.DateofCompletion = DateUtil.GetDate(taskDto.DateofCompletion);
                            task.Details = taskDto.Details;
                            task.TaskLeader = taskDto.TaskLeader;
                            task.LastUpdatedBy = taskDto.LastUpdatedBy;
                            task.Type = taskDto.TaskStaffs.Count() == 1 ? Enums.TaskType.Individual.ToString() : Enums.TaskType.Group.ToString();
                            task.LastUpdatedOn = DateUtil.Now();

                            context.Entry(task).State = EntityState.Modified;
                            context.SaveChanges();


                            taskDto.TaskStaffs.ToList().ForEach(ts =>
                            {
                                if (ts.Staff.ExistingStaff)
                                {
                                    var taskStaff = context.TaskStaffs.Where(t => t.StaffID == ts.Staff.ID).FirstOrDefault();
                                    taskStaff.TaskLeader = ts.Staff.TaskLeader;

                                    context.Entry(taskStaff).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    var taskStaff = new Model.TaskStaff
                                    {
                                        TaskID = taskDto.ID,
                                        StaffID = ts.Staff.ID,
                                        TaskLeader = ts.Staff.TaskLeader
                                    };

                                    context.TaskStaffs.Add(taskStaff);
                                    context.SaveChanges();
                                }
                            });

                            db.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
                            throw ex;
                        }
                    }
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
                var tokens = new List<string>();
                var taskName = string.Empty;

                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var task = context.Tasks
                                                .Where(t => t.ID == updateDto.Task.ID)
                                                .Include(t => t.TaskStaffs)
                                                .FirstOrDefault();

                            task.LastUpdatedBy = updateDto.ReportedBy;
                            task.LastUpdatedOn = DateUtil.Now();
                            task.Status = updateDto.Status;

                            context.Entry(task).State = EntityState.Modified;
                            context.SaveChanges();

                            var taskUpdate = new Model.TaskUpdate
                            {
                                ReportedBy = updateDto.ReportedBy,
                                ReportedOn = DateUtil.Now(),
                                Status = updateDto.Status,
                                Update = updateDto.Update,
                                TaskID = updateDto.Task.ID
                            };

                            context.TaskUpdates.Add(taskUpdate);
                            context.SaveChanges();

                            var staffIds = (from staff in task.TaskStaffs                                            
                                            select staff.StaffID).ToList();

                            tokens = StaffDL.GetStaffTokens(staffIds);

                            taskName = task.Subject;

                            db.Commit();                            
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
                            throw ex;
                        }
                    }
                }

                PushNotification.Engine.SendMessage(tokens, $"{updateDto.ReportedBy} has just provided an update on the task {taskName}", "Task Update", Enums.NotificationType.TaskUpdate);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(Model.Task task)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingTask = context.Tasks.Where(t => t.ID == task.ID).FirstOrDefault();

                    existingTask.Status = Enums.TaskStatus.Deleted.ToString();
                    existingTask.LastUpdatedBy = task.LastUpdatedBy;
                    existingTask.LastUpdatedOn = DateUtil.Now();

                    context.Entry(existingTask).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool TaskExists(string subject, long taskID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var tasks = context.Tasks
                                    .Where(t => t.Subject.Equals(subject));

                    if (tasks.Any())
                    {
                        var existingTask = tasks.FirstOrDefault();

                        if (existingTask.ID == taskID)
                        {
                            //This condition caters for update of the same name. If the name has not changed then update
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Model.Task> RetrieveTasks(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);
                    var deleted = Enums.TaskStatus.Deleted.ToString();

                    var tasks = context.Tasks
                                        .Where(t => t.CreatedOn >= fromDate && t.CreatedOn <= toDate && t.Status != deleted);

                    if (tasks.Any())
                    {
                        if (!string.IsNullOrEmpty(filter.StaffName))
                        {
                            tasks = tasks.Where(t => t.TaskLeader.Contains(filter.StaffName));
                        }

                        if (!string.IsNullOrEmpty(filter.Status))
                        {
                            tasks = tasks.Where(t => t.Status == filter.Status);
                        }
                    }

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Model.Task RetrieveTaskDetails(long taskID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var task = context.Tasks
                                        .Include(t => t.TaskStaffs.Select(s => s.Staff.Role))
                                        .Include(t => t.TaskStaffs.Select(s => s.Staff.Location))
                                        .Include(t => t.TaskUpdates)
                                        .Where(t => t.ID == taskID)
                                        .FirstOrDefault();

                    return task;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Model.Task> RetrieveTasksForStaff(long staffID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var deleted = Enums.TaskStatus.Deleted.ToString();

                    var tasks = (from ts in context.TaskStaffs.Include(x => x.Task)
                                 where ts.StaffID == staffID
                                 select ts.Task);



                    return tasks.Any() ? tasks.OrderByDescending(t => t.ID).ToList() : new List<Model.Task>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Model.TaskStaff> RetrieveTaskStaff(long taskID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var taskStaff = context.TaskStaffs
                                        .Include(t => t.Staff.Role)
                                        .Include(t => t.Staff.Location)
                                        .Where(t => t.TaskID == taskID)
                                        .ToList();

                    return taskStaff;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Model.TaskUpdate> RetrieveTaskUpdates(long taskID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var taskUpdates = context.TaskUpdates
                                        .Where(t => t.TaskID == taskID)
                                        .ToList();

                    return taskUpdates;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
