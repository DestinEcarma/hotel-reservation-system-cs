using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers.Api.ReceptionistController
{
    [Route("api/receptionist/[controller]")]
    public class RoomController(MySqlDatabase database) : Controller
    {
        private static class Queries
        {
            public static class Read
            {
                public readonly static string NOT_IN_PERIOD_RANGE = """
                SELECT
                    ro.*
                FROM
                    rooms ro
                    LEFT JOIN reservations re
                    ON ro.id = re.room_id
                WHERE
                    ro.room_type = @roomType AND (
                        re.start_date IS NULL OR (
                            re.start_date NOT BETWEEN @startDate AND @endDate AND
                            re.end_date NOT BETWEEN @startDate AND @endDate
                        )
                    )
                """;
            }
        }

        private readonly MySqlDatabase _database = database;

        [HttpGet("available/{roomType}/{startDate}/{endDate}")]
        public ActionResult<List<Room>> Get(string roomType, DateOnly startDate, DateOnly endDate)
        {
            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.Read.NOT_IN_PERIOD_RANGE, connection);

                command.Parameters.AddWithValue("@roomType", roomType);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                using MySqlDataReader reader = command.ExecuteReader();

                List<Room> rooms = [];

                while (reader.Read())
                {
                    rooms.Add(item: new Room(reader));
                }

                return Ok(rooms);
            }
        }
    }
}
