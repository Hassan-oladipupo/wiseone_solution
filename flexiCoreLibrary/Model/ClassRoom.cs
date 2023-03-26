using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class ClassRoom
    {
        public ClassRoom()
        {
            this.SignInOuts = new HashSet<SignInOut>();            
        }

        public long ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public long LocationID { get; set; }        
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [ForeignKey("LocationID")]
        public virtual Location Location { get; set; }
        public virtual ICollection<SignInOut> SignInOuts { get; set; }
    }
}
