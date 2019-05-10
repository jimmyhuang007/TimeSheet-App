using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using projd.MailModel;
using System.Data.SqlClient;
using System.Data;
using projd.UserModel;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projd.Controllers
{
    [Route("api/[controller]")]
    public class Email : Controller
    {
        // GET: api/<controller>
        public static string SQLConnection = "Data Source=LAPTOP-N4D761U2\\JIMSQL;Initial Catalog=Main;Integrated Security=True";
        // NOTE CHANGE EMAIL AND PASSWORD BELOW
        [HttpGet]
        [Route("getemails")]
        public ActionResult<List<Mail>> Emails()
        {
            try
            {
                List<Mail> maillist = new List<Mail>();
                SqlConnection connection = new SqlConnection(SQLConnection);
                SqlCommand getmaillst = new SqlCommand("LoginEmail", connection);
                getmaillst.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader mailreader = getmaillst.ExecuteReader();
                while (mailreader.Read())
                {
                    maillist.Add(new Mail()
                    {
                        EmployeeID = mailreader.GetInt32(0),
                        EmailAddress = mailreader.GetString(1),
                        EmployeeMail = mailreader.IsDBNull(2) ? null : mailreader.GetString(2),
                    });
                }
                connection.Close();
                return Ok(maillist);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("sent")]
        public ActionResult Mail([FromBody]Mail msg)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.office365.com");
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("hh2huang@edu.uwaterloo.ca", "password");

                MailMessage mail = new MailMessage("hh2huang@edu.uwaterloo.ca", msg.EmailAddress);
                mail.Subject = msg.MailSubject;
                mail.Body = msg.MailBody;
                client.Send(mail);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
