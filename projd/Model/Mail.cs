using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projd.MailModel
{
    public class Mail
    {
        public int EmployeeID { get; set; }
        public string EmailAddress { get; set; }
        public string EmployeeMail { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
    }
}
