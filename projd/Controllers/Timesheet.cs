using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projd.SheetModel;
using projd.QuickSheetModel;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using projd.UserModel;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projd.Controllers
{
    [Route("api/[controller]")]
    public class Timesheet : Controller
    {
        public static string SQLConnection = "Data Source=LAPTOP-N4D761U2\\JIMSQL;Initial Catalog=Main;Integrated Security=True";

        public bool Validatejwt(string token, string userid, string usertype)
        {
            string key = "hometrustisthebestcompany";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = signingKey
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            var decode = new JwtSecurityToken(token);
            return (decode.Claims.First().Value == userid) && (decode.Claims.Last().Value == usertype);
        }

        [HttpPost]
        [Route("initial")]
        //[Route("User")] user initalization only, manager should use another get
        // no jwt
        public ActionResult<QuickSheet[]> Get([FromBody]User client)
        {
            try
            {
                //get calender weeks 
                CultureInfo myCI = new CultureInfo("en-US");
                Calendar myCal = myCI.Calendar;
                CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
                DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
                int week = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW); // # of weeks since year start

                // first sunday of the biweekly, occurs when week number is even
                DateTime biweekstart = (week % 2 == 0) ? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) : DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
                DateTime packagestart = biweekstart.AddDays(-56); //first sunday of package
                DateTime packageend = biweekstart.AddDays(7); //Last Sunday of package
                //get a list of datetime already in database
                SqlConnection connection = new SqlConnection(SQLConnection);
                List<DateTime> sqlDatelst = new List<DateTime>();
                SqlCommand getweeks = new SqlCommand("TimesheetInitQuery", connection);
                getweeks.Parameters.AddWithValue("@WeeksStart", packagestart);
                getweeks.Parameters.AddWithValue("@WeeksEnd", packageend);
                getweeks.Parameters.AddWithValue("@EmployeeID", client.EmployeeID);
                getweeks.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader readweeks = getweeks.ExecuteReader();
                while (readweeks.Read())
                {
                    sqlDatelst.Add(readweeks.GetDateTime(0));
                }
                connection.Close();
                //filter those date that alrady exist
                List<DateTime> tracking = new List<DateTime>();
                if (sqlDatelst != null)
                    while (packageend.Date >= packagestart.Date)
                    {
                        if (sqlDatelst.Any() && packageend.Date == sqlDatelst.First().Date)
                        {
                            sqlDatelst.Remove(sqlDatelst.First());
                        }
                        else
                        {
                            tracking.Add(packageend);
                        }
                        packageend = packageend.AddDays(-7);
                    }

                //create all the nessary timesheets
                SqlCommand initnewtimesheet = new SqlCommand("TimesheetInitCreate", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                initnewtimesheet.Parameters.AddWithValue("@EmployeeID", client.EmployeeID);
                initnewtimesheet.Parameters.AddWithValue("@ManagerID", client.ManagerID);
                initnewtimesheet.Parameters.AddWithValue("@day1", 0);
                initnewtimesheet.Parameters.AddWithValue("@day2", 0);
                initnewtimesheet.Parameters.AddWithValue("@day3", 0);
                initnewtimesheet.Parameters.AddWithValue("@day4", 0);
                initnewtimesheet.Parameters.AddWithValue("@day5", 0);
                initnewtimesheet.Parameters.AddWithValue("@day6", 0);
                initnewtimesheet.Parameters.AddWithValue("@day7", 0);
                initnewtimesheet.Parameters.AddWithValue("@Overtime", 0);
                initnewtimesheet.Parameters.AddWithValue("Tstatus", 0);
                initnewtimesheet.Parameters.Add("@StartDate", SqlDbType.Date);
                initnewtimesheet.Parameters.Add("@EndDate", SqlDbType.Date);
                foreach (var date in tracking)
                {
                    connection.Open();
                    initnewtimesheet.Parameters["@StartDate"].Value = date;
                    initnewtimesheet.Parameters["@EndDate"].Value = date.AddDays(6);
                    SqlDataReader createtimesheet = initnewtimesheet.ExecuteReader();
                    connection.Close();
                }
                //intalize the return pacakge
                QuickSheet[] package = new QuickSheet[5];
                //now return with full package with timesheet id, dates, and status
                SqlCommand gettimesheet = new SqlCommand("TimesheetsGetQuick", connection);
                gettimesheet.Parameters.AddWithValue("@EmployeeID", client.EmployeeID);
                gettimesheet.Parameters.AddWithValue("@WeeksStart", packagestart);
                gettimesheet.Parameters.AddWithValue("@WeeksEnd", biweekstart.AddDays(7));
                gettimesheet.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader sheetsreader = gettimesheet.ExecuteReader();
                int i = 0; //index to iterate through package array
                Boolean t2 = true; // boolearn for timesheet 1 or 2, since sql is from latest to earliest, week2 will appear first
                while (sheetsreader.Read())
                {
                    if (t2)
                    {
                        package[i] = new QuickSheet
                        {
                            Sheet2ID = sheetsreader.GetInt32(0),
                            EndDate = sheetsreader.GetDateTime(2),
                            T2Status = sheetsreader.GetByte(3)
                        };
                        t2 = false;
                    }
                    else
                    {
                        package[i].Sheet1ID = sheetsreader.GetInt32(0);
                        package[i].StartDate = sheetsreader.GetDateTime(1);
                        package[i].T1Status = sheetsreader.GetByte(3);
                        t2 = true;
                        i++;
                    }
                }
                connection.Close();
                return Ok(package);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("manager")]
        public ActionResult<QuickSheet[]> Manager([FromBody]User client)
        {
            try
            {
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand manager = new SqlCommand("TimesheetManager", connection);
                manager.Parameters.AddWithValue("@ManagerID", client.EmployeeID);
                manager.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader managerreader = manager.ExecuteReader();
                QuickSheet[] package = new QuickSheet[5];

                int i = 0;
                bool t2 = true;
                while (managerreader.Read() && (i < 5))
                {
                    if (t2)
                    {
                        package[i] = new QuickSheet
                        {
                            Firstname = managerreader.GetString(0),
                            Lastname = managerreader.GetString(1),
                            T2Status = managerreader.GetByte(5),
                            Sheet2ID = managerreader.GetInt32(2),
                            EndDate = managerreader.GetDateTime(4),
                        };
                        t2 = false;
                    }
                    else
                    {
                        package[i].StartDate = managerreader.GetDateTime(3);
                        package[i].Sheet1ID = managerreader.GetInt32(2);
                        package[i].T1Status = managerreader.GetByte(5);
                        t2 = true;
                        i++;
                    }
                }
                connection.Close();
                Array.Resize(ref package, i);
                return Ok(package);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("weekly")]
        public ActionResult<Sheet> Biweekly([FromBody]Sheet asheet)
        {
            try
            {
                // just checks if employeeID has been amended, API dosen't need to be restricted by UserType as its a simple get
                if (!(Validatejwt(asheet.jwt, asheet.EmployeeID.ToString(), asheet.EmployeeType.ToString())))
                {
                    return Unauthorized();
                }
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand getweeklydata = new SqlCommand("TimesheetGet", connection);
                getweeklydata.Parameters.AddWithValue("@TimesheetID", asheet.TimesheetID);
                getweeklydata.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader sheetreader = getweeklydata.ExecuteReader();
                Sheet returnsheet = new Sheet();
                if (sheetreader.Read())
                {
                    returnsheet.TimesheetID = sheetreader.GetInt32(0);
                    returnsheet.EmployeeID = sheetreader.GetInt32(1);
                    returnsheet.ManagerID = sheetreader.GetInt32(2);
                    returnsheet.StartDate = sheetreader.GetDateTime(3);
                    returnsheet.EndDate = sheetreader.GetDateTime(4);
                    returnsheet.DateApproved = sheetreader.IsDBNull(5) ? DateTime.MinValue : sheetreader.GetDateTime(5);
                    returnsheet.Day1 = sheetreader.GetSqlDecimal(6).Value;
                    returnsheet.Day2 = sheetreader.GetSqlDecimal(7).Value;
                    returnsheet.Day3 = sheetreader.GetSqlDecimal(8).Value;
                    returnsheet.Day4 = sheetreader.GetSqlDecimal(9).Value;
                    returnsheet.Day5 = sheetreader.GetSqlDecimal(10).Value;
                    returnsheet.Day6 = sheetreader.GetSqlDecimal(11).Value;
                    returnsheet.Day7 = sheetreader.GetSqlDecimal(12).Value;
                    returnsheet.Overtime = sheetreader.GetSqlDecimal(13).Value;
                    returnsheet.TStatus = sheetreader.GetByte(14);
                    returnsheet.Comments = sheetreader.IsDBNull(15) ? "" : sheetreader.GetString(15);
                }
                connection.Close();
                return Ok(returnsheet);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("submit")]
        public ActionResult<QuickSheet[]> Submit([FromBody]QuickSheet client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 1))
                {
                    return Unauthorized();
                }

                //get calender weeks 
                CultureInfo myCI = new CultureInfo("en-US");
                Calendar myCal = myCI.Calendar;
                CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
                DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
                int week = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
                DateTime biweekstart = (week % 2 == 0) ? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) : DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
                DateTime packagestart = biweekstart.AddDays(-56);

                //intalize the return pacakge
                QuickSheet[] package = new QuickSheet[5];
                //now return with full package with timesheet id, dates, and status
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand gettimesheet = new SqlCommand("TimesheetsGetQuick", connection);
                gettimesheet.Parameters.AddWithValue("@EmployeeID", client.EmployeeID);
                gettimesheet.Parameters.AddWithValue("@WeeksStart", packagestart);
                gettimesheet.Parameters.AddWithValue("@WeeksEnd", biweekstart.AddDays(7));
                gettimesheet.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader sheetsreader = gettimesheet.ExecuteReader();
                int i = 0; //index to iterate through package array
                Boolean t2 = true; // boolearn for timesheet 1 or 2, since sql is from latest to earliest, week2 will appear first

                bool change1 = false; // switch to update timesheet 1
                bool change2 = false; // switch to update timesheet 2

                while (sheetsreader.Read())
                {
                    if (t2)
                    {
                        package[i] = new QuickSheet
                        {
                            Sheet2ID = sheetsreader.GetInt32(0),
                            EndDate = sheetsreader.GetDateTime(2),
                            T2Status = sheetsreader.GetByte(3)
                        };
                        if (package[i].T2Status < 10 && package[i].Sheet2ID == client.Timesheet2.TimesheetID && sheetsreader.GetInt32(4) == client.EmployeeID)
                        {
                            change2 = true;
                            package[i].T2Status = 10;//preset the status
                        }
                        t2 = false;
                    }
                    else
                    {
                        package[i].Sheet1ID = sheetsreader.GetInt32(0);
                        package[i].StartDate = sheetsreader.GetDateTime(1);
                        package[i].T1Status = sheetsreader.GetByte(3);
                        if (package[i].T1Status < 10 && package[i].Sheet1ID == client.Timesheet1.TimesheetID && sheetsreader.GetInt32(4) == client.EmployeeID)
                        {
                            change1 = true;
                            package[i].T1Status = 10;//preset the status
                        }
                        t2 = true;
                        i++;
                    }
                }
                connection.Close();

                Array.Resize(ref package, i);

                if (!(change1 && change2))
                {
                    return Forbid();
                }

                if (change1)
                {
                    SqlCommand submit1 = new SqlCommand("TimesheetSubmit", connection);
                    submit1.Parameters.AddWithValue("@TimesheetID", client.Timesheet1.TimesheetID);
                    submit1.Parameters.AddWithValue("@Day1", client.Timesheet1.Day1);
                    submit1.Parameters.AddWithValue("@Day2", client.Timesheet1.Day2);
                    submit1.Parameters.AddWithValue("@Day3", client.Timesheet1.Day3);
                    submit1.Parameters.AddWithValue("@Day4", client.Timesheet1.Day4);
                    submit1.Parameters.AddWithValue("@Day5", client.Timesheet1.Day5);
                    submit1.Parameters.AddWithValue("@Day6", client.Timesheet1.Day6);
                    submit1.Parameters.AddWithValue("@Day7", client.Timesheet1.Day7);
                    submit1.Parameters.AddWithValue("@Overtime", client.Timesheet1.Overtime);
                    submit1.Parameters.AddWithValue("@Tstatus", 10);
                    submit1.Parameters.AddWithValue("@Comments", client.Comments);
                    submit1.Parameters.AddWithValue("@ManagerID", client.ManagerID);
                    submit1.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = submit1.ExecuteReader();
                    connection.Close();
                }
                if (change2)
                {
                    SqlCommand submit2 = new SqlCommand("TimesheetSubmit", connection);
                    submit2.Parameters.AddWithValue("@TimesheetID", client.Timesheet2.TimesheetID);
                    submit2.Parameters.AddWithValue("@Day1", client.Timesheet2.Day1);
                    submit2.Parameters.AddWithValue("@Day2", client.Timesheet2.Day2);
                    submit2.Parameters.AddWithValue("@Day3", client.Timesheet2.Day3);
                    submit2.Parameters.AddWithValue("@Day4", client.Timesheet2.Day4);
                    submit2.Parameters.AddWithValue("@Day5", client.Timesheet2.Day5);
                    submit2.Parameters.AddWithValue("@Day6", client.Timesheet2.Day6);
                    submit2.Parameters.AddWithValue("@Day7", client.Timesheet2.Day7);
                    submit2.Parameters.AddWithValue("@Overtime", client.Timesheet2.Overtime);
                    submit2.Parameters.AddWithValue("@Tstatus", 10);
                    submit2.Parameters.AddWithValue("@Comments", client.Comments);
                    submit2.Parameters.AddWithValue("@ManagerID", client.ManagerID);
                    submit2.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = submit2.ExecuteReader();
                    connection.Close();
                }
                return Ok(package);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [HttpPost("approve")]
        public ActionResult<QuickSheet[]> Approve([FromBody]QuickSheet client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 2))
                {
                    return Unauthorized();
                }

                bool change1 = false; // assuming sheet1 is earlier then sheet2
                bool change2 = false;
                //intialize return package
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand manager = new SqlCommand("TimesheetManager", connection);
                manager.Parameters.AddWithValue("@ManagerID", client.EmployeeID);
                manager.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader managerreader = manager.ExecuteReader();
                QuickSheet[] package = new QuickSheet[5];

                int i = 0;
                bool t2 = true;
                while (managerreader.Read() && (i < 5))
                {
                    if (t2)
                    {
                        package[i] = new QuickSheet
                        {
                            Firstname = managerreader.GetString(0),
                            Lastname = managerreader.GetString(1),
                            T2Status = managerreader.GetByte(5),
                            Sheet2ID = managerreader.GetInt32(2),
                            EndDate = managerreader.GetDateTime(4),
                        };
                        if (package[i].Sheet2ID == client.Sheet2ID && package[i].T2Status == 10 && client.EmployeeID == managerreader.GetInt32(7))
                        {
                            change2 = true;
                            package[i].T2Status = 20;
                        }
                        t2 = false;
                    }
                    else
                    {
                        package[i].StartDate = managerreader.GetDateTime(3);
                        package[i].Sheet1ID = managerreader.GetInt32(2);
                        package[i].T1Status = managerreader.GetByte(5);
                        if (package[i].Sheet1ID == client.Sheet1ID && package[i].T1Status == 10 && client.EmployeeID == managerreader.GetInt32(7))
                        {
                            change1 = true;
                            package[i].T1Status = 20;
                        }
                        t2 = true;
                        i++;
                    }
                }
                connection.Close();
                Array.Resize(ref package, i);

                if (!(change1 && change2))
                {
                    return Forbid();
                }

                if (change1)
                {
                    SqlCommand approve1 = new SqlCommand("TimesheetApprove", connection);
                    approve1.Parameters.AddWithValue("@TimesheetID", client.Sheet1ID);
                    approve1.Parameters.AddWithValue("@DateApproved", DateTime.Now.Date);
                    approve1.Parameters.AddWithValue("@TStatus", 20);
                    approve1.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = approve1.ExecuteReader();
                    connection.Close();
                }
                if (change2)
                {
                    SqlCommand approve2 = new SqlCommand("TimesheetApprove", connection);
                    approve2.Parameters.AddWithValue("@TimesheetID", client.Sheet2ID);
                    approve2.Parameters.AddWithValue("@DateApproved", DateTime.Now.Date);
                    approve2.Parameters.AddWithValue("@TStatus", 20);
                    approve2.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = approve2.ExecuteReader();
                    connection.Close();
                }
                return Ok(package);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [HttpPost("reject")]
        public ActionResult<QuickSheet[]> Reject([FromBody]QuickSheet client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 2))
                {
                    return Unauthorized();
                }

                bool change1 = false; // assuming sheet1 is earlier then sheet2
                bool change2 = false;

                //intialize return package
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand manager = new SqlCommand("TimesheetManager", connection);
                manager.Parameters.AddWithValue("@ManagerID", client.EmployeeID);
                manager.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader managerreader = manager.ExecuteReader();
                QuickSheet[] package = new QuickSheet[5];

                int i = 0;
                bool t2 = true;
                while (managerreader.Read() && (i < 5))
                {
                    if (t2)
                    {
                        package[i] = new QuickSheet
                        {
                            Firstname = managerreader.GetString(0),
                            Lastname = managerreader.GetString(1),
                            T2Status = managerreader.GetByte(5),
                            Sheet2ID = managerreader.GetInt32(2),
                            EndDate = managerreader.GetDateTime(4),
                        };
                        if (package[i].Sheet2ID == client.Sheet2ID && package[i].T2Status == 10 && client.EmployeeID == managerreader.GetInt32(7))
                        {
                            change2 = true;
                            package[i].T2Status = 5;
                        }
                        t2 = false;
                    }
                    else
                    {
                        package[i].StartDate = managerreader.GetDateTime(3);
                        package[i].Sheet1ID = managerreader.GetInt32(2);
                        package[i].T1Status = managerreader.GetByte(5);
                        if (package[i].Sheet1ID == client.Sheet1ID && package[i].T1Status == 10 && client.EmployeeID == managerreader.GetInt32(7))
                        {
                            change1 = true;
                            package[i].T1Status = 5;
                        }
                        t2 = true;
                        i++;
                    }
                }
                connection.Close();
                Array.Resize(ref package, i);

                if (!(change1 && change2))
                {
                    return Forbid();
                }

                if (change1)
                {
                    SqlCommand reject = new SqlCommand("TimesheetReject", connection);
                    reject.Parameters.AddWithValue("@TimesheetID", client.Sheet1ID);
                    reject.Parameters.AddWithValue("@Comments", client.Comments);
                    reject.Parameters.AddWithValue("@TStatus", 5);
                    reject.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = reject.ExecuteReader();
                    connection.Close();
                }
                if (change2)
                {
                    SqlCommand reject = new SqlCommand("TimesheetReject", connection);
                    reject.Parameters.AddWithValue("@TimesheetID", client.Sheet2ID);
                    reject.Parameters.AddWithValue("@Comments", client.Comments);
                    reject.Parameters.AddWithValue("@TStatus", 5);
                    reject.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    SqlDataReader execute = reject.ExecuteReader();
                    connection.Close();
                }
                return Ok(package);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
