using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class ClassRoomDL
    {
        public static bool Save(ClassRoom classRoom)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.ClassRooms.Add(classRoom);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(ClassRoom classRoom)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingClassRoom = new ClassRoom();

                    existingClassRoom = context.ClassRooms
                                    .Where(t => t.ID == classRoom.ID)
                                    .FirstOrDefault();

                    if (existingClassRoom != null)
                    {
                        existingClassRoom.Name = classRoom.Name;
                        existingClassRoom.LocationID = classRoom.LocationID;
                        existingClassRoom.LastModifiedOn = DateUtil.Now();

                        context.Entry(existingClassRoom).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
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

        public static bool EnableOrDisable(ClassRoom classRoom)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingClassRoom = new ClassRoom();

                    existingClassRoom = context.ClassRooms
                                    .Where(t => t.ID == classRoom.ID)
                                    .FirstOrDefault();

                    if (existingClassRoom != null)
                    {
                        existingClassRoom.Status = classRoom.Status;
                        existingClassRoom.LastModifiedOn = DateUtil.Now();

                        context.Entry(existingClassRoom).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
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

        public static bool ClassRoomExists(ClassRoom classRoom)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var classRooms = context.ClassRooms
                                    .Where(t => t.Name.Equals(classRoom.Name) && t.LocationID.Equals(classRoom.LocationID));

                    if (classRooms.Any())
                    {
                        var existingClassRoom = classRooms.FirstOrDefault();

                        if (existingClassRoom.ID == classRoom.ID)
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

        public static List<ClassRoomDto> RetrieveClassRooms()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var classRooms = context.ClassRooms
                                            .Include(c => c.Location)
                                            .AsEnumerable()
                                             .Select((classroom) => new ClassRoomDto()
                                             {
                                                 ID = classroom.ID,
                                                 Name = classroom.Name,
                                                 Location = new LocationDto { ID = classroom.Location.ID, Name = classroom.Location.Name },
                                                 Status = classroom.Status,
                                                 CreatedOn = string.Format("{0:g}", classroom.CreatedOn.Value),
                                                 LastModifiedOn = classroom.LastModifiedOn != null ? string.Format("{0:g}", classroom.LastModifiedOn.Value) : string.Empty
                                             })
                                            .ToList();
                    return classRooms;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ClassRoomDto> RetrieveActiveClassRooms()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var classRooms = context.ClassRooms
                                            .Include(c => c.Location)
                                            .Where(c => c.Status == "Active")
                                            .AsEnumerable()
                                            .Select((classroom) => new ClassRoomDto()
                                            {
                                                ID = classroom.ID,
                                                Name = classroom.Name,
                                                Location = new LocationDto { ID = classroom.Location.ID, Name = classroom.Location.Name },
                                                Status = classroom.Status,
                                                CreatedOn = string.Format("{0:g}", classroom.CreatedOn.Value),
                                                LastModifiedOn = classroom.LastModifiedOn != null ? string.Format("{0:g}", classroom.LastModifiedOn.Value) : string.Empty
                                            })
                                            .ToList();
                    return classRooms;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ClassRoomDto> RetrieveActiveClassRoomsInLocation(long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var classRooms = context.ClassRooms
                                            .Include(c => c.Location)
                                            .Where(c => c.Status == "Active" && c.LocationID == locationID)
                                            .AsEnumerable()
                                            .Select((classroom) => new ClassRoomDto()
                                            {
                                                ID = classroom.ID,
                                                Name = classroom.Name,
                                                Location = new LocationDto { ID = classroom.Location.ID, Name = classroom.Location.Name },
                                                Status = classroom.Status,
                                                CreatedOn = string.Format("{0:g}", classroom.CreatedOn.Value),
                                                LastModifiedOn = classroom.LastModifiedOn != null ? string.Format("{0:g}", classroom.LastModifiedOn.Value) : string.Empty
                                            })
                                            .ToList();
                    return classRooms;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
