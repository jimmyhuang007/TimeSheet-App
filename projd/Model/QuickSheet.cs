using projd.SheetModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projd.QuickSheetModel
{
    public class QuickSheet
    {
        public int Sheet1ID { get; set; }
        public int Sheet2ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int T1Status { get; set; }
        public int T2Status { get; set; }
        public Sheet Timesheet1 { get; set; }
        public Sheet Timesheet2 { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int EmployeeID { get; set; }
        public int EmployeeType { get; set; }
        public string Comments { get; set; }
        public string jwt { get; set; }
        public int ManagerID { get; set; }
    }
}
