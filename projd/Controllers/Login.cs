using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using projd.UserModel;
using System.Web.Http.Cors;
using System.IdentityModel;
using System.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projd.Controllers
{
    [Route("api/[controller]")]
    public class Login : Controller
    {
        public static string SQLConnection = "Data Source=LAPTOP-N4D761U2\\JIMSQL;Initial Catalog=Main;Integrated Security=True";

        // POST api/<controller>
        [HttpPost]
        [Route("authenticate")]
        public ActionResult<User> Authenticate([FromBody]User client)
        {
            try
            {
                SqlConnection conn = new SqlConnection(SQLConnection);
                SqlCommand cmd = new SqlCommand("LoginUser", conn);
                cmd.Parameters.AddWithValue("@LoginID", client.LoginID);
                cmd.Parameters.AddWithValue("@PasswordID", client.PasswordID);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    User _user = new User
                    {
                        EmployeeID = rdr.GetInt32(0),
                        LoginID = rdr.GetString(1),
                        EmployeeType = rdr.GetByte(2),
                        LastName = rdr.GetString(3),
                        FirstName = rdr.GetString(4),
                        Email = rdr.GetString(5),
                        DepartmentName = rdr.GetString(6),
                        PositionTitle = rdr.GetString(7),
                        ManagerID = rdr.IsDBNull(8) ? 0 : rdr.GetInt32(8),
                    };

                    //create the jwt token with key
                    string key = "hometrustisthebestcompany";
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                    var header = new JwtHeader(credentials);
                    var payload = new JwtPayload {
                        { "id", rdr.GetInt32(0).ToString() },
                        { "type", rdr.GetByte(2).ToString()}
                    };
                    var Token = new JwtSecurityToken(header, payload);
                    var handler = new JwtSecurityTokenHandler();
                    var tokenString = handler.WriteToken(Token);

                    _user.jwt = tokenString;
                    conn.Close();
                    return Ok(_user);
                }
                else
                {
                    conn.Close();
                    return Unauthorized();
                }
            }
            catch (global::System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("change")]
        public ActionResult<User> Change([FromBody]User client)
        {
            try
            {
                SqlConnection conn = new SqlConnection(SQLConnection);
                SqlCommand changecmd = new SqlCommand("LoginChange", conn);
                changecmd.Parameters.AddWithValue("@LoginID", client.LoginID);
                changecmd.Parameters.AddWithValue("@PasswordID", client.PasswordID);
                changecmd.Parameters.AddWithValue("@NewPassword", client.Newpassword);
                changecmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader rdr = changecmd.ExecuteReader();
                conn.Close();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
