using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class RoleDL
    {
        public static bool Save(Role role)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.Roles.Add(role);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(Role role)
        {
            try
            {
                var existingRole = new Role();
                using (var context = new DataContext())
                {
                    existingRole = context.Roles
                                    .Where(t => t.ID == role.ID)
                                    .FirstOrDefault();

                    if (existingRole != null)
                    {
                        existingRole.Name = role.Name;
                        existingRole.Type = role.Type;
                        existingRole.CanAccessWeb = role.CanAccessWeb;
                        existingRole.LastUpdatedOn = DateUtil.Now();

                        //Transaction block
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //Modifying just the property details
                                context.Entry(existingRole).State = EntityState.Modified;
                                context.SaveChanges();

                                //Delete existing role function of the role
                                var existingRoleFunctions = context.RoleFunctions.Include("Role")
                                                                 .Where(t => existingRole.ID.Equals(t.RoleID))
                                                                 .ToList();

                                if (existingRoleFunctions != null && existingRoleFunctions.ToList().Count != 0)
                                {
                                    context.RoleFunctions.RemoveRange(existingRoleFunctions);
                                    context.SaveChanges();
                                }

                                //Adding new Role Functions
                                var newRoleFunctions = (from function in role.RoleFunctions
                                                        select new RoleFunction
                                                        {
                                                            RoleID = existingRole.ID,
                                                            FunctionID = function.FunctionID
                                                        }).ToList();

                                if (newRoleFunctions.Any())
                                {
                                    context.RoleFunctions.AddRange(newRoleFunctions);
                                    context.SaveChanges();
                                }

                                //commit changes
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }

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

        public static bool EnableOrDisable(Role role)
        {
            try
            {
                var existingRole = new Role();
                using (var context = new DataContext())
                {
                    existingRole = context.Roles
                                    .Where(t => t.ID == role.ID)
                                    .FirstOrDefault();

                    if (existingRole != null)
                    {
                        existingRole.LastUpdatedOn = DateUtil.Now();
                        existingRole.Status = role.Status;

                        context.Entry(existingRole).State = EntityState.Modified;
                        context.SaveChanges();

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

        public static bool RoleExists(Role role)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var roles = context.Roles
                                    .Where(t => t.Name.Equals(role.Name));

                    if (roles.Any())
                    {
                        var existingRole = roles.FirstOrDefault();

                        if (existingRole.ID == role.ID)
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

        public static List<Role> RetrieveRoles()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var roles = context.Roles
                                .Include(r => r.RoleFunctions.Select(rf => rf.Function))
                                .ToList();

                    return roles;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Role> RetrieveActiveRoles()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var roles = context.Roles
                                .Include(r => r.RoleFunctions.Select(rf => rf.Function))
                                .Where(r => r.Status == "Active")
                                .ToList();

                    return roles;
                }
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
                using (var context = new DataContext())
                {
                    var role = context.Roles
                                .Where(r => r.ID == roleID)
                                .Include(r => r.RoleFunctions.Select(rf => rf.Function))
                                .ToList().FirstOrDefault();

                    return role;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
