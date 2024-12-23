using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Models
{
    public class Room
    {
        public class InputObject
        {
            public required string RoomNumber { get; set; }
            public required string RoomType { get; set; }
        }

        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }

        public Room(int id, string roomNumber, string roomType)
        {
            Id = id;
            RoomNumber = roomNumber;
            RoomType = roomType;
        }

        public Room(MySqlDataReader reader)
        {
            Id = reader.GetInt32("id");
            RoomNumber = reader.GetString("room_number");
            RoomType = reader.GetString("room_type");
        }
    }
}
