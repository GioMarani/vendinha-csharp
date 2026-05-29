using Microsoft.Data.Sqlite;

namespace VendinhaBackend.Data
{
    public class Database
    {
        private string connectionString = "Data Source=C:\\data\\sqlite\\vendinha.db";

        public SqliteConnection CriarConexao()
        {
            return new SqliteConnection(connectionString);
        }
    }
}
