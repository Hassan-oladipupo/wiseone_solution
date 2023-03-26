using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Process;

namespace flexiFixImages
{
    class Program
    {
        static void Main(string[] args)
        {
            // read all 
            // save images 
            // done LOL
            var staffs = StaffPL.RetrieveStaffs();
            var debugDir = Environment.CurrentDirectory;

            
            foreach (StaffDto staff in staffs)
            {
                string filePath = debugDir+"\\images\\profile_"+staff.ID+"_.jpg";
                File.WriteAllBytes(filePath, Convert.FromBase64String(staff.Picture.Replace("data:image/jpeg;base64,","")));

                //staff.Picture
            }


        }
    }
}
