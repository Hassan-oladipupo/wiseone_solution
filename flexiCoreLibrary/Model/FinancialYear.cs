using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class FinancialYear
    {
        public FinancialYear()
        {
            this.FinancialYearMonths = new HashSet<FinancialYearMonth>();
        }

        public long ID { get; set; }
        public long LocationID { get; set; }
        [MaxLength(50)]
        public string StartDate { get; set; }
        [MaxLength(50)]
        public string EndDate { get; set; }        
        public long LeavePriorNotice { get; set; }
        [MaxLength(250)]
        public string ExcludeMonths { get; set; }
        [MaxLength(20)]        
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<FinancialYearMonth> FinancialYearMonths { get; set; }
    }
}
