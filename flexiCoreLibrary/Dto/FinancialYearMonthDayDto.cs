using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class FinancialYearMonthDayDto
    {
        public long ID { get; set; }        
        public string Name { get; set; }
        public int Day { get; set; }
        public bool Available { get; set; }
        public bool BankHoliday { get; set; }
        public int NumberOfStaff { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        //A json array of staff on holiday for this day      
        public List<StaffLeaveDto> Staff { get; set; }
        public long FinancialYearMonthID { get; set; }        
        public FinancialYearMonthDto FinancialYearMonth { get; set; }
    }
}
