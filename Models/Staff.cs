using MySql.Data.MySqlClient;

namespace hotel_reservation_system.Models
{
    public class Staff
    {
        public class WithPasswordHash : Staff
        {
            public string PasswordHash { get; set; }

            public WithPasswordHash(MySqlDataReader reader)
            {
                this.Id = reader.GetInt32("id");
                this.Name = reader.GetString("name");
                this.Contact = reader.GetString("contact");
                this.Username = reader.GetString("username");
                this.PasswordLength = reader.GetInt32("password_length");
                this.PasswordHash = reader.GetString("password_hash");
            }
        }

        public class InputObject
        {
            public required string Name { get; set; }
            public required string Contact { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Username { get; set; }
        public int PasswordLength { get; set; }

        public Staff()
        {
            this.Id = 0;
            this.Name = "";
            this.Contact = "";
            this.Username = "";
            this.PasswordLength = 0;
        }

        public Staff(MySqlDataReader reader)
        {
            this.Id = reader.GetInt32("id");
            this.Name = reader.GetString("name");
            this.Contact = reader.GetString("contact");
            this.Username = reader.GetString("username");
            this.PasswordLength = reader.GetInt32("password_length");
        }
    }
}
