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
    public class StaffShiftFeedPL
    {
        public static void PrepareShiftFeedsForTheDay()
        {
            try
            {
                var today = (DateUtil.Now()).DayOfWeek.ToString().ToLower();

                var locations = LocationDL.RetrieveLocations();

                var staffs = StaffDL.RetrieveStaff();

                var latestFeeds = new List<StaffShiftFeed>();

                locations.ForEach(location =>
                {                   
                    var weeklyShifts = SignInOutDL.RetrieveShiftsForTheWeek(location.ID);

                    if (weeklyShifts.Any())
                    {
                        weeklyShifts.ForEach(weeklyShift =>
                        {
                            var todaysShift = new ShiftDto();

                            var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(weeklyShift.Shift);

                            staffShifts.ForEach(shift =>
                            {
                                if (shift.Configure)
                                {
                                    if (shift.Day.ToLower() == today)
                                    {
                                        todaysShift = shift;
                                        todaysShift.StaffID = weeklyShift.StaffID;
                                        todaysShift.StaffShiftID = weeklyShift.ID;

                                        var staff = staffs.Where(s => s.ID == weeklyShift.StaffID).FirstOrDefault();

                                        var latestFeed = new StaffShiftFeed
                                        {
                                            BreakTimeDuration = shift.BreakTimeDuration,
                                            ClockedIn = false,
                                            ClockedOut = false,
                                            CreatedOn = DateUtil.Now(),
                                            Date = shift.Date,
                                            Day = shift.Day,
                                            From = shift.From,                                            
                                            LocationID = location.ID,
                                            LocationName = location.Name,
                                            RoomName = shift.Room.Name,
                                            StaffEmail = staff.Email,
                                            StaffID = staff.ID,
                                            StaffKnownAs = staff.KnownAs,
                                            StaffName = string.Format("{0} {1} {2}", staff.FirstName, staff.MiddleName, staff.Surname),
                                            StaffPhoneNumber = staff.Telephone,
                                            StaffShiftID = weeklyShift.ID,
                                            StaffUsername = staff.Username,
                                            To = shift.To,
                                        };

                                        latestFeeds.Add(latestFeed);
                                    }
                                }
                            });
                        });
                    }
                });

                StaffShiftFeedDL.Save(latestFeeds);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffShiftFeedDto> RetrieveShiftLatestFeeds(long locationID)
        {
            try
            {
                var staffs = StaffDL.RetrieveStaff();

                var latestFeeds = new List<StaffShiftFeedDto>();

                var shiftFeeds = StaffShiftFeedDL.RetrieveLatestFeeds(locationID);

                if(shiftFeeds.Any())
                {
                    shiftFeeds.ForEach(shiftFeed =>
                    {
                        var staff = staffs.Where(x => x.ID == shiftFeed.StaffID).FirstOrDefault();

                        var latestFeed = new StaffShiftFeedDto
                        {
                            BreakTimeDuration = shiftFeed.BreakTimeDuration,
                            ClockedIn = shiftFeed.ClockedIn,
                            ClockedInTime = shiftFeed.ClockedInTime != null ? string.Format("{0:H.mm}", shiftFeed.ClockedInTime) : "--.--",
                            ClockedOut = shiftFeed.ClockedOut,
                            ClockedOutTime= shiftFeed.ClockedOutTime != null ? string.Format("{0:H.mm}", shiftFeed.ClockedOutTime) : "--.--",
                            CreatedOn = string.Format("{0:g}", shiftFeed.CreatedOn),
                            Date = shiftFeed.Date,
                            Day = shiftFeed.Day,                          
                            From = shiftFeed.From.ToString(),                            
                            RoomName = shiftFeed.RoomName,
                            StaffEmail = shiftFeed.StaffEmail,
                            StaffKnownAs = shiftFeed.StaffKnownAs,
                            StaffName = shiftFeed.StaffName,
                            StaffPhoneNumber = shiftFeed.StaffPhoneNumber,
                            StaffPicture = staff.Picture,
                            StaffUsername = shiftFeed.StaffUsername,                            
                            To = shiftFeed.To.ToString(),
                            Absent = shiftFeed.Absent,
                            AbsentReason = shiftFeed.AbsentReason
                        };

                        latestFeeds.Add(latestFeed);
                    });
                }

                return latestFeeds;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
