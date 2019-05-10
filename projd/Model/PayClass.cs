using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projd.Model
{
    public class PayClass
    {
        public int EmployeeID { get; set; }
        public string StudentF { get; set; }
        public string StudentL { get; set; }
        public string Manager { get; set; }
        public decimal Regular { get; set; }
        public decimal Overtime { get; set; }
        public decimal Total { get; set; }
    }
}
