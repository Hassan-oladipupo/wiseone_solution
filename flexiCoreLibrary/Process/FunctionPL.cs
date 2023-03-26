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
    public class FunctionPL
    {
        public static bool Save(FunctionDto functionDto)
        {
            try
            {
                var function = new Function
                {
                    Name = functionDto.Name,
                    Type = functionDto.Type,
                    Module = functionDto.Module,
                    PageUrl = functionDto.PageUrl
                };

                if (FunctionDL.FunctionExists(function))
                {
                    throw new Exception(string.Format("Function with name: {0} exists already", function.Name));
                }
                else
                {
                    return FunctionDL.Save(function);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(FunctionDto functionDto)
        {
            try
            {
                var function = new Function
                {
                    ID = functionDto.ID,
                    Name = functionDto.Name,
                    Type = functionDto.Type,
                    Module = functionDto.Module,
                    PageUrl = functionDto.PageUrl
                };

                if (FunctionDL.FunctionExists(function))
                {
                    throw new Exception(string.Format("Function with name: {0} exists already", function.Name));
                }
                else
                {
                    return FunctionDL.Update(function);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FunctionDto> RetrieveFunctions()
        {
            try
            {
                var functions = FunctionDL.RetrieveFunctions();

                var returnedFunctions = (from function in functions
                                         select new FunctionDto
                                         {
                                             ID = function.ID,
                                             Name = function.Name,
                                             Type = function.Type,
                                             Module = function.Module,
                                             PageUrl = function.PageUrl
                                         })
                                         .ToList();

                return returnedFunctions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static FunctionDto RetrieveFunctionById(long id)
        {
            try
            {
                var function = FunctionDL.RetrieveFunctionByID(id);

                var returnedFunction = new FunctionDto
                {
                    ID = function.ID,
                    Name = function.Name,
                    Type = function.Type,
                    Module = function.Module,
                    PageUrl = function.PageUrl
                };

                return returnedFunction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
