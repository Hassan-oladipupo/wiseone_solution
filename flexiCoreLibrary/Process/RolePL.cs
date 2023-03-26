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
    public class RolePL
    {
        public static bool Save(RoleDto roleDto)
        {
            try
            {
                var role = new Role
                {
                    Name = roleDto.Name,
                    CanAccessWeb = roleDto.CanAccessWeb,
                    Type = roleDto.Type,
                    RoleFunctions = (from function in roleDto.Functions
                                     select new RoleFunction()
                                     {
                                         FunctionID = function
                                     }).ToList(),
                    CreatedOn = DateUtil.Now(),
                    Status = roleDto.Status
                };

                if (RoleDL.RoleExists(role))
                {
                    throw new Exception(string.Format("Role with name: {0} exists already", role.Name));
                }
                else
                {
                    return RoleDL.Save(role);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(RoleDto roleDto)
        {
            try
            {
                var role = new Role
                {
                    ID = roleDto.ID,
                    Name = roleDto.Name,
                    Type = roleDto.Type,
                    CanAccessWeb = roleDto.CanAccessWeb,
                    RoleFunctions = (from function in roleDto.Functions
                                     select new RoleFunction()
                                     {
                                         FunctionID = function
                                     }).ToList()
                };

                if (RoleDL.RoleExists(role))
                {
                    throw new Exception(string.Format("Role with name: {0} exists already", role.Name));
                }
                else
                {
                    return RoleDL.Update(role);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool EnableOrDisable(RoleDto roleDto)
        {
            try
            {
                var role = new Role
                {
                    ID = roleDto.ID,
                    Status = roleDto.Status
                };

                return RoleDL.EnableOrDisable(role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<RoleDto> RetrieveRoles()
        {
            try
            {
                var roles = RoleDL.RetrieveRoles();

                var returnedRoles = (from role in roles
                                     select new RoleDto
                                     {
                                         ID = role.ID,
                                         Name = role.Name,
                                         CanAccessWeb = role.CanAccessWeb,
                                         Type = role.Type,
                                         Status = role.Status,
                                         CreatedOn = string.Format("{0:g}", role.CreatedOn),
                                         LastUpdatedOn = role.LastUpdatedOn != null ? string.Format("{0:g}", role.LastUpdatedOn) : string.Empty,
                                         Functions = (from roleFunction in role.RoleFunctions
                                                      select roleFunction.FunctionID).ToList(),
                                         FunctionNames = (from roleFunction in role.RoleFunctions
                                                          select roleFunction.Function.Name).ToList()
                                     }).ToList();

                return returnedRoles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<RoleDto> RetrieveActiveRoles()
        {
            try
            {
                var roles = RoleDL.RetrieveActiveRoles();

                var returnedRoles = (from role in roles
                                     select new RoleDto
                                     {
                                         ID = role.ID,
                                         Name = role.Name,
                                         Type = role.Type,
                                         CanAccessWeb = role.CanAccessWeb,
                                         Status = role.Status,
                                         CreatedOn = string.Format("{0:g}", role.CreatedOn),
                                         LastUpdatedOn = role.LastUpdatedOn != null ? string.Format("{0:g}", role.LastUpdatedOn) : string.Empty,
                                         Functions = (from roleFunction in role.RoleFunctions
                                                      select roleFunction.FunctionID).ToList(),
                                         FunctionNames = (from roleFunction in role.RoleFunctions
                                                          select roleFunction.Function.Name).ToList()
                                     }).ToList();

                return returnedRoles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Role RetrieveRoleByID(long? roleID)
        {
            try
            {
                return RoleDL.RetrieveRoleByID(roleID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
