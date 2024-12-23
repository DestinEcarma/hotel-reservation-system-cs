using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Models
{
    public class Reservation
    {
        public class InputObject
        {
            public required string Name { get; set; }
            public required string Contact { get; set; }
            public required string RoomNumber { get; set; }
            public required DateOnly StartDate { get; set; }
            public required DateOnly EndDate { get; set; }
        }

        public int Id { get; set; }
        public string Customer { get; set; }
        public string RoomNumber { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool PaymentMade { get; set; }

        public Reservation(int id, string customer, string roomNumber, DateOnly startDate, DateOnly endDate, bool paymentMade)
        {
            Id = id;
            Customer = customer;
            RoomNumber = roomNumber;
            StartDate = startDate;
            EndDate = endDate;
            PaymentMade = paymentMade;
        }

        public Reservation(MySqlDataReader reader)
        {
            Id = reader.GetInt32("id");
            Customer = reader.GetString("customer");
            RoomNumber = reader.GetString("room_number");
            StartDate = DateOnly.FromDateTime(reader.GetDateTime("start_date"));
            EndDate = DateOnly.FromDateTime(reader.GetDateTime("end_date"));
            PaymentMade = reader.GetBoolean("payment_made");
        }
    }
}
