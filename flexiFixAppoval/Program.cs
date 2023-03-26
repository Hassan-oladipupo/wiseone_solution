using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using flexiCoreLibrary.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace flexiFixAppoval
{
    class Program
    {
        static void Main(string[] args)
        {
            //FixImage(); return;
            FixApproval();
        }


        public class ShouldSerializeContractResolver : DefaultContractResolver
        {
            public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (property.PropertyType == typeof(string))
                {
                    // Do not include emptry strings
                    property.ShouldSerialize = instance =>
                    {
                        return !string.IsNullOrWhiteSpace(
                            instance.GetType().GetProperty(member.Name).GetValue(instance, null) as string);
                    };
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    // Do not include zero DateTime
                    property.ShouldSerialize = instance =>
                    {
                        return Convert.ToDateTime(instance.GetType().GetProperty(member.Name)
                                   .GetValue(instance, null)) != default(DateTime);
                    };
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    // Do not include zero-length lists
                    switch (member.MemberType)
                    {
                        case MemberTypes.Property:
                            property.ShouldSerialize = instance =>
                            {
                                var enumerable =
                                    instance.GetType().GetProperty(member.Name).GetValue(instance, null) as IEnumerable;
                                return enumerable != null ? enumerable.GetEnumerator().MoveNext() : false;
                            };
                            break;
                        case MemberTypes.Field:
                            property.ShouldSerialize = instance =>
                            {
                                var enumerable =
                                    instance.GetType().GetField(member.Name).GetValue(instance) as IEnumerable;
                                return enumerable != null ? enumerable.GetEnumerator().MoveNext() : false;
                            };
                            break;
                    }
                }

                return property;
            }
        }



        public static void FixImage()
        {
            List<long> list = null;
            using (Wise1neDBEntities db = new Wise1neDBEntities())
            {
                list = db.Staffs.Select(c=>c.ID).ToList();
            }

            foreach (long id in list)
            {
                using (Wise1neDBEntities db = new Wise1neDBEntities())
                {
                    var staff = db.Staffs.FirstOrDefault(c => c.ID == id);
                    if (string.IsNullOrEmpty(staff.Picture) == false)
                    {
                        var base64imageString = staff.Picture.Replace("data:image/jpeg;base64,", "");
                        var path = $@"E:\Projects\ODESK PROJECTS\2020\wise1ne.com\wise1ne_admin\wiseone_solution\flexiFixAppoval\bin\Debug\images\profile_{id}_.jpg";
                        File.WriteAllBytes(path, Convert.FromBase64String(base64imageString));
                        staff.Picture = $"profile_{id}_.jpg";
                        db.SaveChanges();
                    }
                }
            }

        }

        public static void FixApproval()
        {
            List<long> list = null;
            using (Wise1neDBEntities db = new Wise1neDBEntities())
            {
                list = db.Approvals.Where(c=>c.Type == "CancelLeave").Select(c=>c.ID).ToList();
            }

            foreach (long id in list)
            {
                using (Wise1neDBEntities db = new Wise1neDBEntities())
                {

                    Approval approval = db.Approvals.FirstOrDefault(c => c.ID == id);

                    object json = null;

                    switch (approval.Type)
                    {
                        case "ShiftSwap":
                            #region ShiftSwap
                            var swapObject = JsonConvert.DeserializeObject<ShiftSwapRequestDto>(approval.Obj);
                            swapObject.FromShift.Room = new ClassRoomDto()
                            {
                                ID = swapObject.FromShift.Room.ID,
                                Name = swapObject.FromShift.Room.Name
                            };
                            swapObject.ToShift.Room = new ClassRoomDto()
                            {
                                ID = swapObject.ToShift.Room.ID,
                                Name = swapObject.ToShift.Room.Name
                            };
                            json = swapObject; 
                            #endregion
                            break;
                        case "StaffSignUp":
                            #region StaffSignUp
                            var staffObj = JsonConvert.DeserializeObject<StaffDto>(approval.Obj);
                            var user = db.Staffs.FirstOrDefault(c => c.Username == staffObj.Username);
                            if (user == null)
                            {
                                //save image 
                                if (string.IsNullOrEmpty(staffObj.Picture) == false)
                                {
                                    var base64imageString = staffObj.Picture.Replace("data:image/jpeg;base64,", "");
                                    var path = $@"E:\Projects\ODESK PROJECTS\2020\wise1ne.com\wise1ne_admin\wiseone_solution\flexiFixAppoval\bin\Debug\images\profile_{staffObj.Username}_.jpg";
                                    File.WriteAllBytes(path, Convert.FromBase64String(base64imageString));
                                    staffObj.Picture = $"Resources/Images/profile_{staffObj.Username}_.jpg";
                                }
                            } //continue;
                            else
                            {
                                continue;
                                staffObj.Picture = $"Resources/Images/" + user.Picture;
                            }
                            staffObj.Location = new LocationDto()
                            {
                                ID = staffObj.Location.ID,
                                Name = staffObj.Location.Name,
                            };
                            staffObj.Role = new RoleDto()
                            {
                                ID = staffObj.Role.ID,
                                Name = staffObj.Role.Name,
                            };
                            staffObj.Token = null;
                            json = staffObj; 
                            #endregion
                            break;
                        case "CancelLeave":
                            var cancel = JsonConvert.DeserializeObject<StaffLeaveDto>(approval.Obj);
                            foreach (var item1 in cancel.RequestedDays)
                            {
                                item1.Staff = null;
                                item1.NumberOfStaff = 0;
                                item1.FinancialYearMonthID = 0;
                                item1.FinancialYearMonth = null;
                                item1.Available = false;
                            }
                            json = cancel;
                            break;
                        case "ApproveLeave":
                            var leaveApprove= JsonConvert.DeserializeObject<LeaveRequestDto>(approval.Obj);
                            leaveApprove.StaffLocation = new LocationDto()
                            {
                                ID = leaveApprove.StaffLocation.ID,
                                Name = leaveApprove.StaffLocation.Name
                            };

                            foreach (var item in leaveApprove.RequestedDays)
                            {
                                var listRequestdays =new List<StaffLeaveDto>();
                                foreach (var staff1 in item.Staff)
                                {
                                    listRequestdays.Add(new StaffLeaveDto()
                                    {
                                        ID = staff1.ID,
                                        StaffKnownAs = staff1.StaffKnownAs
                                    });
                                }

                                item.Staff = listRequestdays;
                            }


                            json = leaveApprove;
                            break;
                        case "CancelShift":
                            #region CancelShift
                            var shiftObj = JsonConvert.DeserializeObject<SignInOutDto>(approval.Obj);
                            shiftObj.Staff = new StaffDto()
                            {
                                ID = shiftObj.Staff.ID,
                                Picture = shiftObj.Staff.Picture,
                                KnownAs = shiftObj.Staff.KnownAs,
                                Location = new LocationDto()
                                {
                                    ID = shiftObj.Staff.Location.ID,
                                    Name = shiftObj.Staff.Location.Name
                                }
                            };
                            shiftObj.Room = new ClassRoomDto()
                            {
                                ID = shiftObj.Room.ID,
                                Name = shiftObj.Room.Name
                            };

                            var user1 = db.Staffs.FirstOrDefault(c => c.ID == shiftObj.Staff.ID);
                            if (user1 == null)
                            {
                                //save image 
                                if (string.IsNullOrEmpty(shiftObj.Staff.Picture) == false)
                                {
                                    var base64imageString = shiftObj.Staff.Picture.Replace("data:image/jpeg;base64,", "");
                                    var path = $@"E:\Projects\ODESK PROJECTS\2020\wise1ne.com\wise1ne_admin\wiseone_solution\flexiFixAppoval\bin\Debug\images\profile_{shiftObj.Staff.Username}_.jpg";
                                    File.WriteAllBytes(path, Convert.FromBase64String(base64imageString));
                                    shiftObj.Staff.Picture = $"Resources/Images/profile_{shiftObj.Staff.Username}_.jpg";
                                }
                            }
                            else
                            {
                                shiftObj.Staff.Picture = $"Resources/Images/" + user1.Picture;
                            }

                            json = shiftObj; 
                            #endregion
                            break;
                    }

                    if (json != null)
                    {

                        //if (item != null)
                        {
                            string jsonString= JsonConvert.SerializeObject(json,
                                new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    DefaultValueHandling = DefaultValueHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    ContractResolver = ShouldSerializeContractResolver.Instance,
                                });
                            approval.Obj = jsonString;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

    }
}