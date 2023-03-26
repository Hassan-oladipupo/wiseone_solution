using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace flexiCoreLibrary.Process
{
    public class ShiftConfigPL
    {
        public async Task<bool> Save(ShiftConfigDto configDto)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                return SaveShift(configDto);
            });           
        }

        private bool SaveShift(ShiftConfigDto configDto)
        {
            try
            {
                var config = new ShiftConfig
                {
                    CreatedBy = configDto.CreatedBy,
                    CreatedOn = DateUtil.Now(),
                    GeneralInformation = configDto.GeneralInformation,
                    EndDate = DateUtil.GetDate(configDto.EndDate),
                    LocationID = configDto.Location.ID,
                    StartDate = DateUtil.GetDate(configDto.StartDate),
                    WeekName = configDto.WeekName,
                    StaffShifts = (from shift in configDto.StaffShifts
                                   select new StaffShift
                                   {
                                       StaffID = shift.StaffID,
                                       Shift = JsonConvert.SerializeObject(shift.Shift)
                                   }).ToList()
                };

                if (ShiftConfigDL.ShiftConfigurationExists(config.StartDate, config.EndDate, config.LocationID))
                {
                    throw new Exception($"Shift setting for the selected start date: {string.Format("{0:g}", config.StartDate)} exists already");
                }
                else
                {
                    var alreadyConfiguredStaff = ShiftConfigDL.StaffShiftExist(configDto);
                    if (!string.IsNullOrEmpty(alreadyConfiguredStaff))
                    {
                        throw new Exception(alreadyConfiguredStaff);
                    }

                    return ShiftConfigDL.SaveShiftConfiguration(config);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(ShiftConfigDto configDto)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                return UpdateShift(configDto);
            });
        }

        public static bool UpdateShift(ShiftConfigDto configDto)
        {
            try
            {
                var config = new ShiftConfig
                {
                    ID = configDto.ID,
                    EndDate = DateUtil.GetDate(configDto.EndDate),
                    StartDate = DateUtil.GetDate(configDto.StartDate),
                    GeneralInformation = configDto.GeneralInformation,
                    StaffShifts = (from shift in configDto.StaffShifts                                  
                                   select new StaffShift
                                   {
                                       ID = shift.ID,
                                       StaffID = shift.StaffID,
                                       ShiftConfigID = configDto.ID,
                                       Shift = JsonConvert.SerializeObject(shift.Shift)
                                   }).ToList()
                };

                var alreadyConfiguredStaff = ShiftConfigDL.StaffShiftExist(configDto);
                if (!string.IsNullOrEmpty(alreadyConfiguredStaff))
                {
                    throw new Exception(alreadyConfiguredStaff);
                }
                return ShiftConfigDL.UpdateShiftConfiguration(config);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteStaffShiftConfiguration(StaffShiftDto staffShiftConfiguration)
        {
            try
            {                
                return ShiftConfigDL.DeleteStaffShiftConfiguration(staffShiftConfiguration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftConfigDto> RetrieveShiftConfigurations(SearchFilter filter)
        {
            try
            {
                var locations = LocationDL.RetrieveLocations();

                var staffs = StaffDL.RetrieveBasicStaffList();

                var configs = ShiftConfigDL.RetrieveShiftConfigurations(filter);

                var returnedConfigs = new List<ShiftConfigDto>();

                if (configs.Any())
                {
                    configs.ForEach(config =>
                    {
                        var c = new ShiftConfigDto();
                        c.ID = config.ID;
                        c.CreatedBy = config.CreatedBy;
                        c.EndDateStr = string.Format("{0}/{1}/{2}", config.EndDate.Day, config.EndDate.Month, config.EndDate.Year);
                        c.StartDateStr = string.Format("{0}/{1}/{2}", config.StartDate.Day, config.StartDate.Month, config.StartDate.Year);
                        c.WeekName = config.WeekName;
                        c.GeneralInformation = config.GeneralInformation;
                        c.CreatedOn = string.Format("{0:g}", config.CreatedOn.Value);
                        c.LastUpdatedOn = config.LastUpdatedOn != null ? string.Format("{0:g}", config.LastUpdatedOn.Value) : string.Empty;
                        c.Location = (from location in locations
                                      where config.LocationID == location.ID
                                      select new LocationDto()
                                      {
                                          ID = location.ID,
                                          Name = location.Name
                                      }).FirstOrDefault();

                        config.StaffShifts.ToList().ForEach(shift =>
                        {
                            var staff = staffs.Where(s => s.ID == shift.StaffID).FirstOrDefault();

                            if (staff != null)
                            {
                                var st = new StaffShiftDto();
                                st.ID = shift.ID;
                                st.StaffEmail = staff.Email;
                                st.StaffKnownAs = staff.KnownAs;
                                st.StaffName = string.Format("{0} {1}", staff.FirstName, staff.Surname);
                                st.StaffUsername = staff.Username;
                                st.StaffID = shift.StaffID;
                                st.Shift = JsonConvert.DeserializeObject<List<ShiftDto>>(shift.Shift);

                                c.StaffShifts.Add(st);
                            }

                        });

                        returnedConfigs.Add(c);
                    });
                }

                return returnedConfigs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftConfigDto> RetrieveStaffShiftSettings(long staffID)
        {
            try
            {
                var settings = ShiftConfigDL.RetrieveStaffShiftSettings(staffID);

                return settings.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ShiftUtilityDto RetrieveRoomStaffLocation()
        {
            try
            {
                var shiftUtility = new ShiftUtilityDto();

                var classrooms = ClassRoomDL.RetrieveActiveClassRooms();
                var locations = LocationDL.RetrieveActiveLocations();
                var staffs = StaffDL.RetrieveActiveStaffWithoutPicture();

                shiftUtility.Rooms = classrooms;
                shiftUtility.Locations = locations;
                shiftUtility.Staffs = staffs;

                return shiftUtility;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ShiftUtilityDto RetrieveLocationRoomStaffLocation(long locationId)
        {
            try
            {
                var shiftUtility = new ShiftUtilityDto();

                var classrooms = ClassRoomDL.RetrieveActiveClassRoomsInLocation(locationId);
                var locations = LocationDL.RetrieveActiveLocations();
                var staffs = StaffDL.RetrieveActiveStaffWithoutPicture();

                shiftUtility.Rooms = classrooms;
                shiftUtility.Locations = locations;
                shiftUtility.Staffs = staffs;

                return shiftUtility;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffShiftDto RetrieveWeeklyShift(long staffID, long configID)
        {
            try
            {
                var shifts = new List<ShiftDto>();

                var weeklyShift = ShiftConfigDL.RetrieveWeeklyShift(staffID, configID);
                var staffSwaps = ShiftConfigDL.RetrieveStaffShiftSwaps(staffID, weeklyShift.ID);

                var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(weeklyShift.Shift);
                staffShifts.ForEach(shift =>
                {
                    if (shift.Configure)
                    {
                        shift.OverTimeData = $"{(Convert.ToInt32(shift.OverTime) / 60).ToString().PadLeft(2, '0')}.{(Convert.ToInt32(shift.OverTime) % 60).ToString().PadLeft(2, '0')}";
                        var shiftSwap = from swap in staffSwaps
                                        where shift.Date == swap.Date
                                        select swap;

                        if (shiftSwap.Any())
                        {
                            shift.Status = Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.FirstOrDefault().Status);
                        }
                        else
                        {
                            var shiftCancelled = SignInOutDL.CancelledShiftExists(staffID, shift.Date);
                            if(shiftCancelled)
                            {
                                shift.Status = Enums.GetDescription(typeof(Enums.ShiftSwapStatus), Enums.ShiftSwapStatus.ShiftCancelled.ToString());
                            }
                            else
                            {
                                shift.Status = "";
                            }                            
                        }

                        shifts.Add(shift);
                    }
                });

                if (shifts.Any())
                {
                    var staffShift = new StaffShiftDto
                    {
                        ShiftConfigID = weeklyShift.ShiftConfig.ID,
                        GeneralInformation = weeklyShift.ShiftConfig.GeneralInformation,
                        WeekName = weeklyShift.ShiftConfig.WeekName,
                        Shift = shifts,
                        ID = weeklyShift.ID
                    };

                    return staffShift;
                }
                else
                {
                    var staffShift = new StaffShiftDto
                    {
                        ShiftConfigID = weeklyShift.ShiftConfig.ID,
                        GeneralInformation = weeklyShift.ShiftConfig.GeneralInformation,
                        WeekName = weeklyShift.ShiftConfig.WeekName,
                        Shift = new List<ShiftDto>(),
                        ID = weeklyShift.ID
                    };

                    return staffShift;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region ---------- SHIFT SWAP ----------
        public static bool SaveShiftSwapRequest(ShiftSwapDto shiftDto)
        {
            try
            {
                var shiftSwap = new ShiftSwap
                {
                    Date = shiftDto.Date,
                    Day = shiftDto.Day,
                    From = shiftDto.From,
                    RequestedOn = DateUtil.Now(),
                    RoomID = shiftDto.Room.ID,
                    StaffShiftID = shiftDto.StaffShiftID,
                    StaffShiftWeek = shiftDto.StaffShiftWeek,
                    ShiftConfigID = shiftDto.ShiftConfigID,
                    StaffEmail = shiftDto.StaffEmail,
                    StaffID = shiftDto.StaffID,
                    StaffKnownAs = shiftDto.StaffKnownAs,
                    StaffName = shiftDto.StaffName,
                    StaffUsername = shiftDto.StaffUsername,
                    Status = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString(),
                    LocationID = shiftDto.LocationID,
                    To = shiftDto.To
                };

                return ShiftConfigDL.SaveShiftSwapRequest(shiftSwap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveShiftOverTimeRequest(ShiftSwapDto shiftDto)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    var shift = new ShiftSwap
                    {
                        Date = shiftDto.Date,
                        Day = shiftDto.Day,
                        From = shiftDto.From,
                        RequestedOn = DateUtil.Now(),
                        RoomID = shiftDto.Room.ID,
                        StaffShiftID = shiftDto.StaffShiftID,
                        StaffShiftWeek = shiftDto.StaffShiftWeek,
                        ShiftConfigID = shiftDto.ShiftConfigID,
                        StaffEmail = shiftDto.StaffEmail,
                        StaffID = shiftDto.StaffID,
                        StaffKnownAs = shiftDto.StaffKnownAs,
                        StaffName = shiftDto.StaffName,
                        StaffUsername = shiftDto.StaffUsername,
                        Status = Enums.ShiftSwapStatus.ShiftOverTimeRequested.ToString(),
                        LocationID = shiftDto.LocationID,
                        To = shiftDto.To,
                        OverTime = shiftDto.OverTime
                    };

                    shiftDto.ID = ShiftConfigDL.SaveShiftOverTimeRequest(shift);
                    // added by msalem 
                    shiftDto.Room = new ClassRoomDto()
                    {
                        ID = shiftDto.Room.ID,
                        Name = shiftDto.Room.Name
                    };

                    var approval = new Approval
                    {
                        Obj = JsonConvert.SerializeObject(shiftDto,new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                        }),
                        Type = Enums.ApprovalType.ShiftOverTime.ToString(),
                        Status = Enums.ApprovalStatus.Pending.ToString(),
                        RequestedOn = DateUtil.Now()
                    };

                    ApprovalDL.Save(approval);

                    transactionScope.Complete();
                }

                Mail.SendShiftOverTimeRequestMail(shiftDto);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveShiftSwapOptions(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.SaveShiftSwapOptions(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool AcceptShiftSwap(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.AcceptShiftSwap(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CancelShiftSwap(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.CancelShiftSwap(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeclineShiftSwap(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.DeclineShiftSwap(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteShiftSwap(ShiftSwapDto shiftDto)
        {
            try
            {
                var shiftSwap = new ShiftSwap
                {
                    Date = shiftDto.Date,
                    StaffShiftID = shiftDto.StaffShiftID,
                    StaffID = shiftDto.StaffID,
                    LocationID = shiftDto.LocationID,
                    StaffName = shiftDto.StaffName
                };

                return ShiftConfigDL.DeleteShiftSwap(shiftSwap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeclineShiftSwapApproval(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.DeclineShiftSwapApproval(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ApproveShiftSwap(ShiftSwapRequestDto shiftSwapRequest)
        {
            try
            {
                return ShiftConfigDL.ApproveShiftSwap(shiftSwapRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapDto> RetrieveAvailableShiftSwaps(ShiftSwapDto shiftDto)
        {
            try
            {
                var rooms = ClassRoomPL.RetrieveActiveClassRooms();

                var swapRequest = new ShiftSwap
                {
                    Date = shiftDto.Date,
                    From = shiftDto.From,
                    StaffID = shiftDto.StaffID,
                    StaffUsername = shiftDto.StaffUsername,
                    LocationID = shiftDto.LocationID,
                    ShiftConfigID = shiftDto.ShiftConfigID
                };

                var shifts = ShiftConfigDL.RetrieveAvailableShiftSwaps(swapRequest);

                var shiftSwaps = new List<ShiftSwapDto>();

                if (shifts.Any())
                {
                    shifts.ForEach(shiftSwap =>
                    {
                        var date = shiftSwap.Date.Split('/');
                        var shiftYear = Convert.ToInt32(date[2]);
                        var shiftMonth = Convert.ToInt32(date[1]);
                        var shiftDay = Convert.ToInt32(date[0]);

                        var shiftDate = new DateTime(shiftYear, shiftMonth, shiftDay);
                        var today = DateTime.Today;

                        if (today < shiftDate)
                        {
                            var swap = new ShiftSwapDto
                            {
                                Date = shiftSwap.Date,
                                Day = shiftSwap.Day,
                                From = shiftSwap.From,
                                RequestedOn = string.Format("{0:g}", shiftSwap.RequestedOn),
                                Room = (from room in rooms
                                        where room.ID == shiftSwap.RoomID
                                        select room).FirstOrDefault(),
                                StaffEmail = shiftSwap.StaffEmail,
                                StaffID = shiftSwap.StaffID,
                                StaffKnownAs = shiftSwap.StaffKnownAs,
                                StaffName = shiftSwap.StaffName,
                                StaffUsername = shiftSwap.StaffUsername,
                                To = shiftSwap.To,
                                ID = shiftSwap.ID,
                                Status = Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.Status),
                                StaffShiftID = shiftSwap.StaffShiftID
                            };

                            shiftSwaps.Add(swap);
                        }

                    });
                }

                return shiftSwaps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapDto> RetrieveStaffShiftSwaps(string username)
        {
            try
            {
                var rooms = ClassRoomPL.RetrieveActiveClassRooms();

                var shifts = ShiftConfigDL.RetrieveStaffShiftSwaps(username);

                var shiftSwaps = (from shiftSwap in shifts
                                  select new ShiftSwapDto
                                  {
                                      Date = shiftSwap.Date,
                                      Day = shiftSwap.Day,
                                      From = shiftSwap.From,
                                      RequestedOn = string.Format("{0:g}", shiftSwap.RequestedOn),
                                      Room = (from room in rooms
                                              where room.ID == shiftSwap.RoomID
                                              select room).FirstOrDefault(),
                                      StaffEmail = shiftSwap.StaffEmail,
                                      StaffID = shiftSwap.StaffID,
                                      StaffKnownAs = shiftSwap.StaffKnownAs,
                                      StaffName = shiftSwap.StaffName,
                                      StaffUsername = shiftSwap.StaffUsername,
                                      Status = Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.Status),
                                      To = shiftSwap.To
                                  }).ToList();

                return shiftSwaps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapRequestDto> RetrieveShiftSwapRequests(long staffID)
        {
            try
            {
                var rooms = ClassRoomPL.RetrieveActiveClassRooms();

                var shiftRequests = ShiftConfigDL.RetrieveShiftSwapRequests(staffID);

                var shiftSwapRequests = new List<ShiftSwapRequestDto>();

                if (shiftRequests.Any())
                {
                    shiftRequests.ForEach(req =>
                    {
                        var shiftSwapRequest = new ShiftSwapRequestDto();

                        var fromShift = JsonConvert.DeserializeObject<ShiftSwap>(req.FromShift);
                        var toShift = JsonConvert.DeserializeObject<ShiftSwap>(req.ToShift);

                        shiftSwapRequest.FromShift = new ShiftSwapDto
                        {
                            ID = fromShift.ID,
                            Date = fromShift.Date,
                            Day = fromShift.Day,
                            From = fromShift.From,
                            Room = (from room in rooms
                                    where room.ID == fromShift.RoomID
                                    select room).FirstOrDefault(),
                            StaffKnownAs = fromShift.StaffKnownAs,
                            To = fromShift.To
                        };

                        shiftSwapRequest.ToShift = new ShiftSwapDto
                        {
                            ID = toShift.ID,
                            Date = toShift.Date,
                            Day = toShift.Day,
                            From = toShift.From,
                            Room = (from room in rooms
                                    where room.ID == toShift.RoomID
                                    select room).FirstOrDefault(),
                            StaffKnownAs = toShift.StaffKnownAs,
                            To = toShift.To
                        };

                        shiftSwapRequest.ID = req.ID;
                        shiftSwapRequest.DateRequested = string.Format("{0:g}", req.DateRequested);
                        shiftSwapRequest.Status = Enums.GetDescription(typeof(Enums.ShiftSwapStatus), req.Status);
                        shiftSwapRequest.FromStaffID = req.FromStaffID;
                        shiftSwapRequest.ToStaffID = req.ToStaffID;
                        shiftSwapRequest.ToMe = staffID == req.ToStaffID ? true : false;
                        shiftSwapRequest.LocationID = req.LocationID;
                        shiftSwapRequest.DeclineReason = req.DeclineReason;

                        shiftSwapRequests.Add(shiftSwapRequest);

                    });
                }

                return shiftSwapRequests;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapRequestDto> RetrieveShiftSwapForApproval(SearchFilter filter)
        {
            try
            {
                var shiftSwaps = new List<ShiftSwapRequestDto>();

                filter.Type = Enums.ApprovalType.ShiftSwap.ToString();
                var swaps = ApprovalDL.RetrieveFilteredApprovals(filter);

                if (swaps.Any())
                {
                    swaps.ForEach(approval =>
                    {
                        var shiftSwap = JsonConvert.DeserializeObject<ShiftSwapRequestDto>(approval.Obj);

                        if (shiftSwap.LocationID == filter.LocationID)
                        {
                            shiftSwap.ApprovalID = approval.ID;
                            shiftSwap.Status = approval.Status;
                            shiftSwap.RequestedOn = string.Format("{0:g}", approval.RequestedOn.Value);
                            shiftSwap.DeclineReason = approval.DeclineReason ?? string.Empty;

                            shiftSwaps.Add(shiftSwap);
                        }
                    });
                }

                return shiftSwaps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---------- SHIFT OVER TIME --------------------
        public static bool ApproveShiftOverTime(ShiftSwapDto shiftOverTime)
        {
            try
            {
                return ShiftConfigDL.ApproveShiftOverTime(shiftOverTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
