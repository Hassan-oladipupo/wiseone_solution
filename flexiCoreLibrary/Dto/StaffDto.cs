using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffDto : ApprovalDto
    {
        public long ID { get; set; }
        public string DBS { get; set; }
        public string SafeGuarding { get; set; }
        public string FirstAid { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public string AlternateTelephone { get; set; }
        public string EmergencyContactPhoneNo { get; set; }
        public string Email { get; set; }
        public string OfficialEmail { get; set; }
        public string EmploymentType { get; set; }
        public string NationalInsuranceNumber { get; set; }
        public string Qualifications { get; set; }
        public string DrivingLicense { get; set; }
        public string RegisteredDisabled { get; set; }
        public string ReferenceName1 { get; set; }
        public string ReferencePhoneNumber1 { get; set; }
        public string ReferenceEmail1 { get; set; }
        public string ReferenceName2 { get; set; }
        public string ReferencePhoneNumber2 { get; set; }
        public string ReferenceEmail2 { get; set; }        
        public string Username { get; set; }
        public string Password { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StaffID { get; set; }
        public decimal NumberOfLeaveDays { get; set; }
        public string PhoneModel { get; set; }
        public LocationDto Location { get; set; }
        public RoleDto Role { get; set; }
        public List<FunctionDto> Function { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedOn { get; set; }
        public string Status { get; set; }
        public string Picture { get; set; }
        public string Token { get; set; }

        public bool TaskLeader { get; set; }
        public string LeaveType { get; set; }        
        public string ReferenceCompanyName1 { get; set; }
        public string ReferenceRelationship1 { get; set; }
        public string ReferenceCanContact1 { get; set; }
        public string ReferenceCompanyName2 { get; set; }
        public string ReferenceRelationship2 { get; set; }
        public string ReferenceCanContact2 { get; set; }
        public bool ExistingStaff { get; set; }
        public string AccessCode { get; set; }

        public bool Checked { get; set; }


        public string Name
        {
            get
            {
                return $"{FirstName} {MiddleName} {Surname}";
            }
        }
    }
}
