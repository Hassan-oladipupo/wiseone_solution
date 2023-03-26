using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class ApproverEmailDL
    {
        public static bool Save(ApproverEmail email)
        {
            using (var context = new DataContext())
            {
                context.ApproverEmails.Add(email);
                context.SaveChanges();

                return true;
            }
        }

        public static bool SaveSecondLevelEmail(SecondLevelApproverEmail email)
        {
            using (var context = new DataContext())
            {
                context.SecondLevelApproverEmails.Add(email);
                context.SaveChanges();

                return true;
            }
        }

        public static bool Update(ApproverEmail email)
        {
            using (var context = new DataContext())
            {
                var approvalEmail = context.ApproverEmails.Where(e => e.ID == email.ID).FirstOrDefault();

                approvalEmail.ApprovalType = email.ApprovalType;
                approvalEmail.ApprovingRoles = email.ApprovingRoles;                
                approvalEmail.RequestingRoleID = email.RequestingRoleID;

                context.Entry(approvalEmail).State = EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        public static bool ApprovalEmailExists(ApproverEmail email)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var emails = context.ApproverEmails
                                    .Where(t => t.ApprovalType.Equals(email.ApprovalType) &&  
                                                t.RequestingRoleID.Equals(email.RequestingRoleID));

                    if (emails.Any())
                    {
                        var existingEmail = emails.FirstOrDefault();

                        if (existingEmail.ID == email.ID)
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

        public static bool SecondLevelApprovalEmailExists(SecondLevelApproverEmail email)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var emails = context.SecondLevelApproverEmails
                                    .Where(t => t.ApproverRoleID == email.ApproverRoleID);

                    return emails.Any() ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(ApproverEmail email)
        {
            using (var context = new DataContext())
            {
                var approvalEmail = context.ApproverEmails.Where(e => e.ID == email.ID).FirstOrDefault();
                context.ApproverEmails.Remove(approvalEmail);
                context.SaveChanges();

                return true;
            }
        }

        public static bool DeleteSecondLevelEmail(SecondLevelApproverEmail email)
        {
            using (var context = new DataContext())
            {
                var approvalEmail = context.SecondLevelApproverEmails.Where(e => e.ID == email.ID).FirstOrDefault();
                context.SecondLevelApproverEmails.Remove(approvalEmail);
                context.SaveChanges();

                return true;
            }
        }

        public static List<ApproverEmail> RetrieveEmails()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var emails = context.ApproverEmails.ToList();

                    return emails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<SecondLevelApproverEmail> RetrieveSecondLevelEmails()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var emails = context.SecondLevelApproverEmails.ToList();

                    return emails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ApproverEmail RetrieveApprovalConfiguration(string requestType, long requestingRoleID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var config = context.ApproverEmails.FirstOrDefault(x => x.ApprovalType.Equals(requestType) && x.RequestingRoleID == requestingRoleID ); 
                    
                    if(config == null)
                    {
                        config = context.ApproverEmails.FirstOrDefault(x => x.ApprovalType.Equals(requestType));
                    }                   

                    return config;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     
           
    }
}
