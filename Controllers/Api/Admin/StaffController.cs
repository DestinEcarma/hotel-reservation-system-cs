using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers.Api.AdminController
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class StaffController(MySqlDatabase database) : Controller
    {
        private static class Queries
        {
            public readonly static string CREATE = "INSERT INTO staffs SET name = @name, contact = @contact, username = @username, password_hash = @passwordHash, password_length = @passwordLength; SELECT LAST_INSERT_ID();";

            public static class Read
            {
                public readonly static string ALL = "SELECT * FROM staffs";
                public readonly static string FROM_ID = "SELECT * FROM staffs WHERE id = @id";
                public readonly static string FROM_USERNAME = "SELECT * FROM staffs WHERE username = @username";
            }

            public static class Update
            {
                public readonly static string NAME = "UPDATE staffs SET name = @name WHERE id = @id";
                public readonly static string CONTACT = "UPDATE staffs SET contact = @contact WHERE id = @id";
                public readonly static string USERNAME = "UPDATE staffs SET username = @username WHERE id = @id";
                public readonly static string PASSWORD = "UPDATE staffs SET password_hash = @passwordStaff, password_length = @passwordLength WHERE id = @id";
            }

            public readonly static string DELETE = "DELETE FROM staffs WHERE id = @id";
        }

        private readonly MySqlDatabase _database = database;

        [HttpPost]
        public ActionResult<ulong> Post([FromBody] Staff.InputObject input)
        {
            MySqlConnection? connection = _database.GetConnection();
            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }
            using (connection)
            {
                using MySqlCommand command = new(Queries.CREATE, connection);

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);

                command.Parameters.AddWithValue("@name", input.Name);
                command.Parameters.AddWithValue("@contact", input.Contact);
                command.Parameters.AddWithValue("@username", input.Username);
                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                command.Parameters.AddWithValue("@passwordLength", input.Password.Length);

                return Ok(Convert.ToUInt64(command.ExecuteScalar()));
            }
        }

        [HttpGet]
        public ActionResult<List<Staff>> Get()
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.Read.ALL, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                List<Staff> staffs = [];

                while (reader.Read())
                {
                    staffs.Add(new Staff(reader));
                }

                return Ok(staffs);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Staff> Get(ulong id)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.Read.FROM_ID, connection);

                command.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new Staff(reader));
                }
            }

            return NotFound();
        }

        [HttpGet("username/{username}")]
        public ActionResult<Staff> Get(string username)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.Read.FROM_USERNAME, connection);

                command.Parameters.AddWithValue("@username", username);

                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new Staff(reader));
                }
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult<Staff> Put(ulong id, [FromBody] Staff.InputObject input)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                ActionResult? result = this.Get(id).Result;

                if (result is OkObjectResult okResult)
                {
                    Staff? staff = (Staff?)okResult.Value;

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    if (staff.Name != input.Name)
                    {
                        using MySqlCommand command = new(Queries.Update.NAME, connection);

                        command.Parameters.AddWithValue("@name", input.Name);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            staff.Name = input.Name;
                        }
                    }

                    if (staff.Contact != input.Contact)
                    {
                        using MySqlCommand command = new(Queries.Update.CONTACT, connection);

                        command.Parameters.AddWithValue("@contact", input.Contact);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            staff.Contact = input.Contact;
                        }
                    }

                    if (staff.Username != input.Username)
                    {
                        using MySqlCommand command = new(Queries.Update.USERNAME, connection);

                        command.Parameters.AddWithValue("@username", input.Username);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            staff.Username = input.Username;
                        }
                    }

                    if (new String('*', staff.PasswordLength) != new string('*', input.Password.Length))
                    {
                        using MySqlCommand command = new(Queries.Update.PASSWORD, connection);

                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);

                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        command.Parameters.AddWithValue("@passwordLength", input.Password.Length);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            staff.PasswordLength = input.Password.Length;
                        }
                    }

                    return Ok(staff);
                }

                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(ulong id)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.DELETE, connection);

                command.Parameters.AddWithValue("@id", id);

                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound();
                }

                return Ok();
            }
        }
    }
}
