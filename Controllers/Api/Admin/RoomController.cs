using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers.Api.AdminController
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class RoomController(MySqlDatabase database) : Controller
    {
        private class Queries
        {
            public readonly static string CREATE = "INSERT INTO rooms SET room_number = @roomNumber, room_type = @roomType; SELECT LAST_INSERT_ID();";

            public static class Read
            {
                public readonly static string ALL = "SELECT * FROM rooms";
                public readonly static string FROM_ID = "SELECT * FROM rooms WHERE id = @id";
                public readonly static string FROM_ROOM_NUMBER = "SELECT * FROM rooms WHERE room_number = @roomNumber";
            }

            public static class Update
            {
                public readonly static string ROOM_NUMBER = "UPDATE rooms SET room_number = @roomNumber WHERE id = @id";
                public readonly static string ROOM_TYPE = "UPDATE rooms SET room_type = @roomType WHERE id = @id";
            }

            public readonly static string DELETE = "DELETE FROM rooms WHERE id = @id";
        }

        private readonly MySqlDatabase _database = database;

        [HttpPost]
        public ActionResult<ulong> Post([FromBody] Room.InputObject input)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.CREATE, connection);

                command.Parameters.AddWithValue("@roomNumber", input.RoomNumber);
                command.Parameters.AddWithValue("@roomType", input.RoomType);

                return StatusCode(201, Convert.ToUInt64(command.ExecuteScalar()));
            }
        }

        [HttpGet]
        public ActionResult<List<Room>> Get()
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

                List<Room> rooms = [];

                while (reader.Read())
                {
                    rooms.Add(item: new Room(reader));
                }

                return Ok(rooms);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Room> Get(ulong id)
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
                    return Ok(new Room(reader));
                }
            }

            return NotFound();
        }

        [HttpGet("number/{roomNumber}")]
        public ActionResult<Room> Get(string roomNumber)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.Read.FROM_ROOM_NUMBER, connection);

                command.Parameters.AddWithValue("@roomNumber", roomNumber);

                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new Room(reader));
                }
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult<Room> Put(ulong id, [FromBody] Room.InputObject input)
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
                    Room? room = (Room?)okResult.Value;

                    if (room == null)
                    {
                        return NotFound();
                    }

                    if (room.RoomType != input.RoomType)
                    {
                        using MySqlCommand command = new(Queries.Update.ROOM_NUMBER, connection);

                        command.Parameters.AddWithValue("@roomNumber", input.RoomNumber);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            room.RoomNumber = input.RoomNumber;
                        }
                    }

                    if (room.RoomType != input.RoomType)
                    {
                        using MySqlCommand command = new(Queries.Update.ROOM_TYPE, connection);

                        command.Parameters.AddWithValue("@roomType", input.RoomType);
                        command.Parameters.AddWithValue("@id", id);

                        if (command.ExecuteNonQuery() != 0)
                        {
                            room.RoomType = input.RoomType;
                        }
                    }

                    return Ok(room);
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
