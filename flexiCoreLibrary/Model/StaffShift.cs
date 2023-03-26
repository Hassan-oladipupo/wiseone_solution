using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class StaffShift
    {
        public StaffShift()
        {            
            this.SignInOuts = new HashSet<SignInOut>();
        }

        public long ID { get; set; }
        public long StaffID { get; set; }
        public long ShiftConfigID { get; set; }
        //Serialized ShiftDto Data        
        public string Shift { get; set; }

        [ForeignKey("ShiftConfigID")]
        public virtual ShiftConfig ShiftConfig { get; set; }
        public virtual ICollection<SignInOut> SignInOuts { get; set; }
    }
}
