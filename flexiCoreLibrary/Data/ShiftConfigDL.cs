using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using flexiCoreLibrary.Dto;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class ShiftConfigDL
    {
        public static bool SaveShiftConfiguration(ShiftConfig shiftConfig)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.ShiftConfigs.Add(shiftConfig);
                            context.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateShiftConfiguration(ShiftConfig shiftConfig)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var existingShiftConfig = context.ShiftConfigs.Where(sc => sc.ID == shiftConfig.ID).FirstOrDefault();

                            existingShiftConfig.EndDate = shiftConfig.EndDate;
                            existingShiftConfig.StartDate = shiftConfig.StartDate;
                            existingShiftConfig.GeneralInformation = shiftConfig.GeneralInformation;
                            existingShiftConfig.LastUpdatedOn = DateUtil.Now();

                            context.Entry(existingShiftConfig).State = EntityState.Modified;
                            context.SaveChanges();

                            shiftConfig.StaffShifts.ToList().ForEach(shift =>
                            {
                                var existingShift = context.StaffShifts.Where(s => s.StaffID == shift.StaffID && s.ShiftConfigID == shift.ShiftConfigID).FirstOrDefault();

                                if (existingShift != null)
                                {
                                    existingShift.Shift = shift.Shift;
                                    context.Entry(existingShift).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    context.StaffShifts.Add(shift);
                                    context.SaveChanges();
                                }

                            });


                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
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
                using (var context = new DataContext())
                {
                    var signedIn = false;

                    foreach (var shift in staffShiftConfiguration.Shift)
                    {
                        var signIn = context.SignInOuts.FirstOrDefault(x => x.StaffID == staffShiftConfiguration.StaffID &&
                                                                            x.StaffShiftID == staffShiftConfiguration.ID &&
                                                                            x.Date == shift.Date);

                        if (signIn != null)
                        {
                            signedIn = true;
                            break;
                        };
                    }

                    if (signedIn)
                    {
                        throw new Exception($"Invalid operation. Reason is: staff has already clocked in during the week.");
                    }
                    else
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                var staffShift = context.StaffShifts.Include(s => s.ShiftConfig).Where(sc => sc.ID == staffShiftConfiguration.ID).FirstOrDefault();

                                var shiftConfig = context.ShiftConfigs.FirstOrDefault(s => s.ID == staffShift.ShiftConfig.ID);
                                shiftConfig.LastUpdatedOn = DateTime.Now;

                                context.Entry(shiftConfig).State = EntityState.Modified;
                                context.SaveChanges();

                                if (staffShift != null)
                                {
                                    context.StaffShifts.Remove(staffShift);
                                    context.SaveChanges();
                                }

                                

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string StaffShiftExist(ShiftConfigDto configDto)
        {
            try
            {
                var exists = new StringBuilder();

                var startDate = DateUtil.GetDate(configDto.StartDate);

                var endDate = DateUtil.GetDate(configDto.EndDate);

                var locations = LocationDL.RetrieveLocations();

                using (var context = new DataContext())
                {
                    configDto.StaffShifts.ToList().ForEach(shift =>
                    {
                        var existingShift = context.StaffShifts
                                                    .Include(s => s.ShiftConfig)
                                                    .Where(s => s.ShiftConfig.StartDate == startDate &&
                                                                s.ShiftConfig.EndDate == endDate &&
                                                                s.ShiftConfig.LocationID != configDto.Location.ID &&
                                                                s.StaffID == shift.StaffID).ToList();

                        if (existingShift.Any())
                        {
                            existingShift.ForEach((oldShift) =>
                            {
                                var location = locations.FirstOrDefault(l => l.ID == oldShift.ShiftConfig.LocationID);

                                var staffShift = JsonConvert.DeserializeObject<List<ShiftDto>>(oldShift.Shift);
                                shift.Shift.ForEach((newShift) =>
                                {
                                    var alreadyConfigured = (from s in staffShift
                                                             where s.Date == newShift.Date &&
                                                                   s.Configure == true &&
                                                                   newShift.Configure == true
                                                             select s);

                                    if (alreadyConfigured.Any())
                                    {
                                        exists.Append($"{shift.StaffName} has shift on {newShift.Day} in {location.Name} | ");
                                    }
                                });
                            });
                        }
                    });
                }

                return exists.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ShiftConfigurationExists(DateTime startDate, DateTime endDate, long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var config = context.ShiftConfigs
                                    .Where(t => ((t.StartDate == startDate && t.EndDate == endDate) || startDate >= t.StartDate && startDate <= t.EndDate) && t.LocationID == locationID);

                    if (config.Any())
                    {
                        return true;
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

        public static List<ShiftConfig> RetrieveShiftConfigurations(SearchFilter filter)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var fromDate = DateUtil.GetDate(filter.FromDate);
                    var toDate = DateUtil.GetDate(filter.ToDate).AddDays(1);

                    var configs = context.ShiftConfigs
                                        .Include(sc => sc.StaffShifts)
                                        .Where(sc => sc.CreatedOn >= fromDate && sc.CreatedOn <= toDate)
                                        .ToList();

                    return configs;
                }
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
                using (var context = new DataContext())
                {
                    var locations = context.Locations.ToList();

                    var shifts = (from staffConfig in context.ShiftConfigs
                                  join staffShift in context.StaffShifts on staffConfig.ID equals staffShift.ShiftConfigID
                                  where staffShift.StaffID == staffID
                                  select staffConfig)
                                 .Distinct()
                                 .AsEnumerable()
                                 .Select(s => new ShiftConfigDto()
                                 {
                                     ID = s.ID,
                                     EndDateStr = string.Format("{0:ddd, MMM d, yyyy}", s.EndDate),
                                     StartDateStr = string.Format("{0:ddd, MMM d, yyyy}", s.StartDate),
                                     WeekName =  s.WeekName + " - " + string.Format("{0:yyyy}", s.EndDate) + "| " + locations.FirstOrDefault(l => l.ID == s.LocationID).Name
                                 })
                                 .OrderByDescending(x => x.ID)
                                 .Take(10)
                                 .ToList();

                    return shifts.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StaffShift RetrieveWeeklyShift(long staffID, long configID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var shifts = (from staffShift in context.StaffShifts
                                  join staffConfig in context.ShiftConfigs on staffShift.ShiftConfigID equals staffConfig.ID
                                  where staffShift.StaffID == staffID && staffConfig.ID == configID
                                  select staffShift)
                                 .Include(sc => sc.ShiftConfig)
                                 .Distinct();


                    return shifts.Any() ? shifts.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region---------- SHIFT SWAP -----------
        public static bool SaveShiftSwapRequest(ShiftSwap shiftSwap)
        {
            var tokens = new List<string>();

            try
            {
                using (var context = new DataContext())
                {
                    var shifts = context.ShiftSwaps
                                        .Where(ss => ss.StaffShiftID == shiftSwap.StaffShiftID &&
                                                    ss.StaffID == shiftSwap.StaffID &&
                                                    ss.Date == shiftSwap.Date);

                    if (shifts.Any())
                    {
                        var shift = shifts.FirstOrDefault();
                        shift.Status = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString();
                        context.Entry(shift).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        context.ShiftSwaps.Add(shiftSwap);
                        context.SaveChanges();
                    }

                    tokens = StaffDL.GetOtherStaffTokens(shiftSwap.StaffID, shiftSwap.LocationID);
                }

                var msg = $"{shiftSwap.StaffName}'s shift on {shiftSwap.Day}, {shiftSwap.Date} is available for swapping. If you are interested, login to the Wise1ne app to swap.";
                PushNotification.Engine.SendMessage(tokens, msg, "Shift Swap", Enums.NotificationType.ShiftSwapAvailability);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static long SaveShiftOverTimeRequest(ShiftSwap shiftSwap)
        {
            try
            {
                long savedId = 0;

                using (var context = new DataContext())
                {
                    var shifts = context.ShiftSwaps
                                        .Where(ss => ss.StaffShiftID == shiftSwap.StaffShiftID &&
                                                    ss.StaffID == shiftSwap.StaffID &&
                                                    ss.Date == shiftSwap.Date);

                    if (shifts.Any())
                    {
                        var shift = shifts.FirstOrDefault();
                        shift.Status = Enums.ShiftSwapStatus.ShiftOverTimeRequested.ToString();
                        context.Entry(shift).State = EntityState.Modified;
                        context.SaveChanges();

                        savedId = shift.ID;
                    }
                    else
                    {
                        context.ShiftSwaps.Add(shiftSwap);
                        context.SaveChanges();

                        savedId = shiftSwap.ID;
                    }
                }

                return savedId;
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
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Save From Shift Request
                            var fromShiftSwap = new ShiftSwap();

                            var fromShiftSwapExists = context.ShiftSwaps
                                               .Where(ss => ss.StaffShiftID == shiftSwapRequest.FromShift.StaffShiftID &&
                                                            ss.StaffID == shiftSwapRequest.FromShift.StaffID &&
                                                            ss.Date == shiftSwapRequest.FromShift.Date);

                            if (fromShiftSwapExists.Any())
                            {
                                fromShiftSwap = fromShiftSwapExists.FirstOrDefault();

                                fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapRequested.ToString();
                                fromShiftSwap.From = shiftSwapRequest.FromShift.From;
                                fromShiftSwap.To = shiftSwapRequest.FromShift.To;
                                fromShiftSwap.RequestedOn = DateUtil.Now();
                                context.Entry(fromShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            else
                            {
                                fromShiftSwap.Date = shiftSwapRequest.FromShift.Date;
                                fromShiftSwap.Day = shiftSwapRequest.FromShift.Day;
                                fromShiftSwap.From = shiftSwapRequest.FromShift.From;
                                fromShiftSwap.RequestedOn = DateUtil.Now();
                                fromShiftSwap.RoomID = shiftSwapRequest.FromShift.Room.ID;
                                fromShiftSwap.StaffShiftID = shiftSwapRequest.FromShift.StaffShiftID;
                                fromShiftSwap.StaffShiftWeek = shiftSwapRequest.FromShift.StaffShiftWeek;
                                fromShiftSwap.StaffEmail = shiftSwapRequest.FromShift.StaffEmail;
                                fromShiftSwap.StaffID = shiftSwapRequest.FromShift.StaffID;
                                fromShiftSwap.StaffKnownAs = shiftSwapRequest.FromShift.StaffKnownAs;
                                fromShiftSwap.StaffName = shiftSwapRequest.FromShift.StaffName;
                                fromShiftSwap.StaffUsername = shiftSwapRequest.FromShift.StaffUsername;
                                fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapRequested.ToString();
                                fromShiftSwap.LocationID = shiftSwapRequest.FromShift.LocationID;
                                fromShiftSwap.ShiftConfigID = shiftSwapRequest.FromShift.ShiftConfigID;
                                fromShiftSwap.To = shiftSwapRequest.FromShift.To;

                                context.ShiftSwaps.Add(fromShiftSwap);
                                context.SaveChanges();
                            }

                            //Update the Status of To Shift request
                            var toShiftSwap = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                .FirstOrDefault();

                            toShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapRequested.ToString();
                            context.Entry(toShiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Create and Save Shift Swap Request Option
                            var staffShift = context.StaffShifts.Include(s => s.ShiftConfig).FirstOrDefault(s => s.ID == shiftSwapRequest.FromShift.StaffShiftID);

                            var swapRequest = new ShiftSwapRequest
                            {
                                FromShift = JsonConvert.SerializeObject(fromShiftSwap),
                                ToShift = JsonConvert.SerializeObject(toShiftSwap),
                                FromStaffID = shiftSwapRequest.FromStaffID,
                                ToStaffID = shiftSwapRequest.ToStaffID,
                                Status = Enums.ShiftSwapStatus.ShiftSwapRequested.ToString(),
                                LocationID = staffShift.ShiftConfig.LocationID,
                                DateRequested = DateUtil.Now()
                            };

                            context.ShiftSwapRequests.Add(swapRequest);
                            context.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                var token = StaffDL.GetStaffToken(shiftSwapRequest.ToStaffID);
                var msg = $"{shiftSwapRequest.FromShift.StaffName} has indicated an interest to your shift swap request. Login to the Wise1ne app to accept or decline.";
                PushNotification.Engine.SendMessage(token, msg, "Shift Swap", Enums.NotificationType.ShiftSwapRequest);

                return true;
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
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update Shift Swap Request to Cancelled
                            var shiftSwap = context.ShiftSwapRequests
                                                    .Where(s => s.ID == shiftSwapRequest.ID)
                                                    .FirstOrDefault();

                            if (shiftSwap.Status == Enums.ShiftSwapStatus.ShiftSwapRequested.ToString())
                            {
                                shiftSwap.Status = Enums.ShiftSwapStatus.AwaitingShiftSwapApproval.ToString();
                                context.Entry(shiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of From Shift request to Cancelled
                                var fromShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.FromShift.ID)
                                                    .FirstOrDefault();

                                fromShiftSwap.Status = Enums.ShiftSwapStatus.AwaitingShiftSwapApproval.ToString();
                                context.Entry(fromShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of To Shift request to Available
                                var toShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                    .FirstOrDefault();

                                toShiftSwap.Status = Enums.ShiftSwapStatus.AwaitingShiftSwapApproval.ToString();
                                context.Entry(toShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //add by msalem 
                                shiftSwapRequest.FromShift.Room= new ClassRoomDto()
                                {
                                    ID = shiftSwapRequest.FromShift.Room.ID,
                                    Name = shiftSwapRequest.FromShift.Room.Name
                                };
                                shiftSwapRequest.ToShift.Room= new ClassRoomDto()
                                {
                                    ID = shiftSwapRequest.ToShift.Room.ID,
                                    Name = shiftSwapRequest.ToShift.Room.Name
                                };
                                //Log shift swap for Approval
                                var approval = new Approval
                                {
                                    Obj = JsonConvert.SerializeObject(shiftSwapRequest,new JsonSerializerSettings()
                                    {
                                        NullValueHandling = NullValueHandling.Ignore,
                                        DefaultValueHandling = DefaultValueHandling.Ignore,
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                        ContractResolver = Utility.ShouldSerializeContractResolver.Instance,
                                    }),
                                    Type = Enums.ApprovalType.ShiftSwap.ToString(),
                                    Status = Enums.ApprovalStatus.Pending.ToString(),
                                    RequestedOn = DateUtil.Now()
                                };
                                context.Approvals.Add(approval);
                                context.SaveChanges();

                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                var errMsg = string.Format("Shift swap request is no longer available for acceptance. The current status of the shift swap is {0}. Kindly refresh your page to get the latest status.", Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.Status));

                                throw new Exception(errMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
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
                var staffName = string.Empty;

                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update Shift Swap Request to Cancelled
                            var shiftSwap = context.ShiftSwapRequests
                                                    .Where(s => s.ID == shiftSwapRequest.ID)
                                                    .FirstOrDefault();

                            if (shiftSwap.Status == Enums.ShiftSwapStatus.ShiftSwapRequested.ToString())
                            {
                                shiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapCancelled.ToString();
                                context.Entry(shiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of From Shift request to Cancelled
                                var fromShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.FromShift.ID)
                                                    .FirstOrDefault();

                                staffName = fromShiftSwap.StaffName;

                                fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapCancelled.ToString();
                                context.Entry(fromShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of To Shift request to Available
                                var toShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                    .FirstOrDefault();

                                toShiftSwap.Status = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString();
                                context.Entry(toShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                transaction.Commit();
                            }
                            else
                            {
                                var errMsg = string.Format("Shift swap request is no longer available for cancellation. The current status of the shift swap is {0}. Kindly refresh your page to get the latest status.", Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.Status));

                                throw new Exception(errMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                var token = StaffDL.GetStaffToken(shiftSwapRequest.ToStaffID);
                var msg = $"{staffName} is no longer interested in swapping shift with you. Your shift however is now available for swapping by any other interesed co-worker.";
                PushNotification.Engine.SendMessage(token, msg, "Shift Swap", Enums.NotificationType.ShiftSwapRequest);

                return true;
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
                var staffName = string.Empty;

                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update Shift Swap Request to Cancelled
                            var shiftSwap = context.ShiftSwapRequests
                                                    .Where(s => s.ID == shiftSwapRequest.ID)
                                                    .FirstOrDefault();

                            if (shiftSwap.Status == Enums.ShiftSwapStatus.ShiftSwapRequested.ToString())
                            {
                                shiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapDeclined.ToString();
                                context.Entry(shiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of From Shift request to Cancelled
                                var fromShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.FromShift.ID)
                                                    .FirstOrDefault();

                                fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapDeclined.ToString();
                                context.Entry(fromShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                //Update the Status of To Shift request to Available
                                var toShiftSwap = context.ShiftSwaps
                                                    .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                    .FirstOrDefault();

                                staffName = toShiftSwap.StaffName;

                                toShiftSwap.Status = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString();
                                context.Entry(toShiftSwap).State = EntityState.Modified;
                                context.SaveChanges();

                                transaction.Commit();
                            }
                            else
                            {
                                var errMsg = string.Format("Shift swap request is no longer available for cancellation. The current status of the shift swap is {0}. Kindly refresh your page to get the latest status.", Enums.GetDescription(typeof(Enums.ShiftSwapStatus), shiftSwap.Status));

                                throw new Exception(errMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                var token = StaffDL.GetStaffToken(shiftSwapRequest.FromStaffID);
                var msg = $"{staffName} has declined your shift swap.";
                PushNotification.Engine.SendMessage(token, msg, "Shift Swap", Enums.NotificationType.ShiftSwapRequest);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeleteShiftSwap(ShiftSwap shiftSwap)
        {
            try
            {
                var tokens = new List<string>();

                using (var context = new DataContext())
                {
                    var loggedForShiftSwap = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString();

                    var shift = context.ShiftSwaps
                                        .Where(ss => ss.StaffShiftID == shiftSwap.StaffShiftID &&
                                                    ss.StaffID == shiftSwap.StaffID &&
                                                    ss.Date == shiftSwap.Date &&
                                                    ss.Status == loggedForShiftSwap);

                    if (shift.Any())
                    {
                        var staffShift = shift.FirstOrDefault();
                        staffShift.Status = Enums.ShiftSwapStatus.ShiftDeletedFromSwapping.ToString();
                        context.Entry(staffShift).State = EntityState.Modified;
                        context.SaveChanges();

                        tokens = StaffDL.GetOtherStaffTokens(shiftSwap.StaffID, shiftSwap.LocationID);

                    }
                    else
                    {
                        var errMsg = string.Format("Your shift is no longer available to be deleted. You can however decline any swap request for the shift.");

                        throw new Exception(errMsg);
                    }
                }

                var msg = $"{shiftSwap.StaffName}'s shift on {shiftSwap.Date} is no longer available for shift swap.";
                PushNotification.Engine.SendMessage(tokens, msg, "Shift Swap", Enums.NotificationType.ShiftSwapAvailability);

                return true;
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
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update Shift Swap Request to Cancelled
                            var shiftSwap = context.ShiftSwapRequests
                                                    .Where(s => s.ID == shiftSwapRequest.ID)
                                                    .FirstOrDefault();

                            shiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapDeclined.ToString();
                            shiftSwap.DeclineReason = shiftSwapRequest.DeclineReason;
                            context.Entry(shiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Update the Status of From Shift request to Cancelled
                            var fromShiftSwap = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftSwapRequest.FromShift.ID)
                                                .FirstOrDefault();

                            fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapDeclined.ToString();
                            fromShiftSwap.DeclineReason = shiftSwapRequest.DeclineReason;

                            context.Entry(fromShiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Update the Status of To Shift request to Available
                            var toShiftSwap = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                .FirstOrDefault();

                            toShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapDeclined.ToString();
                            toShiftSwap.DeclineReason = shiftSwapRequest.DeclineReason;

                            context.Entry(toShiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Log shift swap for Approval
                            var approval = context.Approvals.Where(a => a.ID == shiftSwapRequest.ApprovalID).FirstOrDefault();
                            approval.DeclineReason = shiftSwapRequest.DeclineReason;
                            approval.Status = Enums.ApprovalStatus.Declined.ToString();
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                var staffIds = new List<long>()
                {
                    shiftSwapRequest.ToStaffID,
                    shiftSwapRequest.FromStaffID
                };
                var tokens = StaffDL.GetStaffTokens(staffIds);
                var msg = $"Your request to swap the shift on {shiftSwapRequest.FromShift.Date} has been declined. The reason is: {shiftSwapRequest.DeclineReason}";
                PushNotification.Engine.SendMessage(tokens, msg, "Shift Swap", Enums.NotificationType.ShiftSwapApproval);
                return true;
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
                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update Shift Swap Request to Cancelled
                            var shiftSwap = context.ShiftSwapRequests
                                                    .Where(s => s.ID == shiftSwapRequest.ID)
                                                    .FirstOrDefault();

                            shiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapApproved.ToString();
                            context.Entry(shiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Update the Status of From Shift request to Cancelled
                            var fromShiftSwap = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftSwapRequest.FromShift.ID)
                                                .FirstOrDefault();

                            fromShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapApproved.ToString();

                            context.Entry(fromShiftSwap).State = EntityState.Modified;
                            context.SaveChanges();

                            //Update the Status of To Shift request to Available
                            var toShiftSwap = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftSwapRequest.ToShift.ID)
                                                .FirstOrDefault();

                            toShiftSwap.Status = Enums.ShiftSwapStatus.ShiftSwapApproved.ToString();

                            context.Entry(toShiftSwap).State = EntityState.Modified;
                            context.SaveChanges();


                            //Swap Shifts
                            var fromStaffShift = context.StaffShifts
                                                       .Where(s => s.ID == fromShiftSwap.StaffShiftID)
                                                       .FirstOrDefault();
                            var fromShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(fromStaffShift.Shift);
                            var fromShift = (from shift in fromShifts
                                             where shift.Date == shiftSwapRequest.FromShift.Date
                                             select shift).FirstOrDefault();
                            var serFromShift = JsonConvert.SerializeObject(fromShift);


                            var toStaffShift = context.StaffShifts
                                                       .Where(s => s.ID == toShiftSwap.StaffShiftID)
                                                       .FirstOrDefault();
                            var toShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(toStaffShift.Shift);
                            var toShift = (from shift in toShifts
                                           where shift.Date == shiftSwapRequest.ToShift.Date
                                           select shift).FirstOrDefault();
                            var serToShift = JsonConvert.SerializeObject(toShift);

                            fromShifts.ForEach(shift =>
                            {
                                var s = JsonConvert.DeserializeObject<ShiftDto>(serToShift);
                                var sf = JsonConvert.DeserializeObject<ShiftDto>(serFromShift);

                                if (shift.Date == s.Date)
                                {
                                    shift.BreakTimeDuration = s.BreakTimeDuration;
                                    shift.Configure = s.Configure;
                                    shift.Date = s.Date;
                                    shift.Day = s.Day;
                                    shift.FolderTimeDuration = s.FolderTimeDuration;
                                    shift.From = s.From;
                                    shift.HasSupervision = s.HasSupervision;
                                    shift.Room = s.Room;
                                    shift.Status = s.Status;
                                    shift.To = s.To;
                                }

                                if (s.Date != sf.Date && shift.Date == sf.Date)
                                {
                                    shift.Configure = false;
                                    shift.Room = null;
                                    shift.From = 8;
                                    shift.To = 18;
                                }
                            });
                            fromStaffShift.Shift = JsonConvert.SerializeObject(fromShifts);
                            context.Entry(fromStaffShift).State = EntityState.Modified;
                            context.SaveChanges();

                            toShifts.ForEach(shift =>
                            {
                                var s = JsonConvert.DeserializeObject<ShiftDto>(serToShift);
                                var sf = JsonConvert.DeserializeObject<ShiftDto>(serFromShift);

                                if (shift.Date == sf.Date)
                                {
                                    shift.BreakTimeDuration = sf.BreakTimeDuration;
                                    shift.Configure = sf.Configure;
                                    shift.Date = sf.Date;
                                    shift.Day = sf.Day;
                                    shift.FolderTimeDuration = sf.FolderTimeDuration;
                                    shift.From = sf.From;
                                    shift.HasSupervision = sf.HasSupervision;
                                    shift.Room = sf.Room;
                                    shift.Status = sf.Status;
                                    shift.To = sf.To;
                                }

                                if (s.Date != sf.Date && shift.Date == s.Date)
                                {
                                    shift.Configure = false;
                                    shift.Room = null;
                                    shift.From = 8;
                                    shift.To = 18;
                                }

                            });
                            toStaffShift.Shift = JsonConvert.SerializeObject(toShifts);
                            context.Entry(toStaffShift).State = EntityState.Modified;
                            context.SaveChanges();

                            //Update shift swap Approval
                            var approval = context.Approvals.Where(a => a.ID == shiftSwapRequest.ApprovalID).FirstOrDefault();
                            approval.Status = Enums.ApprovalStatus.Approved.ToString();
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                var staffIds = new List<long>()
                {
                    shiftSwapRequest.ToStaffID,
                    shiftSwapRequest.FromStaffID
                };
                var tokens = StaffDL.GetStaffTokens(staffIds);
                var msg = $"Your request to swap the shift on {shiftSwapRequest.FromShift.Date} has been approved.";
                PushNotification.Engine.SendMessage(tokens, msg, "Shift Swap", Enums.NotificationType.ShiftSwapApproval);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwap> RetrieveAvailableShiftSwaps(ShiftSwap swapRequest)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var settings = RetrieveStaffShiftSettings(swapRequest.StaffID);
                    var possibleWeeks = (from setting in settings
                                         select setting.ID).ToList();

                    var available = Enums.ShiftSwapStatus.LoggedForShiftSwap.ToString();
                    var shiftSwaps = context.ShiftSwaps
                                        .Where(sc => sc.Status == available &&
                                                    sc.StaffUsername != swapRequest.StaffUsername &&
                                                    sc.Date == swapRequest.Date &&
                                                    sc.From != swapRequest.From &&
                                                    sc.ShiftConfigID == swapRequest.ShiftConfigID &&
                                                    possibleWeeks.Contains(sc.ShiftConfigID))
                                        .ToList();

                    return shiftSwaps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwap> RetrieveStaffShiftSwaps(string username)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var shiftSwaps = context.ShiftSwaps
                                        .Where(sc => sc.StaffUsername == username)
                                        .ToList();

                    return shiftSwaps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwap> RetrieveStaffShiftSwaps(long staffID, long staffShiftID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var shiftSwaps = context.ShiftSwaps
                                        .Where(sc => sc.StaffID == staffID && sc.StaffShiftID == staffShiftID)
                                        .ToList();

                    return shiftSwaps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ShiftSwapRequest> RetrieveShiftSwapRequests(long staffID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var shiftSwaps = context.ShiftSwapRequests
                                        .Where(sc => sc.FromStaffID == staffID || sc.ToStaffID == staffID)
                                        .OrderByDescending(sc => sc.ID)
                                        .Take(20)
                                        .ToList();

                    return shiftSwaps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region---------- Shift Over Time--------------
        public static bool ApproveShiftOverTime(ShiftSwapDto shiftOverTimeRequest)
        {
            try
            {
                var tokens = new List<string>();

                using (var context = new DataContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Update the Status of From Shift request to Cancelled
                            var shiftOverTime = context.ShiftSwaps
                                                .Where(ss => ss.ID == shiftOverTimeRequest.ID)
                                                .FirstOrDefault();

                            if (shiftOverTime != null)
                            {
                                shiftOverTime.Status = shiftOverTimeRequest.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString() ? Enums.ShiftSwapStatus.ShiftOverTimeApproved.ToString() : Enums.ShiftSwapStatus.ShiftOverTimeDeclined.ToString();

                                context.Entry(shiftOverTime).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            //Shift Overtime update
                            if (shiftOverTimeRequest.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString())
                            {
                                var staffShift = context.StaffShifts.FirstOrDefault(s => s.ID == shiftOverTimeRequest.StaffShiftID);
                                var staffShifts = JsonConvert.DeserializeObject<List<ShiftDto>>(staffShift.Shift);
                                staffShifts.ForEach(shift =>
                                {
                                    if (shift.Date == shiftOverTimeRequest.Date)
                                    {
                                        shift.OverTime = shiftOverTimeRequest.OverTime;
                                    }
                                });

                                staffShift.Shift = JsonConvert.SerializeObject(staffShifts);
                                context.Entry(staffShift).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            //Update shift swap Approval
                            var approval = context.Approvals.FirstOrDefault(a => a.ID == shiftOverTimeRequest.ApprovalID);
                            approval.Status = shiftOverTimeRequest.ApprovalStatus == Enums.ApprovalStatus.Approved.ToString() ? Enums.ApprovalStatus.Approved.ToString() : Enums.ApprovalStatus.Declined.ToString();
                            approval.ApprovedOn = DateUtil.Now();

                            context.Entry(approval).State = EntityState.Modified;
                            context.SaveChanges();

                            tokens = StaffDL.GetStaffToken(shiftOverTimeRequest.StaffID);


                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                PushNotification.Engine.SendMessage(tokens, $"Your shift over time request has been {shiftOverTimeRequest.ApprovalStatus.ToLower()}.", "Shift Over Time", Enums.NotificationType.ShiftOverTimeRequest);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
