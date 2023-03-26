using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class LocationDL
    {
        public static bool Save(Location location)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.Locations.Add(location);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(Location location)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existinglocation = new Location();

                    existinglocation = context.Locations
                                    .Where(t => t.ID == location.ID)
                                    .FirstOrDefault();

                    if (existinglocation != null)
                    {
                        existinglocation.Name = location.Name;
                        existinglocation.Address = location.Address;
                        existinglocation.Latitude = location.Latitude;
                        existinglocation.Longitude = location.Longitude;
                        existinglocation.HeadOffice = location.HeadOffice;
                        existinglocation.OperationOffice = location.OperationOffice;
                        existinglocation.LastUpdatedOn = DateUtil.Now();

                        context.Entry(existinglocation).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
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

        public static bool EnableOrDisable(Location location)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existinglocation = new Location();

                    existinglocation = context.Locations
                                    .Where(t => t.ID == location.ID)
                                    .FirstOrDefault();

                    if (existinglocation != null)
                    {
                        existinglocation.Status = location.Status;
                        existinglocation.LastUpdatedOn = DateUtil.Now();

                        context.Entry(existinglocation).State = EntityState.Modified;
                        var record = context.SaveChanges();
                        return record > 0 ? true : false;
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

        public static bool LocationExists(Location location)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var locations = context.Locations
                                    .Where(t => t.Name.Equals(location.Name));

                    if (locations.Any())
                    {
                        var existingLocation = locations.FirstOrDefault();

                        if (existingLocation.ID == location.ID)
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

        public static List<LocationDto> RetrieveLocations()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var locations = context.Locations
                                             .AsEnumerable()
                                            .Select((location) => new LocationDto()
                                            {
                                                ID = location.ID,
                                                Name = location.Name,
                                                Address = location.Address,
                                                Status = location.Status,
                                                Longitude = location.Longitude,
                                                Latitude = location.Latitude,
                                                HeadOffice = location.HeadOffice,
                                                OperationOffice = location.OperationOffice,
                                                CreatedOn = string.Format("{0:g}", location.CreatedOn.Value),
                                                LastUpdatedOn = location.LastUpdatedOn != null ? string.Format("{0:g}", location.LastUpdatedOn.Value) : string.Empty
                                            })
                                            .ToList();
                    return locations;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Location RetrieveHeadOfficeLocation()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var location = context.Locations.Where(l => l.HeadOffice.Equals(true)).FirstOrDefault();
                    return location;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Location RetrieveOperationOfficeLocation()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var location = context.Locations.Where(l => l.OperationOffice.Equals(true)).FirstOrDefault();
                    return location;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Location RetrieveLocationByID(long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var location = context.Locations.FirstOrDefault(l => l.ID == locationID);
                    return location;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Location RetrieveShiftLocationByID(long staffShiftID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var staffShift = context.StaffShifts.Include(s => s.ShiftConfig).FirstOrDefault(s => s.ID == staffShiftID);
                    var location = context.Locations.FirstOrDefault(l => l.ID == staffShift.ShiftConfig.LocationID);
                    return location;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<LocationDto> RetrieveActiveLocations()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var locations = context.Locations
                                            .Where(l => l.Status == "Active")
                                            .AsEnumerable()
                                            .Select((location) => new LocationDto()
                                            {
                                                ID = location.ID,
                                                Name = location.Name,
                                                Address = location.Address,
                                                Status = location.Status,
                                                Longitude = location.Longitude,
                                                Latitude = location.Latitude,
                                                HeadOffice = location.HeadOffice,
                                                OperationOffice = location.OperationOffice,
                                                CreatedOn = string.Format("{0:g}", location.CreatedOn.Value),
                                                LastUpdatedOn = location.LastUpdatedOn != null ? string.Format("{0:g}", location.LastUpdatedOn.Value) : string.Empty
                                            })
                                            .ToList();
                    return locations;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
