using Microsoft.Data.Sqlite;

namespace VendinhaBackend.Data
{
    public class Database
    {
        private string connectionString = "Data Source=vendinha.db";

        public SqliteConnection CriarConexao()
        {
            return new SqliteConnection(connectionString);
        }

        public void CriarTabelas()
        {
            using var connection = CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    NomeCompleto TEXT NOT NULL,
                    Cpf TEXT NOT NULL UNIQUE,
                    DataNascimento TEXT NOT NULL,
                    Email TEXT NULL
                );

                CREATE TABLE IF NOT EXISTS Dividas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClienteId INTEGER NOT NULL,
                    Valor REAL NOT NULL,
                    Situacao TEXT NOT NULL,
                    DataCriacao TEXT NOT NULL,
                    DataPagamento TEXT NULL,
                    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE
                );";
            command.ExecuteNonQuery();
        }
    }
}
