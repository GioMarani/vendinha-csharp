using Microsoft.AspNetCore.Mvc;
using VendinhaBackend.Data;
using VendinhaBackend.Models;
using VendinhaBackend.Requests;

namespace VendinhaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DividasController : ControllerBase
    {
        private readonly Database database;

        public DividasController(Database database)
        {
            this.database = database;
        }

        [HttpGet("cliente/{clienteId}")]
        public IActionResult ListarPorCliente(int clienteId)
        {
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

            return Ok(dividas);
        }

        [HttpPost]
        public IActionResult Criar(CriarDividaRequest request)
        {
            if (request.ClienteId <= 0)
            {
                return BadRequest("Cliente inválido.");
            }

            if (request.Valor <= 0)
            {
                return BadRequest("Valor inválido.");
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
                    return NotFound("Cliente não encontrado.");
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
                    return BadRequest("O cliente já possui dívida em aberto.");
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

                return Created($"/api/dividas/{id}", new { Id = id });
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        [HttpPut("{id}/pagar")]
        public IActionResult MarcarComoPaga(int id)
        {
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
                return NotFound("Dívida não encontrada ou já paga.");
            }

            return Ok("Dívida paga.");
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Dividas WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            int linhasAfetadas = command.ExecuteNonQuery();
            if (linhasAfetadas == 0)
            {
                return NotFound("Dívida não encontrada.");
            }

            return Ok("Dívida excluída.");
        }
    }
}
