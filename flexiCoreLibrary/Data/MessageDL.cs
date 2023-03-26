using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class MessageDL
    {
        public static bool SaveMessage(MessageDto message)
        {
            using (var context = new DataContext())
            {
                using (var trnx = context.Database.BeginTransaction())
                {
                    try
                    {
                        message.ToStaffIDs.ForEach(toStaffID =>
                        {
                            var msg = new Message
                            {
                                DateSent = DateUtil.Now(),
                                FromStaffID = message.FromStaffID,
                                MessageText = message.MessageText,
                                ToStaffID = toStaffID
                            };
                            context.Messages.Add(msg);
                            context.SaveChanges();
                        });

                        trnx.Commit();

                    }
                    catch (Exception ex)
                    {
                        trnx.Rollback();
                        throw ex;
                    }
                }
            }

            var staff = StaffDL.RetrieveStaffByID(message.FromStaffID);
            var tokens = StaffDL.GetStaffTokens(message.ToStaffIDs);            
            PushNotification.Engine.SendMessage(tokens, message.MessageText, "New Message", Enums.NotificationType.NewMessage);
            return true;
        }

        public static List<Message> RetrieveConversations(long staffId)
        {
            using (var context = new DataContext())
            {
                var messages = context.Messages
                                        .Where(x => x.FromStaffID == staffId || x.ToStaffID == staffId)
                                        .ToList();
                return messages;
            }
        }
    }
}
