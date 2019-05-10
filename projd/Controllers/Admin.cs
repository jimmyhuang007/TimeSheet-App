using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using projd.Model;
using projd.QuickSheetModel;
using projd.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projd.Controllers
{

    [Route("api/[controller]")]
    public class Admin : Controller
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

        // POST api/<controller>
        [HttpPost]
        [Route("payroll")]
        public ActionResult<List<PayClass>> Payroll([FromBody]QuickSheet client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 8))
                {
                    return Unauthorized();
                }
                List<PayClass> paylist = new List<PayClass>();
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand getpaylst = new SqlCommand("TimesheetFilter", connection);
                getpaylst.Parameters.AddWithValue("@StartDate", client.StartDate);
                getpaylst.Parameters.AddWithValue("@EndDate", client.EndDate);
                getpaylst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader payreader = getpaylst.ExecuteReader();
                while (payreader.Read())
                {
                    paylist.Add(new PayClass()
                    {
                        EmployeeID = payreader.GetInt32(0),
                        StudentF = payreader.GetString(2),
                        StudentL = payreader.GetString(1),
                        Manager = payreader.GetString(4) + ' ' + payreader.GetString(5),
                        Regular = payreader.GetSqlDecimal(6).Value - payreader.GetSqlDecimal(7).Value,
                        Overtime = payreader.GetSqlDecimal(7).Value,
                        Total = payreader.GetSqlDecimal(6).Value,
                    });
                }
                connection.Close();
                return Ok(paylist);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("getuser")]
        public ActionResult<List<User>> GetUser([FromBody]User client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 9))
                {
                    return Unauthorized();
                }
                List<User> userlist = new List<User>();
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand getuserlst = new SqlCommand("LoginGet", connection);
                getuserlst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader userreader = getuserlst.ExecuteReader();
                while (userreader.Read())
                {
                    userlist.Add(new User()
                    {
                        EmployeeID = userreader.GetInt32(0),
                        LoginID = userreader.GetString(1),
                        EmployeeType = userreader.GetByte(2),
                        LastName = userreader.GetString(3),
                        FirstName = userreader.GetString(4),
                        Email = userreader.GetString(5),
                        DepartmentName = userreader.GetString(6),
                        PositionTitle = userreader.GetString(7),
                        ManagerID = userreader.IsDBNull(8) ? 0 : userreader.GetInt32(8),
                        PasswordID = userreader.GetString(9),
                    });
                }
                connection.Close();
                return Ok(userlist);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("update")]
        public ActionResult<List<User>> UpdateUser([FromBody]User client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 9))
                {
                    return Unauthorized();
                }

                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand update = new SqlCommand("LoginUpdate", connection);
                update.Parameters.AddWithValue("@EmployeeID", client.AdminUse.EmployeeID);
                update.Parameters.AddWithValue("@LoginID", client.AdminUse.LoginID);
                update.Parameters.AddWithValue("@EmployeeType", client.AdminUse.EmployeeType);
                update.Parameters.AddWithValue("@LastName", client.AdminUse.LastName);
                update.Parameters.AddWithValue("@FirstName", client.AdminUse.FirstName);
                update.Parameters.AddWithValue("@Email", client.AdminUse.Email);
                update.Parameters.AddWithValue("@DepartmentName", client.AdminUse.DepartmentName);
                update.Parameters.AddWithValue("@PositionTitle", client.AdminUse.PositionTitle);
                if (client.AdminUse.EmployeeType == 1)
                {
                    update.Parameters.AddWithValue("@ManagerID", client.AdminUse.ManagerID);
                }
                else
                {
                    update.Parameters.AddWithValue("@ManagerID", DBNull.Value);
                }
                update.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader createreader = update.ExecuteReader();
                connection.Close();
                List<User> userlist = new List<User>();
                SqlCommand getuserlst = new SqlCommand("LoginGet", connection);
                getuserlst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader userreader = getuserlst.ExecuteReader();
                while (userreader.Read())
                {
                    userlist.Add(new User()
                    {
                        EmployeeID = userreader.GetInt32(0),
                        LoginID = userreader.GetString(1),
                        EmployeeType = userreader.GetByte(2),
                        LastName = userreader.GetString(3),
                        FirstName = userreader.GetString(4),
                        Email = userreader.GetString(5),
                        DepartmentName = userreader.GetString(6),
                        PositionTitle = userreader.GetString(7),
                        ManagerID = userreader.IsDBNull(8) ? 0 : userreader.GetInt32(8),
                    });
                }
                connection.Close();
                return Ok(userlist);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("create")]
        public ActionResult<List<User>> CreateUser([FromBody]User client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 9))
                {
                    return Unauthorized();
                }

                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand create = new SqlCommand("LoginCreate", connection);
                create.Parameters.AddWithValue("@EmployeeID", client.AdminUse.EmployeeID);
                create.Parameters.AddWithValue("@LoginID", client.AdminUse.LoginID);
                create.Parameters.AddWithValue("@PasswordID", client.AdminUse.PasswordID);
                create.Parameters.AddWithValue("@EmployeeType", client.AdminUse.EmployeeType);
                create.Parameters.AddWithValue("@LastName", client.AdminUse.LastName);
                create.Parameters.AddWithValue("@FirstName", client.AdminUse.FirstName);
                create.Parameters.AddWithValue("@Email", client.AdminUse.Email);
                create.Parameters.AddWithValue("@DepartmentName", client.AdminUse.DepartmentName);
                create.Parameters.AddWithValue("@PositionTitle", client.AdminUse.PositionTitle);
                if (client.AdminUse.EmployeeType == 1) // Employee
                {
                    create.Parameters.AddWithValue("@ManagerID", client.AdminUse.ManagerID);
                }
                else // Manager
                {
                    create.Parameters.AddWithValue("@ManagerID", DBNull.Value);
                }
                create.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader createreader = create.ExecuteReader();
                connection.Close();

                List<User> userlist = new List<User>();
                SqlCommand getuserlst = new SqlCommand("LoginGet", connection);
                getuserlst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader userreader = getuserlst.ExecuteReader();
                while (userreader.Read())
                {
                    userlist.Add(new User()
                    {
                        EmployeeID = userreader.GetInt32(0),
                        LoginID = userreader.GetString(1),
                        EmployeeType = userreader.GetByte(2),
                        LastName = userreader.GetString(3),
                        FirstName = userreader.GetString(4),
                        Email = userreader.GetString(5),
                        DepartmentName = userreader.GetString(6),
                        PositionTitle = userreader.GetString(7),
                        ManagerID = userreader.IsDBNull(8) ? 0 : userreader.GetInt32(8),
                    });
                }
                connection.Close();
                return Ok(userlist);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("delete")]
        public ActionResult<List<User>> DeleteUser([FromBody]User client)
        {
            try
            {
                if (!(Validatejwt(client.jwt, client.EmployeeID.ToString(), client.EmployeeType.ToString()) && client.EmployeeType == 9))
                {
                    return Unauthorized();
                }
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand delete = new SqlCommand("LoginDelete", connection);
                delete.Parameters.AddWithValue("@EmployeeID", client.AdminUse.EmployeeID);
                delete.Parameters.AddWithValue("@EmployeeType", client.AdminUse.EmployeeType);
                delete.Parameters.AddWithValue("@LoginID", client.AdminUse.LoginID);
                delete.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader createreader = delete.ExecuteReader();
                connection.Close();

                List<User> userlist = new List<User>();
                SqlCommand getuserlst = new SqlCommand("LoginGet", connection);
                getuserlst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader userreader = getuserlst.ExecuteReader();
                while (userreader.Read())
                {
                    userlist.Add(new User()
                    {
                        EmployeeID = userreader.GetInt32(0),
                        LoginID = userreader.GetString(1),
                        EmployeeType = userreader.GetByte(2),
                        LastName = userreader.GetString(3),
                        FirstName = userreader.GetString(4),
                        Email = userreader.GetString(5),
                        DepartmentName = userreader.GetString(6),
                        PositionTitle = userreader.GetString(7),
                        ManagerID = userreader.IsDBNull(8) ? 0 : userreader.GetInt32(8),
                    });
                }
                connection.Close();
                return Ok(userlist);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
