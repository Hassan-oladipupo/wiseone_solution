using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class FinancialYearMonth
    {
        public FinancialYearMonth()
        {
            this.FinancialYearMonthDays = new HashSet<FinancialYearMonthDay>();
        }

        public long ID { get; set; }
        [MaxLength(50)]
        public string Label { get; set; }
        [MaxLength(50)]
        public string Month { get; set; }
        public long Year { get; set; }
        public bool Exclude { get; set; }            
        public long FinancialYearID { get; set; }
           
        [ForeignKey("FinancialYearID")]
        public virtual FinancialYear FinancialYear { get; set; }
        public virtual ICollection<FinancialYearMonthDay> FinancialYearMonthDays { get; set; }
    }
}
