using Microsoft.AspNetCore.Mvc;
using VendinhaBackend.Data;
using VendinhaBackend.Models;
using VendinhaBackend.Requests;

namespace VendinhaBackend.Services
{
    public class DividaService
    {
        private readonly Database database;

        public DividaService(Database database)
        {
            this.database = database;
        }

        public IActionResult ListarPorCliente(int clienteId)
        {
            if (clienteId <= 0)
            {
                return new BadRequestObjectResult("Cliente inválido.");
            }

            var dividas = new List<Divida>();

            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, ClienteId, Valor, Situacao, DataCriacao, DataPagamento
                FROM Dividas
                WHERE ClienteId = @clienteId
                ORDER BY DataCriacao DESC;";
            command.Parameters.AddWithValue("@clienteId", clienteId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                dividas.Add(new Divida
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Valor = reader.GetDecimal(2),
                    Situacao = reader.GetString(3),
                    DataCriacao = DateTime.Parse(reader.GetString(4)),
                    DataPagamento = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
                });
            }

            return new OkObjectResult(dividas);
        }

        public IActionResult Criar(CriarDividaRequest request)
        {
            if (request == null)
            {
                return new BadRequestObjectResult("Dados inválidos.");
            }

            if (request.ClienteId <= 0)
            {
                return new BadRequestObjectResult("Cliente inválido.");
            }

            if (request.Valor <= 0)
            {
                return new BadRequestObjectResult("Valor inválido.");
            }

            using var connection = database.CriarConexao();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var verificarCliente = connection.CreateCommand();
                verificarCliente.Transaction = transaction;
                verificarCliente.CommandText = "SELECT COUNT(1) FROM Clientes WHERE Id = @clienteId";
                verificarCliente.Parameters.AddWithValue("@clienteId", request.ClienteId);
                long clienteExiste = (long)(verificarCliente.ExecuteScalar() ?? 0L);

                if (clienteExiste == 0)
                {
                    transaction.Rollback();
                    return new NotFoundObjectResult("Cliente não encontrado.");
                }

                using var verificarDivida = connection.CreateCommand();
                verificarDivida.Transaction = transaction;
                verificarDivida.CommandText = @"
                    SELECT COUNT(1)
                    FROM Dividas
                    WHERE ClienteId = @clienteId AND Situacao = 'Aberta';";
                verificarDivida.Parameters.AddWithValue("@clienteId", request.ClienteId);
                long dividasAbertas = (long)(verificarDivida.ExecuteScalar() ?? 0L);

                if (dividasAbertas > 0)
                {
                    transaction.Rollback();
                    return new BadRequestObjectResult("O cliente já possui dívida em aberto.");
                }

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO Dividas (ClienteId, Valor, Situacao, DataCriacao, DataPagamento)
                    VALUES (@clienteId, @valor, 'Aberta', @dataCriacao, NULL);
                    SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@clienteId", request.ClienteId);
                command.Parameters.AddWithValue("@valor", request.Valor);
                command.Parameters.AddWithValue("@dataCriacao", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                long id = (long)(command.ExecuteScalar() ?? 0L);
                transaction.Commit();

                return new CreatedResult($"/api/dividas/{id}", new { Id = id });
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public IActionResult MarcarComoPaga(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult("Dívida inválida.");
            }

            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Dividas
                SET Situacao = 'Paga',
                    DataPagamento = @dataPagamento
                WHERE Id = @id AND Situacao = 'Aberta';";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@dataPagamento", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            int linhasAfetadas = command.ExecuteNonQuery();
            if (linhasAfetadas == 0)
            {
                return new NotFoundObjectResult("Dívida não encontrada ou já paga.");
            }

            return new OkObjectResult("Dívida paga.");
        }

        public IActionResult Excluir(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult("Dívida inválida.");
            }

            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Dividas WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            int linhasAfetadas = command.ExecuteNonQuery();
            if (linhasAfetadas == 0)
            {
                return new NotFoundObjectResult("Dívida não encontrada.");
            }

            return new OkObjectResult("Dívida excluída.");
        }
    }
}
