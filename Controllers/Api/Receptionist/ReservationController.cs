using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers.Api.ReceptionistController
{
    [Route("api/receptionist/[controller]")]
    public class ReservationController(MySqlDatabase database) : Microsoft.AspNetCore.Mvc.Controller
    {
        private static class Queries
        {
            public static class Create
            {
                public readonly static string PART_1 = "INSERT INTO customers SET name = @name, contact = @contact; LAST_INSERT_ID();";
                public readonly static string PART_2 = """
                INSERT INTO
                    reservations
                SET
                    customer_id = @customerId,
                    room_id = (SELECT id FROM rooms WHERE room_number = @roomNumber),
                    handled_by = @handledBy,
                    start_date = @startDate,
                    end_date = @endDate;
                SELECT LAST_INSERT_ID();
                """;
            }

            public static class Read
            {
                public readonly static string ALL = """
                SELECT
                    re.*,
                    c.name AS customer,
                    ro.room_number AS room_number
                FROM
                    reservations re
                    LEFT JOIN customers c
                    ON re.customer_id = c.id
                    LEFT JOIN rooms ro
                    ON re.room_id = ro.id;
                """;
                public readonly static string FROM_ID = """
                SELECT
                    re.*,
                    c.name AS customer,
                    ro.room_number AS room_number
                FROM
                    reservations re
                    LEFT JOIN customers c
                    ON re.customer_id = c.id
                    LEFT JOIN rooms ro
                    ON re.room_id = ro.id;
                WHERE
                    re.id = @id;
                """;
            }
        }

        private readonly MySqlDatabase _database = database;

        [HttpPost]
        public ActionResult<ulong> Post([FromBody] Reservation.InputObject input)
        {
            if (HttpContext.Items.TryGetValue("session", out var sessionObj) && sessionObj is User user)
            {
                MySqlConnection? connection = _database.GetConnection();

                if (connection == null)
                {
                    return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
                }

                using (connection)
                {
                    using MySqlCommand customerCommand = new(Queries.Create.PART_1, connection);

                    customerCommand.Parameters.AddWithValue("@name", input.Name);
                    customerCommand.Parameters.AddWithValue("@contact", input.Contact);

                    ulong customerId = Convert.ToUInt64(customerCommand.ExecuteScalar());

                    using MySqlCommand reservationCommand = new(Queries.Create.PART_2, connection);

                    reservationCommand.Parameters.AddWithValue("@customerId", customerId);
                    reservationCommand.Parameters.AddWithValue("@roomNumber", input.RoomNumber);
                    reservationCommand.Parameters.AddWithValue("@handledBy", user.Id);
                    reservationCommand.Parameters.AddWithValue("@startDate", input.StartDate);
                    reservationCommand.Parameters.AddWithValue("@endDate", input.EndDate);

                    return StatusCode(201, Convert.ToUInt64(reservationCommand.ExecuteScalar()));
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public ActionResult<List<Reservation>> Get()
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

                List<Reservation> reservations = [];

                while (reader.Read())
                {
                    reservations.Add(item: new Reservation(reader));
                }

                return Ok(reservations);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> Get(ulong id)
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
                    return Ok(new Reservation(reader));
                }

                return NotFound();
            }
        }
    }
}
