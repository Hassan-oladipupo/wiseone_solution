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
    public class ClassRoomPL
    {
        public static bool Save(ClassRoomDto classroomDto)
        {
            try
            {
                var classRoom = new ClassRoom
                {
                    CreatedOn = DateUtil.Now(),
                    LocationID = classroomDto.Location.ID,
                    Name = classroomDto.Name,
                    Status = classroomDto.Status
                };

                if (ClassRoomDL.ClassRoomExists(classRoom))
                {
                    throw new Exception(string.Format("Class room with name: {0} exists already", classRoom.Name));
                }
                else
                {
                    return ClassRoomDL.Save(classRoom);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(ClassRoomDto classroomDto)
        {
            try
            {

                var classRoom = new ClassRoom
                {
                    LocationID = classroomDto.Location.ID,
                    Name = classroomDto.Name,
                    ID = classroomDto.ID
                };

                if (ClassRoomDL.ClassRoomExists(classRoom))
                {
                    throw new Exception(string.Format("Class room with name: {0} exists already", classRoom.Name));
                }
                else
                {
                    return ClassRoomDL.Update(classRoom);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool EnableOrDisable(ClassRoomDto classroomDto)
        {
            try
            {
                var classRoom = new ClassRoom
                {
                    Status = classroomDto.Status,
                    ID = classroomDto.ID
                };

                return ClassRoomDL.EnableOrDisable(classRoom);
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
                var classrooms = ClassRoomDL.RetrieveClassRooms();                

                return classrooms;
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
                var classrooms = ClassRoomDL.RetrieveActiveClassRooms();               

                return classrooms;
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
                var classrooms = ClassRoomDL.RetrieveActiveClassRoomsInLocation(locationID);               

                return classrooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
