namespace hotel_reservation_system.Models
{
    public class Inquery
    {
        public class InputObject
        {
            public required string Name { get; set; }
            public required string Contact { get; set; }
            public required string RoomType { get; set; }
            public required string StartDate { get; set; }
            public required string EndDate { get; set; }
        }
    }
}
