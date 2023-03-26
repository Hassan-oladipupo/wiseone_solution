using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class FunctionDL
    {
        public static bool Save(Function function)
        {
            try
            {
                using (var context = new DataContext())
                {
                    context.Functions.Add(function);
                    var record = context.SaveChanges();
                    return record > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(Function function)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var existingfunction = new Function();

                    existingfunction = context.Functions
                                    .Where(t => t.ID == function.ID)
                                    .FirstOrDefault();

                    if (existingfunction != null)
                    {
                        existingfunction.Name = function.Name;
                        existingfunction.Type = function.Type;
                        existingfunction.PageUrl = function.PageUrl;
                        existingfunction.Module = function.Module;

                        context.Entry(existingfunction).State = EntityState.Modified;
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

        public static bool FunctionExists(Function function)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var functions = context.Functions
                                    .Where(t => t.Name.Equals(function.Name));

                    if (functions.Any())
                    {
                        var existingFunction = functions.FirstOrDefault();

                        if (existingFunction.ID == function.ID)
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

        public static List<Function> RetrieveFunctions()
        {
            try
            {
                using (var context = new DataContext())
                {
                    var functions = context.Functions.OrderBy(f => f.Module).ThenBy(f => f.Name).ToList();
                    return functions;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Function RetrieveFunctionByID(long functionID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var functions = context.Functions
                                            .Where(f => f.ID == functionID);

                    return functions.Any() ? functions.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
