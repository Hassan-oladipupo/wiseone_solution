using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using flexiCoreLibrary.Utility;

namespace flexiCoreLibrary.Data
{
    public class StaffDL
    {
        public static bool ApproveStaff(Staff staff, long approvalId)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.Staffs.Add(staff);
                            context.SaveChanges();

                            var approval = context.Approvals.Where(a => a.ID == approvalId).FirstOrDefault();
                            approval.ApprovedOn = DateUtil.Now();
                            approval.Status = Enums.ApprovalStatus.Approved.ToString();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
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

        public static void UpdateToken(StaffDto staffDto)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staff = context.Staffs.FirstOrDefault(s => s.ID == staffDto.ID);
                    staff.Token = staffDto.Token;
                    context.Entry(staff).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool StaffExists(string username, string email, long id)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffs = context.Staffs
                                    .Where(t => t.Username.Equals(username) || t.Email.Equals(email));

                    if (staffs.Any())
                    {
                        var existingStaff = staffs.FirstOrDefault();

                        if (existingStaff.ID == id)
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

        public static bool StaffIDExists(Staff staff)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffs = context.Staffs
                                    .Where(t => t.StaffID.Equals(staff.StaffID));

                    if (staffs.Any())
                    {
                        var existingStaff = staffs.FirstOrDefault();

                        if (existingStaff.ID == staff.ID)
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

        public static bool EmailExists(Staff staff)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffs = context.Staffs
                                    .Where(t => t.Email.Equals(staff.Email));

                    if (staffs.Any())
                    {
                        var existingStaff = staffs.FirstOrDefault();

                        if (existingStaff.ID == staff.ID)
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

        public static Staff RetrieveStaffByUsername(string username)
        {
            try
            {
                var existingStaff = new Staff();
                using (var context = new DataContext())
                {
                    existingStaff = context.Staffs
                                    .Where(t => t.Username.Equals(username))
                                    .Include(x => x.Location)
                                    .Include(x => x.Role)
                                    .FirstOrDefault();
                }

                return existingStaff;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Staff RetrieveStaffByID(long id)
        {
            try
            {
                var existingStaff = new Staff();
                using (var context = new DataContext())
                {
                    existingStaff = context.Staffs
                                    .Include(x => x.Role)
                                    .Include(x => x.Location)
                                    .Where(t => t.ID.Equals(id))
                                    .FirstOrDefault();
                }

                return existingStaff;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffDto RetrieveBasicStaffByID(long id)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                            .Where(t => t.ID.Equals(id))
                                            .AsEnumerable()
                                            .Select((staff) => new StaffDto()
                                            {
                                                ID = staff.ID,
                                                NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                                StartDate = staff.StartDate,
                                                EndDate = staff.EndDate
                                            })
                                            .FirstOrDefault();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Staff RetrieveStaffByRoleLocationID(long roleID, long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingStaff = context.Staffs
                                    .Where(t => t.RoleID == roleID &&
                                                t.LocationID == locationID &&
                                                t.LeaveType != Enums.LeaveType.Started);

                    return existingStaff.Any() ? existingStaff.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ChangePassword(string username, string password)
        {
            try
            {
                var existingStaff = new Staff();
                using (var context = new DataContext())
                {
                    existingStaff = context.Staffs
                                    .Where(t => t.Username == username)
                                    .FirstOrDefault();

                    if (existingStaff != null)
                    {
                        existingStaff.Password = password;
                        context.Entry(existingStaff).State = EntityState.Modified;
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

        public static List<StaffDto> RetrieveStaff()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                         .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Address = staff.Address,
                                            AlternateTelephone = staff.AlternateTelephone,
                                            CreatedOn = string.Format("{0:g}", staff.CreatedOn.Value),
                                            DBS = staff.DBS,
                                            DOB = staff.DOB,
                                            DrivingLicense = staff.DrivingLicense,
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            EmploymentType = staff.EmploymentType,
                                            EndDate = staff.EndDate,
                                            FirstAid = staff.FirstAid,
                                            FirstName = staff.FirstName,
                                            Gender = staff.Gender,
                                            KnownAs = staff.KnownAs,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            NationalInsuranceNumber = staff.NationalInsuranceNumber,
                                            PhoneModel = staff.PhoneModel,
                                            Picture = staff.Picture.GetPicture(),
                                            PostCode = staff.PostCode,
                                            Qualifications = staff.Qualifications,
                                            ReferenceEmail1 = staff.ReferenceEmail1,
                                            ReferenceEmail2 = staff.ReferenceEmail2,
                                            ReferenceName1 = staff.ReferenceName1,
                                            ReferenceName2 = staff.ReferenceName2,
                                            ReferencePhoneNumber1 = staff.ReferencePhoneNumber1,
                                            ReferencePhoneNumber2 = staff.ReferencePhoneNumber2,
                                            RegisteredDisabled = staff.RegisteredDisabled,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name },
                                            SafeGuarding = staff.SafeGuarding,
                                            StartDate = staff.StartDate,
                                            StaffID = staff.StaffID,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            LastModifiedOn = staff.LastModifiedOn != null ? string.Format("{0:g}",
                                          staff.LastModifiedOn.Value) : string.Empty,
                                            Status = staff.Status,
                                            EmergencyContactPhoneNo = staff.EmergencyContactPhoneNo,
                                            ExistingStaff = staff.ExistingStaff,
                                            ReferenceCanContact1 = staff.ReferenceCanContact1,
                                            ReferenceCompanyName1 = staff.ReferenceCompanyName1,
                                            ReferenceRelationship1 = staff.ReferenceRelationship1,
                                            ReferenceCanContact2 = staff.ReferenceCanContact2,
                                            ReferenceCompanyName2 = staff.ReferenceCompanyName2,
                                            ReferenceRelationship2 = staff.ReferenceRelationship2,
                                            LeaveType = staff.LeaveType.ToString()
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveBasicStaffList()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                         .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Email = staff.Email,
                                            FirstName = staff.FirstName,
                                            KnownAs = staff.KnownAs,
                                            MiddleName = staff.MiddleName,
                                            StaffID = staff.StaffID,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            Status = staff.Status,
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveStaffWithoutPicture()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                         .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            EmploymentType = staff.EmploymentType,
                                            FirstName = staff.FirstName,
                                            Gender = staff.Gender,
                                            KnownAs = staff.KnownAs,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name, Type = staff.Role.Type },
                                            StaffID = staff.StaffID,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            Status = staff.Status,
                                            ExistingStaff = staff.ExistingStaff,
                                            LeaveType = staff.LeaveType.ToString()
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveStaffRoleTypes()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Role")
                                         .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name, Type = staff.Role.Type },
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaff()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                        .Where(u => u.Status == "Active")
                                        .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Address = staff.Address,
                                            AlternateTelephone = staff.AlternateTelephone,
                                            CreatedOn = string.Format("{0:g}", staff.CreatedOn.Value),
                                            DBS = staff.DBS,
                                            DOB = staff.DOB,
                                            DrivingLicense = staff.DrivingLicense,
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            EmploymentType = staff.EmploymentType,
                                            EndDate = staff.EndDate,
                                            FirstAid = staff.FirstAid,
                                            FirstName = staff.FirstName,
                                            Gender = staff.Gender,
                                            KnownAs = staff.KnownAs,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            NationalInsuranceNumber = staff.NationalInsuranceNumber,
                                            PhoneModel = staff.PhoneModel,
                                            Picture = staff.Picture.GetPicture(),
                                            PostCode = staff.PostCode,
                                            Qualifications = staff.Qualifications,
                                            ReferenceEmail1 = staff.ReferenceEmail1,
                                            ReferenceEmail2 = staff.ReferenceEmail2,
                                            ReferenceName1 = staff.ReferenceName1,
                                            ReferenceName2 = staff.ReferenceName2,
                                            ReferencePhoneNumber1 = staff.ReferencePhoneNumber1,
                                            ReferencePhoneNumber2 = staff.ReferencePhoneNumber2,
                                            RegisteredDisabled = staff.RegisteredDisabled,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name },
                                            SafeGuarding = staff.SafeGuarding,
                                            StartDate = staff.StartDate,
                                            StaffID = staff.StaffID,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            LastModifiedOn = staff.LastModifiedOn != null ? string.Format("{0:g}", staff.LastModifiedOn.Value) : string.Empty,
                                            Status = staff.Status,
                                            EmergencyContactPhoneNo = staff.EmergencyContactPhoneNo,
                                            ExistingStaff = staff.ExistingStaff,
                                            ReferenceCanContact1 = staff.ReferenceCanContact1,
                                            ReferenceCompanyName1 = staff.ReferenceCompanyName1,
                                            ReferenceRelationship1 = staff.ReferenceRelationship1,
                                            ReferenceCanContact2 = staff.ReferenceCanContact2,
                                            ReferenceCompanyName2 = staff.ReferenceCompanyName2,
                                            ReferenceRelationship2 = staff.ReferenceRelationship2,
                                            LeaveType = staff.LeaveType.ToString()
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaffWithoutPicture()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                        .Where(u => u.Status == "Active")
                                        .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            EmploymentType = staff.EmploymentType,
                                            FirstName = staff.FirstName,
                                            Gender = staff.Gender,
                                            KnownAs = staff.KnownAs,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name },
                                            StaffID = staff.StaffID,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            Status = staff.Status,
                                            ExistingStaff = staff.ExistingStaff,
                                            LeaveType = staff.LeaveType.ToString()
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaffOnLeave()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                        .Where(u => u.Status == "Active" && u.EmploymentType == "FullTime")
                                        .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Address = staff.Address,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            FirstName = staff.FirstName,
                                            Picture = staff.Picture,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name },
                                            StaffID = staff.StaffID,
                                            Surname = staff.Surname,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            LeaveType = staff.LeaveType.ToString(),
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            KnownAs = staff.KnownAs,
                                            Telephone = staff.Telephone,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaffInLocation(long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffList = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                        .Where(u => u.Status == "Active" && u.LocationID == locationID)
                                        .AsEnumerable()
                                        .Select((staff) => new StaffDto()
                                        {
                                            ID = staff.ID,
                                            Address = staff.Address,
                                            AlternateTelephone = staff.AlternateTelephone,
                                            CreatedOn = string.Format("{0:g}", staff.CreatedOn.Value),
                                            DBS = staff.DBS,
                                            DOB = staff.DOB,
                                            DrivingLicense = staff.DrivingLicense,
                                            Email = staff.Email,
                                            OfficialEmail = staff.OfficialEmail,
                                            EmploymentType = staff.EmploymentType,
                                            EndDate = staff.EndDate,
                                            FirstAid = staff.FirstAid,
                                            FirstName = staff.FirstName,
                                            Gender = staff.Gender,
                                            KnownAs = staff.KnownAs,
                                            Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name },
                                            MiddleName = staff.MiddleName,
                                            NationalInsuranceNumber = staff.NationalInsuranceNumber,
                                            PhoneModel = staff.PhoneModel,
                                            Picture = staff.Picture,
                                            PostCode = staff.PostCode,
                                            Qualifications = staff.Qualifications,
                                            ReferenceEmail1 = staff.ReferenceEmail1,
                                            ReferenceEmail2 = staff.ReferenceEmail2,
                                            ReferenceName1 = staff.ReferenceName1,
                                            ReferenceName2 = staff.ReferenceName2,
                                            ReferencePhoneNumber1 = staff.ReferencePhoneNumber1,
                                            ReferencePhoneNumber2 = staff.ReferencePhoneNumber2,
                                            RegisteredDisabled = staff.RegisteredDisabled,
                                            Role = new RoleDto { ID = staff.Role.ID, Name = staff.Role.Name, CanAccessWeb = staff.Role.CanAccessWeb },
                                            SafeGuarding = staff.SafeGuarding,
                                            StartDate = staff.StartDate,
                                            StaffID = staff.StaffID,
                                            NumberOfLeaveDays = staff.NumberOfLeaveDays,
                                            Surname = staff.Surname,
                                            Telephone = staff.Telephone,
                                            Title = staff.Title,
                                            Username = staff.Username,
                                            LastModifiedOn = staff.LastModifiedOn != null ? string.Format("{0:g}", staff.LastModifiedOn.Value) : string.Empty,
                                            Status = staff.Status,
                                            EmergencyContactPhoneNo = staff.EmergencyContactPhoneNo,
                                            ExistingStaff = staff.ExistingStaff,
                                            ReferenceCanContact1 = staff.ReferenceCanContact1,
                                            ReferenceCompanyName1 = staff.ReferenceCompanyName1,
                                            ReferenceRelationship1 = staff.ReferenceRelationship1,
                                            ReferenceCanContact2 = staff.ReferenceCanContact2,
                                            ReferenceCompanyName2 = staff.ReferenceCompanyName2,
                                            ReferenceRelationship2 = staff.ReferenceRelationship2,
                                            LeaveType = staff.LeaveType.ToString()
                                        })
                                        .ToList();

                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Staff AuthenticateStaff(string username, string password, string token)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staff = context.Staffs
                                        .Include("Location")
                                        .Include("Role.RoleFunctions.Function")
                                        .FirstOrDefault(f => f.Username == username && f.Password == password);

                    if (staff != null && !string.IsNullOrEmpty(token))
                    {
                        staff.Token = token;
                        context.Entry(staff).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    return staff;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(Staff staff)
        {
            try
            {
                var existingStaff = new Staff();
                using (var context = new DataContext())
                {
                    existingStaff = context.Staffs
                                    .Where(t => t.ID == staff.ID)
                                    .FirstOrDefault();

                    if (existingStaff != null)
                    {
                        existingStaff.DBS = staff.DBS;
                        existingStaff.SafeGuarding = staff.SafeGuarding;
                        existingStaff.FirstAid = staff.FirstAid;

                        existingStaff.Title = staff.Title;
                        existingStaff.FirstName = staff.FirstName;
                        existingStaff.MiddleName = staff.MiddleName;
                        existingStaff.Surname = staff.Surname;
                        existingStaff.KnownAs = staff.KnownAs;
                        existingStaff.Gender = staff.Gender;
                        existingStaff.DOB = staff.DOB;
                        existingStaff.Address = staff.Address;
                        existingStaff.PostCode = staff.PostCode;
                        existingStaff.Telephone = staff.Telephone;
                        existingStaff.AlternateTelephone = staff.AlternateTelephone;
                        existingStaff.Email = staff.Email;
                        existingStaff.OfficialEmail = staff.OfficialEmail;
                        existingStaff.EmploymentType = staff.EmploymentType;
                        existingStaff.NationalInsuranceNumber = staff.NationalInsuranceNumber;
                        existingStaff.Qualifications = staff.Qualifications;
                        existingStaff.DrivingLicense = staff.DrivingLicense;
                        existingStaff.RegisteredDisabled = staff.RegisteredDisabled;

                        existingStaff.ReferenceName1 = staff.ReferenceName1;
                        existingStaff.ReferencePhoneNumber1 = staff.ReferencePhoneNumber1;
                        existingStaff.ReferenceEmail1 = staff.ReferenceEmail1;
                        existingStaff.ReferenceName2 = staff.ReferenceName2;
                        existingStaff.ReferencePhoneNumber2 = staff.ReferencePhoneNumber2;
                        existingStaff.ReferenceEmail2 = staff.ReferenceEmail2;

                        existingStaff.StartDate = staff.StartDate;
                        existingStaff.EndDate = staff.EndDate;
                        existingStaff.LocationID = staff.LocationID;
                        existingStaff.RoleID = staff.RoleID;

                        existingStaff.StaffID = staff.StaffID;
                        existingStaff.NumberOfLeaveDays = staff.NumberOfLeaveDays;
                        existingStaff.LastModifiedOn = DateUtil.Now();

                        context.Entry(existingStaff).State = EntityState.Modified;
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

        public static bool UpdateLeaveStatus(Staff staff)
        {
            try
            {
                var existingStaff = new Staff();
                using (var context = new DataContext())
                {
                    existingStaff = context.Staffs
                                    .Where(t => t.ID == staff.ID)
                                    .FirstOrDefault();

                    if (existingStaff != null)
                    {
                        existingStaff.LeaveType = staff.LeaveType;
                        existingStaff.LastModifiedOn = DateUtil.Now();

                        context.Entry(existingStaff).State = EntityState.Modified;
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

        public static bool EnableOrDisable(Staff staff)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingStaff = new Staff();

                    existingStaff = context.Staffs
                                    .Where(t => t.ID == staff.ID)
                                    .FirstOrDefault();

                    if (existingStaff != null)
                    {
                        existingStaff.Status = staff.Status;
                        if (existingStaff.Status == Enums.Status.InActive.ToString())
                        {
                            existingStaff.EndDate = string.Format("{0:dd/MM/yyyy}", DateUtil.Now());
                        }
                        else
                        {
                            existingStaff.EndDate = string.Empty;
                        }
                        existingStaff.LastModifiedOn = DateUtil.Now();

                        context.Entry(existingStaff).State = EntityState.Modified;
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

        public static bool CancelLeave(long staffID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var trnx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var staff = context.Staffs.FirstOrDefault(t => t.ID == staffID);
                            staff.LeaveType = Enums.LeaveType.None;
                            staff.LastModifiedOn = DateUtil.Now();

                            context.Entry(staff).State = EntityState.Modified;
                            context.SaveChanges();

                            var staffLeave = context.StaffLeaves.FirstOrDefault(t => t.StaffID == staffID && t.LeaveType == Enums.LeaveType.Started);
                            staffLeave.LeaveType = Enums.LeaveType.None;

                            context.Entry(staffLeave).State = EntityState.Modified;
                            context.SaveChanges();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            trnx.Rollback();
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

        public static List<string> GetStaffTokens(List<long> ids)
        {
            using (var context = new DataContext())
            {
                var tokens = (from staff in context.Staffs
                              where !string.IsNullOrEmpty(staff.Token) && ids.Contains(staff.ID)
                              select staff.Token).ToList();

                return tokens;
            }
        }

        public static List<string> GetOtherStaffTokens(long staffId, long locationId)
        {
            using (var context = new DataContext())
            {
                var tokens = (from staff in context.Staffs
                              where staff.ID != staffId && staff.LocationID == locationId
                              select staff.Token).ToList();

                return tokens;
            }
        }

        public static List<string> GetStaffToken(long staffId)
        {
            using (var context = new DataContext())
            {
                var tokens = (from staff in context.Staffs
                              where staff.ID == staffId
                              select staff.Token).ToList();

                return tokens;
            }
        }

        public static List<Staff> RetrieveStaffByRoleIDLocationID(long roleID, long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staff = context.Staffs
                                    .Where(t => t.RoleID == roleID &&
                                                t.LocationID == locationID &&
                                                t.LeaveType != Enums.LeaveType.Started)
                                    .ToList();

                    return staff;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveStaffEmailByRoleID(List<long> roleIDs)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffEmails = context.Staffs
                                        .AsEnumerable()
                                        .Where(t => roleIDs.Contains(Convert.ToInt64(t.RoleID)) &&
                                                t.LeaveType != Enums.LeaveType.Started)
                                         .Select(c => new StaffDto()
                                         {
                                             Email = c.Email,
                                             Token = c.Token
                                         }).ToList();

                    return staffEmails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
