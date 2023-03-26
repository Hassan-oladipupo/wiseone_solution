using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class FinancialYearMonthDay
    {        
        public long ID { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        public int Day { get; set; }        
        public bool Available { get; set; }
        public bool BankHoliday { get; set; }
        public int NumberOfStaff { get; set; }

        //A json array of staff on holiday for this day      
        public string Staff { get; set; }     
        public long FinancialYearMonthID { get; set; }

        [ForeignKey("FinancialYearMonthID")]
        public virtual FinancialYearMonth FinancialYearMonth { get; set; }        
    }
}
