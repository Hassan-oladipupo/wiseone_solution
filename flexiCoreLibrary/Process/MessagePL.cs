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
    public class MessagePL
    {
        public static bool SaveMessage(MessageDto message)
        {
            try
            {
                return MessageDL.SaveMessage(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffConversationDto> RetrieveStaffConversations(long staffId)
        {
            try
            {
                var staffConversations = new List<StaffConversationDto>();

                var messages = MessageDL.RetrieveConversations(staffId);
                if (messages.Any())
                {
                    var staffs = StaffDL.RetrieveStaff();

                    messages.ForEach((message) =>
                    {
                        var staffConversation = new StaffConversationDto();
                        long staffID = 0;
                        var conversation = new ConversationDto()
                        {
                            Date = message.DateSent.ToString("MMM dd, yyyy h:mm tt"),
                            Time = message.DateSent.ToString("h:mm tt"),
                            MessageText = message.MessageText
                        };

                        if (message.FromStaffID == staffId)
                        {
                            conversation.SentByMe = true;
                            staffID = message.ToStaffID;
                            staffConversation = staffConversations.FirstOrDefault(m => m.StaffID == staffID);
                        }
                        else
                        {
                            conversation.SentByMe = false;
                            staffID = message.FromStaffID;
                            staffConversation = staffConversations.FirstOrDefault(m => m.StaffID == staffID);
                        }

                        var staff = staffs.FirstOrDefault(s => s.ID == staffID);

                        if (staffConversation == null)
                        {
                            staffConversation = new StaffConversationDto()
                            {
                                StaffID = staffID,
                                StaffName = $"{staff.FirstName} {staff.MiddleName} {staff.Surname}",
                                StaffPicture = staff.Picture,
                                Date = message.DateSent,
                                DateSent = message.DateSent.ToString("MMM dd, yyyy h:mm tt"),
                                MessageText = message.MessageText,
                                Conversations = new List<ConversationDto>() { conversation }
                            };
                            staffConversations.Add(staffConversation);
                        }
                        else
                        {
                            if (message.DateSent > staffConversation.Date)
                            {
                                staffConversation.Date = message.DateSent;
                                staffConversation.DateSent = message.DateSent.ToString("MMM dd, yyyy h:mm tt");
                                staffConversation.MessageText = message.MessageText;
                            }

                            staffConversation.Conversations.Add(conversation);
                            var staffConverstionIdx = staffConversations.FindIndex(c => c.StaffID == staffConversation.StaffID);
                            staffConversations[staffConverstionIdx] = staffConversation;
                        }
                    });

                    staffConversations = staffConversations.OrderByDescending(s => s.Date).ToList();
                }

                return staffConversations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
