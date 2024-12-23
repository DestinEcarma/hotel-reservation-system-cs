namespace hotel_reservation_system.libs
{
    using MySql.Data.MySqlClient;

    public class MySqlDatabase
    {
        private readonly string _connectionString;

        public readonly static string DB_NOT_CONNECTED = "There seem's to be a problem with the database";
        public readonly static string DB_FAILED_TRANSACTION = "The sql transaction failed";

        public MySqlDatabase(string? connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null."); ;
        }

        public MySqlConnection? GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(_connectionString);

            try
            {
                connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }
    }
}
