using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace flexiCoreLibrary.Utility
{
    public static class Extentions
    {
        public static string GetPicture(this string picture)
        {
            try
            {
                var imagePath = picture;
                if (picture.Contains("Resources/Images")==false)
                {
                    imagePath = $"Resources/Images/{picture}";
                }

                var path = HttpContext.Current.Server.MapPath($"~/{imagePath}");
                byte[] imageArray = System.IO.File.ReadAllBytes(path);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return "data:image/jpeg;base64," + base64ImageRepresentation;
            }
            catch (Exception e)
            {
                return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAACK1BMVEX////9+Pr99vj//f788/Xsu8nvxtH9+fr99/jor7/OVHbkoLP99/n+/f3uw8/KRmvCK1Xima356u7QWny/HkvBJlLfkKb88/b//v7AIU2/H0zAI0/cg5v67vH46OzKRWq+HUu/IE3WcIz45erqtMPAIk7PWXr02uH99Pf41uD64ejkobS/HUvJQWfvx9P41N7vm7LmX4b0uMny0drPWHm/IU3DLVfmqrv++fr51+Hvm7PmXYXfMmPgNmf3zdn++vvimq7RYIDCKVTbgpv78PP56e7+/P364Ofwn7fmX4XgN2fdKVzfNWbqepr97/L++/zVbYrAJFDAIU734ObkorXps8L30NvmYoffNmbdJlrgNmbqeZn52OLcg5y+G0nELljXdZDHOGDfXoPdKV3dJ1vdKFzjUXv76O3nrL3EMlvAJVHFM1zMNF/ZJlnfNGXysMP12uHFNF2+HUrEL1jGkrq4gLTVM2HhPWzvmrP9+PnUZ4bSW3vS0fCdis/TQm7eM2TlW4Lyqr798fTsu8jOU3bHO2Lty9jXqsbbY4bulq/40t3++PnchZ3MS2/BJFDXc4/uxNDyytX12uLgkafJQ2nDL1nlo7bgk6jDMFm/HkzOUXTz09z77/LjnbHMTXHAIk/SYIDpsMDqtcTNUHO+HkvGOWDpssLUZoXHPWTXcY7z19713ePLSW7HPGPy09z78fTpssDqt8Xy0tvimKz019/89/j78vT+/Pz9+vpYAAWQAAAAAWJLR0QAiAUdSAAAAAd0SU1FB+QGFhcIKSgxqUYAAAFkSURBVDjLY2CgK2BkYsYrz8LKxo5PnoOTi5sHjzwvH7+AIBMeBULCIqJi4rjlJQQlpaRlZHErkJNXkFJUUkbSgeYjFVUpKRE1dThfQ1MLVYG2opSUjq4eVBuTvoGhEaoCYxNTKREzc3BAWFhaWdvY2qHI2zs4OklJSTq7uFq4uXt4enn7+PqhKPAPCAwCOoIrOCQ0LDzCOzIqOkYC1YrYOCmggnjOhMSk5JSUlNS0dDRvZgAdKSWlmJmVnZMLVJCXjx4OBYUiIBVFxSWlZZHe5RWV6Aoqq4pAKoqqa2rr6hsamzCDsrmlUDRISqS1rb2js6sbS1gzGWf09PZJSfVPmKghwYAN+E+aPEVVSmTqtEqs0gxC07lmAONjptSs2dgVzJk7D+jK+aJFqgsWCmFVwbNocdaSpcsclq9YucoNqwq31WJr1q5jWL9hoywvdlsYGDdt3sIgsXXbdgacYAfehD9QAACuO1EnvlYFpwAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyMC0wNi0yMlQyMzowODo0MSswMDowMK+SNZwAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjAtMDYtMjJUMjM6MDg6NDErMDA6MDDez40gAAAAAElFTkSuQmCC";
            }
        }

         public static string SavePicture(this string picture,string username)
        {
            try
            {
                var base64imageString = picture.Replace("data:image/jpeg;base64,", "");
                var imageName= $"profile_{username}_.jpg";
                var path = HttpContext.Current.Server.MapPath($"~/Resources/Images/{imageName}");
                File.WriteAllBytes(path, Convert.FromBase64String(base64imageString));
                return imageName;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
