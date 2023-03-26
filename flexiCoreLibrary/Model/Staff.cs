using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Staff
    {
        public Staff()
        {
            this.TaskStaffs = new HashSet<TaskStaff>();
            this.SignInOuts = new HashSet<SignInOut>();
        }

        public long ID { get; set; }
        [MaxLength(10)]
        public string DBS { get; set; }
        [MaxLength(10)]
        public string SafeGuarding { get; set; }
        [MaxLength(10)]
        public string FirstAid { get; set; }
        [MaxLength(10)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string MiddleName { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string KnownAs { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(20)]
        public string DOB { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        [MaxLength(10)]
        public string PostCode { get; set; }
        [MaxLength(20)]
        public string Telephone { get; set; }
        [MaxLength(20)]
        public string AlternateTelephone { get; set; }
        public string EmergencyContactPhoneNo { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string OfficialEmail { get; set; }
        [MaxLength(100)]
        public string EmploymentType { get; set; }
        [MaxLength(20)]
        public string NationalInsuranceNumber { get; set; }
        [MaxLength(250)]
        public string Qualifications { get; set; }
        [MaxLength(20)]
        public string DrivingLicense { get; set; }
        [MaxLength(20)]
        public string RegisteredDisabled { get; set; }
        [MaxLength(50)]
        public string ReferenceName1 { get; set; }
        [MaxLength(20)]
        public string ReferencePhoneNumber1 { get; set; }
        [MaxLength(50)]
        public string ReferenceEmail1 { get; set; }
        public string ReferenceCompanyName1 { get; set; }
        public string ReferenceRelationship1 { get; set; }
        public string ReferenceCanContact1 { get; set; }
        [MaxLength(50)]
        public string ReferenceName2 { get; set; }
        [MaxLength(20)]
        public string ReferencePhoneNumber2 { get; set; }
        [MaxLength(50)]
        public string ReferenceEmail2 { get; set; }
        public string ReferenceCompanyName2 { get; set; }
        public string ReferenceRelationship2 { get; set; }
        public string ReferenceCanContact2 { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        [MaxLength(20)]
        public string StartDate { get; set; }
        [MaxLength(20)]
        public string EndDate { get; set; }
        [MaxLength(20)]
        public string StaffID { get; set; }
        public decimal NumberOfLeaveDays { get; set; }
        [MaxLength(200)]
        public string PhoneModel { get; set; }
        public Nullable<long> LocationID { get; set; }
        public Nullable<long> RoleID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        [MaxLength(20)]
        public string Status { get; set; }
        public Enums.LeaveType LeaveType { get; set; }
        public string Picture { get; set; }                       
        public bool ExistingStaff { get; set; }
        public string Token { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }
        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
        public virtual ICollection<TaskStaff> TaskStaffs { get; set; }
        public virtual ICollection<SignInOut> SignInOuts { get; set; }
    }
}
