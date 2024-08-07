using LoginRegistrationAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LoginRegistrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public IActionResult Registration(Registration registration)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatrimonyCon").ToString()))
            {
                string query = "INSERT INTO Registration(FirstName, LastName, Email, Password, ConfirmPassword) VALUES (@FirstName, @LastName, @Email, @Password, @ConfirmPassword)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", registration.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", registration.LastName);
                    cmd.Parameters.AddWithValue("@Email", registration.Email);
                    cmd.Parameters.AddWithValue("@Password", registration.Password);
                    cmd.Parameters.AddWithValue("@ConfirmPassword", registration.ConfirmPassword);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();

                    if (i > 0)
                    {
                        return Ok("Data inserted");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting data");
                    }
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatrimonyCon").ToString()))
            {
                string query = "SELECT * FROM Registration WHERE Email = @Email AND Password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", loginModel.Email);
                    cmd.Parameters.AddWithValue("@Password", loginModel.Password);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        return Ok("Valid User");
                    }
                    else
                    {
                        return Unauthorized("Invalid User");
                    }
                }
            }
        }

    }
}
