using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projd.UserModel
{
    public class User
    {
        public int EmployeeID { get; set; } //vital
        public string LoginID { get; set; }
        public string PasswordID { get; set; }
        public int EmployeeType { get; set; } //vital
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public string PositionTitle { get; set; }
        public int ManagerID { get; set; }
        public User AdminUse { get; set; }
        public string jwt { get; set; }
        public string Newpassword { get; set; }
}
}
