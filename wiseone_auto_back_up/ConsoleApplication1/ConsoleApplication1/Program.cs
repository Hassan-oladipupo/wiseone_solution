using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Wise1ne DB Back Up Started");

                var db = ConfigurationManager.AppSettings.Get("Wise1neDb");
                var dbName = ConfigurationManager.AppSettings.Get("Wise1neDbName");
                var dbUsername = ConfigurationManager.AppSettings.Get("Wise1neDbUsername");
                var dbPassword = ConfigurationManager.AppSettings.Get("Wise1neDbPassword");
                var dbBackUpPath = ConfigurationManager.AppSettings.Get("Wise1neDbBackUpPath");
                var date = $"{DateTime.Now.Day.ToString()}-{DateTime.Now.Month.ToString()}-{DateTime.Now.Year.ToString()}";
                string dbBackUpDirectory = $"{dbBackUpPath}{date}";

                if (!Directory.Exists(dbBackUpDirectory))
                {
                    Directory.CreateDirectory(dbBackUpDirectory);

                    StringBuilder sb = new StringBuilder();
                    Server srv = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(db, dbUsername, dbPassword));
                    Database dbs = srv.Databases[dbName];
                    ScriptingOptions options = new ScriptingOptions()
                    {
                        ScriptData = true,
                        ScriptDrops = false,
                        EnforceScriptingOptions = true,
                        ScriptSchema = true,
                        IncludeHeaders = true,
                        AppendToFile = false,
                        Indexes = true,
                        WithDependencies = false
                    };

                    foreach (Table table in dbs.Tables)
                    {
                        options.FileName = $"{dbBackUpDirectory}\\{table.Name}.sql";
                        table.EnumScript(options);
                        Console.WriteLine($"{table.Name} script generated");
                    }
                }               

                Console.WriteLine("Wise1ne DB Back Up Completed");
                Console.Read();
            }            
            catch(Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }
    }
}
