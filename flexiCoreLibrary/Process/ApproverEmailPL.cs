using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Process
{
    public class ApproverEmailPL
    {
        public static bool Save(ApproverEmailDto emailDto)
        {
            try
            {
                var email = new ApproverEmail
                {
                    ApprovalType = emailDto.ApprovalType,
                    ApprovingRoles = JsonConvert.SerializeObject(emailDto.ApprovingRoles),
                    RequestingRoleID = emailDto.RequestingRole.ID,
                };

                if (ApproverEmailDL.ApprovalEmailExists(email))
                {
                    throw new Exception(string.Format("Notification rule with request type {0} exists already for requesting role {1}.", Enums.GetDescription(typeof(Enums.ApprovalType), emailDto.ApprovalType), emailDto.RequestingRole.Name));
                }
                else
                {
                    return ApproverEmailDL.Save(email);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveSecondLevelEmail(SecondLevelApproverEmailDto emailDto)
        {
            try
            {
                var email = new SecondLevelApproverEmail
                {
                    ApproverRoleID = emailDto.ApproverRole.ID
                };

                if (ApproverEmailDL.SecondLevelApprovalEmailExists(email))
                {
                    throw new Exception($"Approver email {emailDto.ApproverRole.Name} exists already.");
                }
                else
                {
                    return ApproverEmailDL.SaveSecondLevelEmail(email);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(ApproverEmailDto emailDto)
        {
            try
            {
                var email = new ApproverEmail
                {
                    ID = emailDto.ID,
                    ApprovalType = emailDto.ApprovalType,
                    ApprovingRoles = JsonConvert.SerializeObject(emailDto.ApprovingRoles),
                    RequestingRoleID = emailDto.RequestingRole.ID,
                };

                if (ApproverEmailDL.ApprovalEmailExists(email))
                {
                    throw new Exception(string.Format("Notification rule with request type {0} exists already for requesting role {1}.", Enums.GetDescription(typeof(Enums.ApprovalType), emailDto.ApprovalType), emailDto.RequestingRole.Name));
                }
                else
                {
                    return ApproverEmailDL.Update(email);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(ApproverEmailDto emailDto)
        {
            try
            {
                var email = new ApproverEmail
                {
                    ID = emailDto.ID,
                };

                return ApproverEmailDL.Delete(email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteSecondLevelEmail(SecondLevelApproverEmailDto emailDto)
        {
            try
            {
                var email = new SecondLevelApproverEmail
                {
                    ID = emailDto.ID,
                };

                return ApproverEmailDL.DeleteSecondLevelEmail(email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ApproverEmailDto> RetrieveEmails()
        {            
            var flEmails = new List<ApproverEmailDto>();            

            var roles = RolePL.RetrieveRoles();          
              
            var flApproverEmails = ApproverEmailDL.RetrieveEmails();

            if (flApproverEmails.Any())
            {
                flApproverEmails.ForEach(e =>
                {
                    var approvalEmail = new ApproverEmailDto();

                    approvalEmail.ID = e.ID;
                    approvalEmail.ApprovalTypeDesc = Enums.GetDescription(typeof(Enums.ApprovalType), e.ApprovalType);
                    approvalEmail.ApprovalType = e.ApprovalType;
                    approvalEmail.ApprovingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(e.ApprovingRoles);
                    approvalEmail.RequestingRole = roles.Where(r => r.ID == e.RequestingRoleID).FirstOrDefault();

                    flEmails.Add(approvalEmail);
                });
            }
            
            return flEmails;
        }

        public static List<SecondLevelApproverEmailDto> RetrieveSecondLevelEmails()
        {
            var slEmails = new List<SecondLevelApproverEmailDto>();

            var roles = RolePL.RetrieveRoles();

            var slApproverEmails = ApproverEmailDL.RetrieveSecondLevelEmails();

            if (slApproverEmails.Any())
            {
                slApproverEmails.ForEach(e =>
                {
                    var approvalEmail = new SecondLevelApproverEmailDto();

                    approvalEmail.ID = e.ID;
                    approvalEmail.ApproverRole = roles.Where(r => r.ID == e.ApproverRoleID).FirstOrDefault();

                    slEmails.Add(approvalEmail);
                });
            }

            return slEmails;
        }
    }
}
