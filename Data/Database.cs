using Microsoft.Data.Sqlite;

namespace VendinhaBackend.Data
{
    public class Database
    {
        private string connectionString = "Data Source=Database/vendinha.db";

        public SqliteConnection CriarConexao()
        {
            return new SqliteConnection(connectionString);
        }
    }
}
