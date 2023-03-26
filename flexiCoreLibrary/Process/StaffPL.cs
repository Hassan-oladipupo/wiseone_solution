using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using flexiCoreLibrary.Utility;

namespace flexiCoreLibrary.Process
{
    public class StaffPL
    {
        public static bool LogStaffSignUp(StaffDto staffDto)
        {
            try
            {
                var accessCode = ConfigurationManager.AppSettings.Get("AccessCode");
                if (accessCode == staffDto.AccessCode)
                {
                    var staffExists = StaffDL.StaffExists(staffDto.Username, staffDto.Email, staffDto.ID);
                    if (staffExists)
                    {
                        throw new Exception(string.Format("Username: {0} or Email Address: {1} already exists.", staffDto.Username, staffDto.Email));
                    }
                    else
                    {
                        staffDto.Telephone = "44" + staffDto.Telephone.Remove(0, 1);

                        if (string.IsNullOrEmpty(staffDto.Picture)==false &&  staffDto.Picture.Contains("data:image/jpeg;base64"))
                        {
                            staffDto.Picture.SavePicture(staffDto.Username);
                        }

                        //Add by msalem 
                        staffDto.Location = new LocationDto()
                        {
                            ID = staffDto.Location.ID,
                            Name = staffDto.Location.Name,
                        };
                        staffDto.Role = new RoleDto()
                        {
                            ID = staffDto.Role.ID,
                            Name = staffDto.Role.Name,
                        };
                        staffDto.Token = null;

                        var approval = new Approval
                        {
                            Obj = JsonConvert.SerializeObject(staffDto, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DefaultValueHandling = DefaultValueHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                            }),
                            Type = Enums.ApprovalType.StaffSignUp.ToString(),
                            Status = Enums.ApprovalStatus.Pending.ToString(),
                            RequestedOn = DateUtil.Now()
                        };
                        var log = ApprovalDL.Save(approval);

                        return log;
                    }
                }
                else
                {
                    throw new Exception($"Invalid access code: {staffDto.AccessCode}. Note: Access code is case-sensitive. Carefully re-enter the access code given to you and retry.");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<StaffDto> RetrieveSignUpRequests(SearchFilter filter)
        {
            try
            {
                var returnedSignUps = new List<StaffDto>();

                filter.Type = Enums.ApprovalType.StaffSignUp.ToString();
                var signUps = ApprovalDL.RetrieveFilteredApprovals(filter);

                if (signUps.Any())
                {
                    signUps.ForEach(approval =>
                    {
                        var signUp = JsonConvert.DeserializeObject<StaffDto>(approval.Obj);
                        signUp.ApprovalID = approval.ID;
                        signUp.Status = approval.Status;
                        signUp.RequestedOn = string.Format("{0:g}", approval.RequestedOn.Value);
                        
                        signUp.Picture = signUp.Picture.GetPicture();

                        returnedSignUps.Add(signUp);
                    });
                }

                return returnedSignUps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ApproveStaffSignUp(StaffDto staffDto)
        {
            try
            {
                if (staffDto.Picture.Contains("data:image/jpeg"))
                {
                    staffDto.Picture = $"profile_{staffDto.Username}_.jpg";
                }
                else
                {
                    staffDto.Picture = null;
                }


                staffDto.Telephone = staffDto.Telephone.Replace("-", string.Empty);
                var staff = new Staff
                {
                    Address = staffDto.Address,
                    AlternateTelephone = staffDto.AlternateTelephone,
                    CreatedOn = DateUtil.Now(),
                    DBS = staffDto.DBS,
                    DOB = staffDto.DOB,
                    DrivingLicense = staffDto.DrivingLicense,
                    Email = staffDto.Email,
                    OfficialEmail = staffDto.OfficialEmail,
                    EmploymentType = staffDto.EmploymentType,
                    EndDate = staffDto.EndDate,
                    FirstAid = staffDto.FirstAid,
                    FirstName = staffDto.FirstName,
                    Gender = staffDto.Gender,
                    KnownAs = staffDto.KnownAs,
                    LocationID = staffDto.Location.ID,
                    MiddleName = staffDto.MiddleName,
                    NationalInsuranceNumber = staffDto.NationalInsuranceNumber,
                    Password = PasswordHash.MD5Hash(staffDto.Password),
                    PhoneModel = staffDto.PhoneModel,
                    Picture = staffDto.Picture,
                    PostCode = staffDto.PostCode,
                    Qualifications = staffDto.Qualifications,
                    ReferenceEmail1 = staffDto.ReferenceEmail1,
                    ReferenceEmail2 = staffDto.ReferenceEmail2,
                    ReferenceName1 = staffDto.ReferenceName1,
                    ReferenceName2 = staffDto.ReferenceName2,
                    ReferencePhoneNumber1 = staffDto.ReferencePhoneNumber1,
                    ReferencePhoneNumber2 = staffDto.ReferencePhoneNumber2,
                    RegisteredDisabled = staffDto.RegisteredDisabled,
                    RoleID = staffDto.Role.ID,
                    SafeGuarding = staffDto.SafeGuarding,
                    StartDate = staffDto.StartDate,
                    StaffID = staffDto.StaffID,
                    NumberOfLeaveDays = staffDto.NumberOfLeaveDays,
                    Status = staffDto.Status,
                    Surname = staffDto.Surname,
                    Telephone = staffDto.Telephone,
                    Title = staffDto.Title,
                    Username = staffDto.Username,
                    EmergencyContactPhoneNo = staffDto.EmergencyContactPhoneNo,
                    ExistingStaff = staffDto.ExistingStaff,
                    ReferenceCanContact1 = staffDto.ReferenceCanContact1,
                    ReferenceCompanyName1 = staffDto.ReferenceCompanyName1,
                    ReferenceRelationship1 = staffDto.ReferenceRelationship1,
                    ReferenceCanContact2 = staffDto.ReferenceCanContact2,
                    ReferenceCompanyName2 = staffDto.ReferenceCompanyName2,
                    ReferenceRelationship2 = staffDto.ReferenceRelationship2,
                    LeaveType = Enums.LeaveType.None,
                    Token = staffDto.Token                    
                };

                if (StaffDL.StaffExists(staff.Username, staff.Email, staff.ID))
                {
                    throw new Exception(string.Format("Username: {0} or Email Address: {1} already exists.", staffDto.Username, staffDto.Email));
                }
                else if (StaffDL.StaffIDExists(staff))
                {
                    throw new Exception(string.Format("Staff ID: {0} already exists.", staffDto.StaffID));
                }
                else
                {
                    return StaffDL.ApproveStaff(staff, staffDto.ApprovalID);
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
                StaffDL.UpdateToken(staffDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateStaff(StaffDto staffDto)
        {
            try
            {
                var staff = new Staff
                {
                    ID = staffDto.ID,
                    Address = staffDto.Address,
                    AlternateTelephone = staffDto.AlternateTelephone,
                    DBS = staffDto.DBS,
                    DOB = staffDto.DOB,
                    DrivingLicense = staffDto.DrivingLicense,
                    Email = staffDto.Email,
                    OfficialEmail = staffDto.OfficialEmail,
                    EmploymentType = staffDto.EmploymentType,
                    EndDate = staffDto.EndDate,
                    FirstAid = staffDto.FirstAid,
                    FirstName = staffDto.FirstName,
                    Gender = staffDto.Gender,
                    KnownAs = staffDto.KnownAs,
                    LocationID = staffDto.Location.ID,
                    MiddleName = staffDto.MiddleName,
                    NationalInsuranceNumber = staffDto.NationalInsuranceNumber,
                    PhoneModel = staffDto.PhoneModel,
                    Picture = staffDto.Picture,
                    PostCode = staffDto.PostCode,
                    Qualifications = staffDto.Qualifications,
                    ReferenceEmail1 = staffDto.ReferenceEmail1,
                    ReferenceEmail2 = staffDto.ReferenceEmail2,
                    ReferenceName1 = staffDto.ReferenceName1,
                    ReferenceName2 = staffDto.ReferenceName2,
                    ReferencePhoneNumber1 = staffDto.ReferencePhoneNumber1,
                    ReferencePhoneNumber2 = staffDto.ReferencePhoneNumber2,
                    RegisteredDisabled = staffDto.RegisteredDisabled,
                    RoleID = staffDto.Role.ID,
                    SafeGuarding = staffDto.SafeGuarding,
                    StartDate = staffDto.StartDate,
                    StaffID = staffDto.StaffID,
                    NumberOfLeaveDays = staffDto.NumberOfLeaveDays,
                    Surname = staffDto.Surname,
                    Telephone = staffDto.Telephone,
                    Title = staffDto.Title,
                    EmergencyContactPhoneNo = staffDto.EmergencyContactPhoneNo,
                    ExistingStaff = staffDto.ExistingStaff,
                    ReferenceCanContact1 = staffDto.ReferenceCanContact1,
                    ReferenceCompanyName1 = staffDto.ReferenceCompanyName1,
                    ReferenceRelationship1 = staffDto.ReferenceRelationship1,
                    ReferenceCanContact2 = staffDto.ReferenceCanContact2,
                    ReferenceCompanyName2 = staffDto.ReferenceCompanyName2,
                    ReferenceRelationship2 = staffDto.ReferenceRelationship2
                };

                if (StaffDL.EmailExists(staff))
                {
                    throw new Exception(string.Format("Email address: {0} already exists.", staff.Email));
                }
                else if (StaffDL.StaffIDExists(staff))
                {
                    throw new Exception(string.Format("Staff ID: {0} already exists.", staff.StaffID));
                }
                else
                {
                    return StaffDL.Update(staff);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateLeaveStatus(StaffDto staffDto)
        {
            try
            {
                var staff = new Staff
                {
                    ID = staffDto.ID,
                    LeaveType = Enums.LeaveType.None
                };

                return StaffDL.UpdateLeaveStatus(staff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool EnableOrDisable(StaffDto staffDto)
        {
            try
            {
                var staff = new Staff
                {
                    ID = staffDto.ID,
                    Status = staffDto.Status
                };

                return StaffDL.EnableOrDisable(staff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CancelLeave(StaffDto staffDto)
        {
            try
            {
                return StaffDL.CancelLeave(staffDto.ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeclineStaffSignUp(StaffDto staffDto)
        {
            try
            {
                var approval = new Approval
                {
                    ID = staffDto.ApprovalID,
                    Status = Enums.ApprovalStatus.Declined.ToString()
                };
                return ApprovalDL.Update(approval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveStaffs()
        {
            try
            {
                var staffs = StaffDL.RetrieveStaff();
                
                return staffs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaffs()
        {
            try
            {
                var staffs = StaffDL.RetrieveActiveStaff();                

                return staffs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<StaffDto>> RetrieveActiveStaffOnLeave()
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                return StaffOnLeave();
            });
        }

        public static List<StaffDto> StaffOnLeave()
        {
            try
            {
                var staffs = StaffDL.RetrieveActiveStaffOnLeave();               

                return staffs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffDto> RetrieveActiveStaffsInLocation(long locationID)
        {
            try
            {
                var staffs = StaffDL.RetrieveActiveStaffInLocation(locationID);                

                return staffs;
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
                return StaffDL.ChangePassword(username, password);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffDto AuthenticateStaff(string username, string password, string token)
        {
            try
            {
                var staff = StaffDL.AuthenticateStaff(username, password, token);
                if (staff != null)
                {
                    var staffObj = new StaffDto
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
                        Location = new LocationDto { ID = staff.Location.ID, Name = staff.Location.Name, Latitude = staff.Location.Latitude, Longitude = staff.Location.Longitude },
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
                    };

                    staffObj.Function = (from function in staff.Role.RoleFunctions
                                         select new FunctionDto
                                         {
                                             Name = function.Function.Name,
                                             Module = function.Function.Module,
                                             Type = function.Function.Type,
                                             PageUrl = function.Function.PageUrl
                                         }).ToList();

                    staffObj.Role = new RoleDto
                    {
                        Name = staff.Role.Name,
                        CanAccessWeb = staff.Role.CanAccessWeb,
                        Status = staff.Role.Status,
                        ID = staff.Role.ID
                    };

                    staffObj.Location = new LocationDto
                    {
                        ID = staff.Location.ID,
                        Name = staff.Location.Name,
                        Longitude = staff.Location.Longitude,
                        Latitude = staff.Location.Latitude,
                        Address = staff.Location.Address,
                    };

                    return staffObj;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffDto RetrieveStaffByUsername(string username)
        {
            try
            {
                var staff = StaffDL.RetrieveStaffByUsername(username);
                if (staff != null)
                {
                    return new StaffDto
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
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffDto RetrieveStaffByID(long id)
        {
            try
            {
                var staff = StaffDL.RetrieveStaffByID(id);
                if (staff != null)
                {
                    var s = new StaffDto
                    {
                        ID = staff.ID,
                        Role = new RoleDto
                        {
                            ID = staff.Role.ID,
                            Name = staff.Role.Name
                        },
                        Location = new LocationDto
                        {
                            ID = staff.Location.ID,
                            Name = staff.Location.Name
                        },
                        Email = staff.Email,
                        OfficialEmail = staff.OfficialEmail,
                        EmploymentType = staff.EmploymentType,
                        Username = staff.Username,
                        Surname = staff.Surname,
                        MiddleName = staff.MiddleName,
                        FirstName = staff.FirstName,
                        LeaveType = staff.LeaveType.ToString(),
                        NumberOfLeaveDays = staff.NumberOfLeaveDays
                    };

                    return s;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
