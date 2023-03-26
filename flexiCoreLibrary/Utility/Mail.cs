using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using flexiCoreLibrary.Model;
using flexiCoreLibrary.Process;
using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using Newtonsoft.Json;

namespace flexiCoreLibrary
{
    public class Mail
    {

        public static void SendNewUserMail(StaffDto staffDto)
        {
            try
            {
                var role = RolePL.RetrieveRoleByID(staffDto.Role.ID);

                string userFullName = staffDto.FirstName;
                string userUsername = staffDto.Username;
                string userPassword = staffDto.Password;
                string userRole = role.Name;
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Welcome to " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }


                string body = "";

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NewUser.txt"));
                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#UserFullName", userFullName);
                body = body.Replace("#Username", userUsername);
                body = body.Replace("#Password", userPassword);
                body = body.Replace("#Role", userRole);
                body = body.Replace("#WebsiteUrl", websiteUrl);

                var tokens = new List<string>() { staffDto.Token };
                var msg = "Your staff sign up request has been approved. You can now login to Wise1ne mobile to access the features of the app";

                Utility.Util.RunServicesInParallel(
                    () => SendMail(staffDto.Email, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                    () => PushNotification.Engine.SendMessage(tokens, msg, "Staff Sign Up Approval", Enums.NotificationType.StaffSignUpRequest));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendForgotPasswordMail(StaffDto staffDto)
        {
            try
            {
                string key = System.Configuration.ConfigurationManager.AppSettings.Get("ekey");
                string encrypted_username = Crypter.Encrypt(key, staffDto.Username);

                string userFullName = staffDto.FirstName;

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string passwordResetUrl = websiteUrl + "#/resetpassword?rq=" + staffDto.ID.ToString();
                string subject = "Password Reset Request on " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }


                string body = "";

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/ForgotPassword.txt"));
                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#UserFullName", userFullName);
                body = body.Replace("#WebsiteUrl", websiteUrl);
                body = body.Replace("#PasswordResetUrl", passwordResetUrl);

                Mail.SendMail(staffDto.Email, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendStaffSignUpRequestMail(StaffDto staffDto)
        {
            try
            {

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Staff Sign Up Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.StaffSignUp.ToString(), staffDto.Role.ID);

                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staffDto.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        string body = "";
                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/StaffSignUpRequest.txt"));

                        body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staffDto.FirstName, staffDto.MiddleName, staffDto.Surname));
                        body = body.Replace("#StaffRole", staffDto.Role.Name);
                        body = body.Replace("#StaffLocation", staffDto.Location.Name);
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(staffDto.Email);

                        var tokens = approver.Tokens;

                        var msg = $"You have a new Staff Sign up request from {staffDto.Name}. Kindly login to Wise1ne web to view this request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, msg, "Staff Sign Up Approval", Enums.NotificationType.StaffSignUpApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staffDto.FirstName, staffDto.MiddleName, staffDto.Surname));
                    body = body.Replace("#RequestType", "Staff Sign Up");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staffDto.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendShiftOverTimeRequestMail(ShiftSwapDto shiftOverTime)
        {
            try
            {
                var staff = StaffDL.RetrieveStaffByID(shiftOverTime.StaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Shift Over Time Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.ShiftOverTime.ToString(), Convert.ToInt32(staff.RoleID));
                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        string body = "";

                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/ShiftOverTimeRequest.txt"));

                        body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                        body = body.Replace("#StaffLocation", staff.Location.Name);
                        body = body.Replace("#OverTime", $"{(Convert.ToInt32(shiftOverTime.OverTime) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(shiftOverTime.OverTime) % 60).ToString().PadLeft(2, '0')}");
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(staff.Email);

                        var tokens = approver.Tokens;

                        var msg = $"{staff.FirstName} {staff.Surname} has submitted a Shift Over Time request on {applicationName}. It is now awaiting approval. Kindly login to Wise1ne web to view the request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, msg, "Shift Over Time Approval", Enums.NotificationType.ShiftOverTimeApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#RequestType", "Shift Over Time");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendCancelLeaveRequestMail(StaffLeaveDto leaveRequest)
        {
            try
            {
                var staff = StaffPL.RetrieveStaffByID(leaveRequest.StaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Leave Cancellation Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.CancelLeave.ToString(), Convert.ToInt32(staff.Role.ID));

                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        var leaveStart = leaveRequest.RequestedDays[0];

                        var leaveEnd = leaveRequest.RequestedDays[leaveRequest.RequestedDays.Count() - 1];

                        string body = "";

                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/CancelLeave.txt"));

                        body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                        body = body.Replace("#StaffLocation", staff.Location.Name);
                        body = body.Replace("#NumberOfDays", leaveRequest.RequestedDays.Count().ToString());
                        body = body.Replace("#LeaveStartDate", string.Format("{0}/{1}/{2}", leaveStart.Day, leaveStart.Month, leaveStart.Year));
                        body = body.Replace("#LeaveEndDate", string.Format("{0}/{1}/{2}", leaveEnd.Day, leaveEnd.Month, leaveEnd.Year));
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(staff.Email);

                        var tokens = approver.Tokens;

                        var msg = $"{staff.Name} has submitted a leave cancellation request on {applicationName}. It is now awaiting approval. Kindly login to Wise1ne web to view the request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, msg, "Cancel Leave Approval", Enums.NotificationType.CancelLeaveApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#RequestType", "Leave Cancellation");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendLeaveRequestMail(LeaveRequestDto leaveRequest)
        {
            try
            {
                var staff = StaffPL.RetrieveStaffByID(leaveRequest.StaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Leave Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.ApproveLeave.ToString(), Convert.ToInt32(staff.Role.ID));

                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        var leaveStart = leaveRequest.RequestedDays[0];
                        var leaveEnd = leaveRequest.RequestedDays[leaveRequest.RequestedDays.Count() - 1];
                        string body = "";

                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/LeaveRequest.txt"));

                        body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                        body = body.Replace("#StaffLocation", staff.Location.Name);
                        body = body.Replace("#NumberOfDays", leaveRequest.LeaveDaysTaken.ToString());
                        body = body.Replace("#LeaveStartDate", string.Format("{0}/{1}/{2}", leaveStart.Day, leaveStart.Month, leaveStart.Year));
                        body = body.Replace("#LeaveEndDate", string.Format("{0}/{1}/{2}", leaveEnd.Day, leaveEnd.Month, leaveEnd.Year));
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(staff.Email);

                        var tokens = approver.Tokens;

                        var msg = $"{staff.Name} has submitted a Leave request on {applicationName} It is now awaiting approval. Kindly login to Wise1ne web to view the request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, msg, "Leave Approval", Enums.NotificationType.LeaveApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#RequestType", "Leave");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendShiftSwapRequestMail(ShiftSwapRequestDto shiftSwap)
        {
            try
            {
                var fromStaff = StaffPL.RetrieveStaffByID(shiftSwap.FromStaffID);
                var toStaff = StaffPL.RetrieveStaffByID(shiftSwap.ToStaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Shift Swap Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.ShiftSwap.ToString(), fromStaff.Role.ID);

                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, fromStaff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        string body = "";

                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/ShiftSwapRequest.txt"));

                        body = body.Replace("#FromStaff", string.Format("{0} {1} {2}", fromStaff.FirstName, fromStaff.MiddleName, fromStaff.Surname));
                        body = body.Replace("#FromDay", shiftSwap.FromShift.Day);
                        body = body.Replace("#FromDate", shiftSwap.FromShift.Date);
                        body = body.Replace("#FromRoom", shiftSwap.FromShift.Room.Name);
                        body = body.Replace("#FromShiftStart ", shiftSwap.FromShift.From.ToString());
                        body = body.Replace("#FromShiftEnd ", shiftSwap.FromShift.To.ToString());
                        body = body.Replace("#ToStaff", string.Format("{0} {1} {2}", toStaff.FirstName, toStaff.MiddleName, toStaff.Surname));
                        body = body.Replace("#ToDay", shiftSwap.ToShift.Day);
                        body = body.Replace("#ToDate", shiftSwap.ToShift.Date);
                        body = body.Replace("#ToRoom", shiftSwap.ToShift.Room.Name);
                        body = body.Replace("#ToShiftStart ", shiftSwap.ToShift.From.ToString());
                        body = body.Replace("#ToShiftEnd ", shiftSwap.ToShift.To.ToString());
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(fromStaff.Email);
                        cc.Add(toStaff.Email);

                        var tokens = approver.Tokens;

                        var message = $"{fromStaff.Name} and {toStaff.Name} have submitted a Shift Swap request on {applicationName}. It is now awaiting approval. Kindly login to Wise1ne web to view the request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, message, "Shift Swap Approval", Enums.NotificationType.StaffSignUpApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", fromStaff.FirstName, fromStaff.MiddleName, fromStaff.Surname));
                    body = body.Replace("#RequestType", "Shift Swap");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(fromStaff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }

                var token = StaffDL.GetStaffToken(shiftSwap.FromStaffID);
                var msg = $"Your shift swap request has been accepted. The Request is now awaiting final approval";
                PushNotification.Engine.SendMessage(token, msg, "Shift Swap", Enums.NotificationType.ShiftSwapRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }
        }

        public static void SendNewShiftMail(ShiftConfigDto configDto)
        {
            try
            {
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Rota WC {0}/{1}/{2}", configDto.StartDate.Day, configDto.StartDate.Month, configDto.StartDate.Year);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var emailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NewShift.txt"));
                emailBody = emailBody.Replace("#Organization", organization);
                emailBody = emailBody.Replace("#ApplicationName", applicationName);
                emailBody = emailBody.Replace("#WebsiteUrl", websiteUrl);

                var emails = (from staffShift in configDto.StaffShifts
                              select staffShift.StaffEmail).ToList();

                var toEmail = emails.FirstOrDefault();
                var ccEmail = (from email in emails
                               where email != toEmail
                               select email).ToList();

                emailBody = emailBody.Replace("#KnownAs", "");
                emailBody = emailBody.Replace("#FromDate", string.Format("{0}/{1}/{2}", configDto.StartDate.Day, configDto.StartDate.Month, configDto.StartDate.Year));


                Mail.SendMail(toEmail, fromAddress, ccEmail, string.Empty, subject, emailBody, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);

                var week = string.Format("{0}/{1}/{2}", configDto.StartDate.Day, configDto.StartDate.Month, configDto.StartDate.Year);

                var msg = string.Format("Your rota for Week {0} is now available. Login to {1} mobile to view your rota.", week, applicationName);

                var numbers = (from staffShift in configDto.StaffShifts
                               where !string.IsNullOrEmpty(staffShift.StaffTelephone)
                               select staffShift.StaffTelephone).ToArray();

                if (numbers.Any())
                {
                    SMSUtility.SendMessage(msg, numbers);
                }

                var staffIds = (from staffShift in configDto.StaffShifts
                                select staffShift.StaffID).ToList();

                var staffTokens = StaffDL.GetStaffTokens(staffIds);
                if (staffTokens.Any())
                {
                    PushNotification.Engine.SendMessage(staffTokens, msg, $"Rota: {week}", Enums.NotificationType.NewShift);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendLeaveCreationMail(LeaveRequestDto leaveRequest)
        {
            try
            {
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Leave Request - " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }


                string body = "";
                var msg = string.Empty;

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/LeaveApproval.txt"));

                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#StaffName", leaveRequest.StaffName);
                body = body.Replace("#RequestedOn", string.Format("{0:g}", DateUtil.Now()));
                body = body.Replace("#ApprovalStatus", "Approved");
                body = body.Replace("#WebsiteUrl", websiteUrl);

                msg = "A leave has been created for you on Wise1ne";

                SendMail(leaveRequest.StaffEmail, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                string[] numbers = { leaveRequest.StaffTelephone };

                SMSUtility.SendMessage(msg, numbers);

                var tokens = StaffDL.GetStaffToken(leaveRequest.StaffID);
                PushNotification.Engine.SendMessage(tokens, msg, "Leave Approval", Enums.NotificationType.LeaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendLeaveApprovalMail(LeaveApprovalDto leaveApproval)
        {
            try
            {
                var staff = StaffDL.RetrieveStaffByID(leaveApproval.LeaveRequest.StaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Leave Request - " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var cc = new List<string>();

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.ApproveLeave.ToString(), Convert.ToInt32(staff.Role.ID));
                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        cc = approver.Emails.ToList();
                    }
                }

                string body = "";
                var msg = string.Empty;

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/LeaveApproval.txt"));

                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#StaffName", leaveApproval.LeaveRequest.StaffName);
                body = body.Replace("#RequestedOn", leaveApproval.Approval.RequestedOn);

                if (leaveApproval.Approval.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString())
                {
                    body = body.Replace("#ApprovalStatus", $"Approved. {leaveApproval.LeaveRequest.DeclineReason}");
                    msg = $"Your leave request on {leaveApproval.Approval.RequestedOn} has been approved. {leaveApproval.LeaveRequest.DeclineReason}";
                }
                else
                {
                    body = body.Replace("#ApprovalStatus", $"Declined. The reason is: {leaveApproval.LeaveRequest.DeclineReason}");
                    msg = $"Your leave request on {leaveApproval.Approval.RequestedOn} has been declined. The reason is: {leaveApproval.LeaveRequest.DeclineReason}";
                }

                body = body.Replace("#WebsiteUrl", websiteUrl);

                SendMail(leaveApproval.LeaveRequest.StaffEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                string[] numbers = { leaveApproval.LeaveRequest.StaffTelephone };

                SMSUtility.SendMessage(msg, numbers);

                var tokens = StaffDL.GetStaffToken(leaveApproval.LeaveRequest.StaffID);
                PushNotification.Engine.SendMessage(tokens, msg, "Leave Approval", Enums.NotificationType.LeaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendSecondLeaveApprovalMail(LeaveApprovalDto leaveApproval)
        {
            try
            {
                var staff = StaffPL.RetrieveStaffByID(leaveApproval.LeaveRequest.StaffID);

                string organization = ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Second Leave Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;                

                var config = ApproverEmailDL.RetrieveSecondLevelEmails();

                var approverRoleIDs = (from role in config
                                       select role.ApproverRoleID).ToList();

                var approverEmails = StaffDL.RetrieveStaffEmailByRoleID(approverRoleIDs);

                if (approverEmails.Any())
                {
                    var toEmail = approverEmails.FirstOrDefault();

                    var leaveStart = leaveApproval.LeaveRequest.RequestedDays[0];
                    var leaveEnd = leaveApproval.LeaveRequest.RequestedDays[leaveApproval.LeaveRequest.RequestedDays.Count() - 1];
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/SecondLevelLeaveRequest.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#StaffLocation", staff.Location.Name);
                    body = body.Replace("#NumberOfDays", leaveApproval.LeaveRequest.RequestedDays.Count().ToString());
                    body = body.Replace("#LeaveStartDate", string.Format("{0}/{1}/{2}", leaveStart.Day, leaveStart.Month, leaveStart.Year));
                    body = body.Replace("#LeaveEndDate", string.Format("{0}/{1}/{2}", leaveEnd.Day, leaveEnd.Month, leaveEnd.Year));
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);
                    body = body.Replace("#WebsiteUrl", websiteUrl);

                    var cc = approverEmails.Where(e => e != toEmail).Select(e => e.Email).ToList();
                    cc.Add(staff.Email);

                    var tokens = approverEmails.Select(e => e.Token).ToList();

                    var msg = $"{staff.Name}'s leave request is now awaiting your second level approval. Kindly login to Wise1ne web to view the request.";

                    Utility.Util.RunServicesInParallel(
                        () => SendMail(toEmail.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                        () => PushNotification.Engine.SendMessage(tokens, msg, "Leave Approval", Enums.NotificationType.LeaveApproval));
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#RequestType", "Leave");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
               
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendCancelLeaveApprovalMail(StaffLeaveDto leaveApproval)
        {
            try
            {
                var staff = StaffDL.RetrieveStaffByID(leaveApproval.StaffID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Leave Cancellation Request - " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var cc = new List<string>();

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.CancelLeave.ToString(), Convert.ToInt32(staff.Role.ID));
                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        cc = approver.Emails.ToList();
                    }
                }

                string body = "";

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/CancelLeaveApproval.txt"));
                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#StaffName", leaveApproval.StaffName);
                body = body.Replace("#RequestedOn", leaveApproval.Approval.RequestedOn);
                body = body.Replace("#ApprovalStatus", leaveApproval.Approval.ApprovalStatus.ToLower());
                body = body.Replace("#WebsiteUrl", websiteUrl);

                Mail.SendMail(leaveApproval.StaffEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);

                var tokens = StaffDL.GetStaffToken(leaveApproval.StaffID);
                PushNotification.Engine.SendMessage(tokens, $"Your leave cancellation request has been {leaveApproval.Approval.ApprovalStatus.ToLower()}", "Cancel Leave Approval", Enums.NotificationType.CancelLeaveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendDeleteLeaveMail(StaffLeaveDto staffLeave)
        {
            try
            {
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Leave Cancellation Request - " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }


                string body = "";

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/CancelLeaveApproval.txt"));
                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#StaffName", staffLeave.StaffName);
                body = body.Replace("#RequestedOn", staffLeave.RequestedOn);
                body = body.Replace("#ApprovalStatus", "Deleted");
                body = body.Replace("#WebsiteUrl", websiteUrl);

                Mail.SendMail(staffLeave.StaffEmail, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);

                var tokens = StaffDL.GetStaffToken(staffLeave.StaffID);
                PushNotification.Engine.SendMessage(tokens, $"Your leave requested on {staffLeave.RequestedOn} has been cancelled", "Cancel Leave Approval", Enums.NotificationType.CancelLeaveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendNewTaskMail(TaskDto taskDto)
        {
            try
            {
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");

                string subject = string.Format("New Task on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                taskDto.TaskStaffs.ToList().ForEach(s =>
                {
                    var emailBody = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NewTask.txt"));
                    emailBody = emailBody.Replace("#Organization", organization);
                    emailBody = emailBody.Replace("#ApplicationName", applicationName);
                    emailBody = emailBody.Replace("#WebsiteUrl", websiteUrl);
                    emailBody = emailBody.Replace("#Subject", taskDto.Subject);
                    emailBody = emailBody.Replace("#Details", taskDto.Details);
                    emailBody = emailBody.Replace("#DateOfCompletion", taskDto.DateofCompletionStr);
                    emailBody = emailBody.Replace("#UserFullName", s.Staff.FirstName + " " + s.Staff.Surname);

                    SendMail(s.Staff.Email, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, emailBody, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                });

                var staffIds = (from task in taskDto.TaskStaffs
                                select task.Staff.ID).ToList();
                var staffTokens = StaffDL.GetStaffTokens(staffIds);
                if (staffTokens.Any())
                {
                    PushNotification.Engine.SendMessage(staffTokens, "You have been assigned a new task. Login to view the task", "New Task", Enums.NotificationType.NewTask);
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendCancelShiftRequestMail(SignInOutDto shiftCancelRequest)
        {
            try
            {
                var staff = StaffPL.RetrieveStaffByID(shiftCancelRequest.Staff.ID);

                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = string.Format("Shift Cancellation Approval Request on {0}", applicationName);

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }

                var contactStaff = false;

                var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.CancelShift.ToString(), staff.Role.ID);

                if (config != null)
                {
                    var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                    var approver = GetApprovers(approvingRoles, staff.Location.ID);

                    if (approver.Emails.Any())
                    {
                        var toEmail = approver.Emails.FirstOrDefault();

                        string body = "";

                        body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/CancelShift.txt"));

                        body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                        body = body.Replace("#ShiftRoom", shiftCancelRequest.Room.Name);
                        body = body.Replace("#ShiftDate", shiftCancelRequest.Date);
                        body = body.Replace("#Reason", shiftCancelRequest.Reason);
                        body = body.Replace("#Organization", organization);
                        body = body.Replace("#ApplicationName", applicationName);
                        body = body.Replace("#WebsiteUrl", websiteUrl);

                        var cc = approver.Emails.Where(e => e != toEmail).ToList();
                        cc.Add(staff.Email);

                        var tokens = approver.Tokens;

                        var message = $"{staff.Name} has submitted a Cancel Shift request on {applicationName}. It is now awaiting approval. Kindly login to Wise1ne web to view the request.";

                        Utility.Util.RunServicesInParallel(
                            () => SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl),
                            () => PushNotification.Engine.SendMessage(tokens, message, "Cancel Shift Approval", Enums.NotificationType.CancelShiftApproval));
                    }
                    else
                    {
                        contactStaff = true;
                    }
                }
                else
                {
                    contactStaff = true;
                }

                if (contactStaff)
                {
                    string body = "";

                    body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/NoApproverFoundEmail.txt"));

                    body = body.Replace("#StaffName", string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname));
                    body = body.Replace("#RequestType", "Shift Cancellation");
                    body = body.Replace("#Organization", organization);
                    body = body.Replace("#ApplicationName", applicationName);

                    var cc = new List<string>();
                    Mail.SendMail(staff.Email, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendCancelShiftApprovalMail(SignInOutDto shiftCancelRequest)
        {
            try
            {
                string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");
                string subject = "Shift Cancellation Request - " + applicationName;

                string fromAddress = "";
                string smtpUsername = "";
                string smtpPassword = "";
                string smtpHost = "";
                Int32 smtpPort = 587;
                bool smtpUseDefaultCredentials = false;
                bool smtpEnableSsl = true;

                MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                if (mailConfig != null && mailConfig.Mail != null)
                {
                    fromAddress = mailConfig.Mail.FromEmailAddress;
                    smtpUsername = mailConfig.Mail.Username;
                    smtpPassword = mailConfig.Mail.Password;
                }

                if (mailConfig != null && mailConfig.Smtp != null)
                {
                    smtpHost = mailConfig.Smtp.Host;
                    smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                    smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                    smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                }


                string body = "";

                body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/CancelShiftApproval.txt"));
                body = body.Replace("#Organization", organization);
                body = body.Replace("#ApplicationName", applicationName);
                body = body.Replace("#StaffName", shiftCancelRequest.StaffName);
                body = body.Replace("#RequestedOn", shiftCancelRequest.RequestedOn);
                body = body.Replace("#ApprovalStatus", shiftCancelRequest.ApprovalStatus.ToLower());
                body = body.Replace("#WebsiteUrl", websiteUrl);

                Mail.SendMail(shiftCancelRequest.Staff.Email, fromAddress, Enumerable.Empty<string>(), string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);


                var msg = string.Format("Your shift cancellation request on {0} has been {1}.", shiftCancelRequest.RequestedOn, shiftCancelRequest.ApprovalStatus.ToLower());

                string[] numbers = { shiftCancelRequest.Staff.Telephone };

                SMSUtility.SendMessage(msg, numbers);

                var tokens = StaffDL.GetStaffToken(shiftCancelRequest.Staff.ID);
                PushNotification.Engine.SendMessage(tokens, msg, "Cancel Shift Approval", Enums.NotificationType.CancelShiftRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendAbsentReportMail(Enums.ReportType reportType = Enums.ReportType.AbsentReport)
        {
            try
            {
                var absentReports = new List<Report>();
                string subject = string.Empty;
                string message = string.Empty;
                string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationName");

                if (reportType == Enums.ReportType.AbsentReport)
                {
                    absentReports = SignInOutDL.RetrieveAbsentReport();
                    subject = string.Format("{0} - Staff Absence Report Auto-Generated On {1}", string.Format("{0:g}", DateUtil.Now()), applicationName);
                    message = $"Kindly find list of staff that were absent from work today. You can also view these staff on {applicationName}.";
                }
                else
                {
                    absentReports = SignInOutDL.RetrieveAbsentReport(Enums.ReportType.MissingClockOutReport);
                    subject = string.Format("{0} - Staff Missing Clock Out Report Auto-Generated On {1}", string.Format("{0:g}", DateUtil.Now()), applicationName);
                    message = $"Kindly find list of staff that did not clock out today. You can also view these staff on {applicationName}.";
                }

                if (absentReports.Any())
                {
                    var config = ApproverEmailDL.RetrieveApprovalConfiguration(Enums.ApprovalType.EndOfDayReport.ToString(), 0);
                    if (config != null)
                    {
                        var approvingRoles = JsonConvert.DeserializeObject<List<ApprovingRolesDto>>(config.ApprovingRoles);

                        var approver = GetApprovers(approvingRoles, 0);

                        if (approver.Emails.Any())
                        {
                            var toEmail = approver.Emails.FirstOrDefault();

                            string organization = System.Configuration.ConfigurationManager.AppSettings.Get("Organization");
                            string websiteUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WebsiteUrl");

                            string fromAddress = "";
                            string smtpUsername = "";
                            string smtpPassword = "";
                            string smtpHost = "";
                            Int32 smtpPort = 587;
                            bool smtpUseDefaultCredentials = false;
                            bool smtpEnableSsl = true;

                            MailHelper mailConfig = ConfigurationManager.GetSection("mailHelperSection") as MailHelper;
                            if (mailConfig != null && mailConfig.Mail != null)
                            {
                                fromAddress = mailConfig.Mail.FromEmailAddress;
                                smtpUsername = mailConfig.Mail.Username;
                                smtpPassword = mailConfig.Mail.Password;
                            }

                            if (mailConfig != null && mailConfig.Smtp != null)
                            {
                                smtpHost = mailConfig.Smtp.Host;
                                smtpPort = Convert.ToInt32(mailConfig.Smtp.Port);
                                smtpUseDefaultCredentials = Convert.ToBoolean(mailConfig.Smtp.UseDefaultCredentials);
                                smtpEnableSsl = Convert.ToBoolean(mailConfig.Smtp.EnableSsl);
                            }

                            string body = "";

                            body = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/MailTemplates/AbsentReport.txt"));
                            body = body.Replace("#Message", message);
                            body = body.Replace("#Organization", organization);
                            body = body.Replace("#ApplicationName", applicationName);
                            body = body.Replace("#WebsiteUrl", websiteUrl);

                            var report = new StringBuilder();
                            absentReports.ForEach((absentReport) =>
                            {
                                var reportTag = "<tr>";
                                reportTag += $"<td width=\"200\">{absentReport.StaffName}</td>";
                                reportTag += $"<td width=\"200\">{absentReport.LocationName}</td>";
                                reportTag += $"<td width=\"200\">{absentReport.Reason}</td>";
                                reportTag += "</tr>";
                                report.Append(reportTag);
                            });
                            body = body.Replace("#StaffList", report.ToString());

                            var cc = approver.Emails.Where(e => e != toEmail).ToList();

                            Mail.SendMail(toEmail, fromAddress, cc, string.Empty, subject, body, smtpHost, smtpPort, smtpUseDefaultCredentials, smtpUsername, smtpPassword, smtpEnableSsl);

                            SignInOutDL.UpdateReports(absentReports);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                throw ex;
            }

        }

        public static void SendMail(string toAddress, string fromAddress, IEnumerable<string> copyEmailAddress, string fileName, string subject, string body, string smtpHost, Int32 smtpPort, bool smtpUseDefaultCredentials, string smtpUsername, string smtpPassword, bool smtpEnableSsl)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.To.Add(toAddress);
                mail.From = new MailAddress(fromAddress);
                mail.Subject = subject;

                Attachment headerWise1neLogo = new Attachment(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Images/wise1ne.gif"));
                string headerLogoCID = "Wise1neLogo@Header";
                headerWise1neLogo.ContentId = headerLogoCID;
                mail.Attachments.Add(headerWise1neLogo);

                Attachment footerWise1neLogo = new Attachment(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Images/wise1ne.jpg"));
                string footerLogoCID = "Wise1neLogo@Footer";
                footerWise1neLogo.ContentId = footerLogoCID;
                mail.Attachments.Add(footerWise1neLogo);

                body = body.Replace("#HeaderLogo", headerLogoCID);
                body = body.Replace("#FooterLogo", footerLogoCID);

                mail.Body = body;
                mail.IsBodyHtml = true;

                if (copyEmailAddress.Any())
                {
                    copyEmailAddress.ToList().ForEach(ccEmail =>
                    {
                        MailAddress copy = new MailAddress(ccEmail);
                        mail.CC.Add(copy);
                    });
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpHost;
                smtp.Port = smtpPort;
                smtp.UseDefaultCredentials = smtpUseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);// Senders User name and password
                smtp.EnableSsl = smtpEnableSsl;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }

        static bool NetworkIsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        static MailApprover GetApprovers(List<ApprovingRolesDto> approvingRoles, long requestingStaffLocationID)
        {
            var approver = new MailApprover();

            approvingRoles.ForEach((role) =>
            {
                var staffList = new List<Staff>();

                var approvingLocationType = role.ApprovingLocation.Type;
                var approvingRoleID = role.ApprovingRole.ID;

                if (approvingLocationType == Enums.ApprovingLocationTypes.HeadOfficeLocation.ToString())
                {
                    var location = LocationDL.RetrieveHeadOfficeLocation();
                    staffList = StaffDL.RetrieveStaffByRoleIDLocationID(approvingRoleID, location.ID);
                }
                else if (approvingLocationType == Enums.ApprovingLocationTypes.OperationOfficeLocation.ToString())
                {
                    var location = LocationDL.RetrieveOperationOfficeLocation();
                    staffList = StaffDL.RetrieveStaffByRoleIDLocationID(approvingRoleID, location.ID);
                }
                else
                {
                    var location = LocationDL.RetrieveLocationByID(requestingStaffLocationID);
                    staffList = StaffDL.RetrieveStaffByRoleIDLocationID(approvingRoleID, location.ID);
                }

                if (staffList.Any())
                {
                    staffList.ForEach((staff) =>
                    {
                        approver.Emails.Add(staff.OfficialEmail);
                        approver.Tokens.Add(staff.Token);
                    });
                }

            });

            return approver;
        }        
    }
}
