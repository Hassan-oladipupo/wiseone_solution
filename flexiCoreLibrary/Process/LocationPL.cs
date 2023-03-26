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
    public class LocationPL
    {
        public static bool Save(LocationDto locationDto)
        {
            try
            {
                var location = new Location
                {                    
                    Address = locationDto.Address,
                    Latitude = locationDto.Latitude,
                    Longitude = locationDto.Longitude,
                    Name = locationDto.Name,
                    HeadOffice = locationDto.HeadOffice,
                    OperationOffice = locationDto.OperationOffice,
                    CreatedOn = DateUtil.Now(),
                    Status = locationDto.Status
                };

                if (LocationDL.LocationExists(location))
                {
                    throw new Exception(string.Format("Location with name: {0} exists already", location.Name));
                }
                else
                {
                    return LocationDL.Save(location);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(LocationDto locationDto)
        {
            try
            {
                var location = new Location
                {
                    ID = locationDto.ID,
                    Address = locationDto.Address,
                    Latitude = locationDto.Latitude,
                    Longitude = locationDto.Longitude,
                    HeadOffice = locationDto.HeadOffice,
                    OperationOffice = locationDto.OperationOffice,
                    Name = locationDto.Name                    
                };

                if (LocationDL.LocationExists(location))
                {
                    throw new Exception(string.Format("Location with name: {0} exists already", location.Name));
                }
                else
                {
                    return LocationDL.Update(location);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool EnableOrDisable(LocationDto locationDto)
        {
            try
            {
                var location = new Location
                {
                    ID = locationDto.ID,
                    Status = locationDto.Status
                };

                return LocationDL.EnableOrDisable(location);
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
                var locations = LocationDL.RetrieveLocations();                
                return locations;
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
                var locations = LocationDL.RetrieveActiveLocations();               
                return locations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
