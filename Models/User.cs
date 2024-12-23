namespace hotel_reservation_system.Models
{
    public class User
    {

        public required ulong Id { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}
