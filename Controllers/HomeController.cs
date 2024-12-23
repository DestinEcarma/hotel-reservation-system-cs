using hotel_reservation_system.libs;
using hotel_reservation_system.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Controllers
{
    [Route("")]
    [Route("home")]
    public class HomeController(MySqlDatabase database) : Controller
    {
        public static class Queries
        {
            public readonly static string CREATE = """
            INSERT INTO inquiries SET
                name = @name,
                contact = @contact,
                room_type = @roomType,
                start_date = @startDate,
                end_date = @endDate;
            """;
        }

        private readonly MySqlDatabase _database = database;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Inquery.InputObject input)
        {
            Console.WriteLine(input.Name);
            Console.WriteLine(input.Contact);
            Console.WriteLine(input.RoomType);
            Console.WriteLine(input.StartDate);
            Console.WriteLine(input.EndDate);


            MySqlConnection? connection = _database.GetConnection();

            if (connection == null)
            {
                return StatusCode(500, MySqlDatabase.DB_NOT_CONNECTED);
            }

            using (connection)
            {
                using MySqlCommand command = new(Queries.CREATE, connection);

                command.Parameters.AddWithValue("@name", input.Name);
                command.Parameters.AddWithValue("@contact", input.Contact);
                command.Parameters.AddWithValue("@roomType", input.RoomType);
                command.Parameters.AddWithValue("@startDate", input.StartDate);
                command.Parameters.AddWithValue("@endDate", input.EndDate);

                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
