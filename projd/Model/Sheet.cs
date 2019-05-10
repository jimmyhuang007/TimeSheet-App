using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projd.SheetModel
{
    public class Sheet
    {
        public int TimesheetID { get; set; }
        public int EmployeeID { get; set; }
        public int ManagerID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateApproved { get; set; }
        public decimal Day1 { get; set; }
        public decimal Day2 { get; set; }
        public decimal Day3 { get; set; }
        public decimal Day4 { get; set; }
        public decimal Day5 { get; set; }
        public decimal Day6 { get; set; }
        public decimal Day7 { get; set; }
        public decimal Overtime { get; set; }
        public int TStatus { get; set; }
        public string Comments { get; set; }
        public int EmployeeType { get; set; }
        public string jwt { get; set; }
    }
}
