using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(MySqlDatabase database) : Controller
    {
        private static class Queries
        {
            public readonly static string READ = "SELECT * FROM staffs WHERE username = @username";
        }

        public class ResponseData
        {
            public required string Token { get; set; }
            public required string Role { get; set; }
        }

        MySqlDatabase _database = database;

        [HttpPost]
        public ActionResult<ResponseData> Post([FromBody] LoginCredentials credentials)
        {
            if (credentials.Username == DotNetEnv.Env.GetString("ADMIN_USERNAME") && credentials.Password == DotNetEnv.Env.GetString("ADMIN_PASSWORD"))
            {
                return Ok(new ResponseData()
                {
                    Token = JwtBuilder.Create()
                        .WithAlgorithm(new JWT.Algorithms.HMACSHA256Algorithm())
                        .WithSecret(Environment.GetEnvironmentVariable("JWT_SECRET"))
                        .AddClaim("Id", 0)
                        .AddClaim("Username", "admin")
                        .AddClaim("Role", "admin")
                        .Encode(),
                    Role = "admin"
                });
            }
            else
            {
                MySqlConnection? connection = _database.GetConnection();

                if (connection == null)
                {
                    return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
                }

                using (connection)
                {
                    using MySqlCommand command = new(Queries.READ, connection);

                    command.Parameters.AddWithValue("@username", credentials.Username);

                    using MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Staff.WithPasswordHash staff = new(reader);

                        if (BCrypt.Net.BCrypt.Verify(credentials.Password, staff.PasswordHash))
                        {
                            return Ok(new ResponseData()
                            {
                                Token = JwtBuilder.Create()
                                    .WithAlgorithm(new JWT.Algorithms.HMACSHA256Algorithm())
                                    .WithSecret(Environment.GetEnvironmentVariable("JWT_SECRET"))
                                    .AddClaim("Id", staff.Id)
                                    .AddClaim("Username", staff.Username)
                                    .AddClaim("Role", "staff")
                                    .Encode(),
                                Role = "staff"
                            });
                        }
                    }
                }

                return Unauthorized();
            }
        }
    }
}
