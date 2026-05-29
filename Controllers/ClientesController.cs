using Microsoft.AspNetCore.Mvc;
using VendinhaBackend.Data;
using VendinhaBackend.Models;
using VendinhaBackend.Requests;
using VendinhaBackend.Utils;

namespace VendinhaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly Database database;

        public ClientesController(Database database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Listar(string busca, int pagina = 1)
        {
            if (pagina <= 0)
            {
                return BadRequest("Página inválida.");
            }

            int tamanhoPagina = 10;
            int pular = (pagina - 1) * tamanhoPagina;
            var clientes = new List<object>();

            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.NomeCompleto, c.Cpf, c.DataNascimento, c.Email,
                       COALESCE(SUM(CASE WHEN d.Situacao = 'Aberta' THEN d.Valor ELSE 0 END), 0) AS TotalDividas
                FROM Clientes c
                LEFT JOIN Dividas d ON d.ClienteId = c.Id
                WHERE (@busca IS NULL OR c.NomeCompleto LIKE '%' || @busca || '%')
                GROUP BY c.Id, c.NomeCompleto, c.Cpf, c.DataNascimento, c.Email
                ORDER BY TotalDividas DESC, c.NomeCompleto ASC
                LIMIT @tamanhoPagina OFFSET @pular;";
            command.Parameters.AddWithValue("@busca", string.IsNullOrWhiteSpace(busca) ? DBNull.Value : busca);
            command.Parameters.AddWithValue("@tamanhoPagina", tamanhoPagina);
            command.Parameters.AddWithValue("@pular", pular);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var cliente = new Cliente
                {
                    Id = reader.GetInt32(0),
                    NomeCompleto = reader.GetString(1),
                    Cpf = reader.GetString(2),
                    DataNascimento = DateTime.Parse(reader.GetString(3)),
                    Email = reader.IsDBNull(4) ? null : reader.GetString(4)
                };

                clientes.Add(new
                {
                    cliente.Id,
                    cliente.NomeCompleto,
                    cliente.Cpf,
                    cliente.DataNascimento,
                    cliente.Idade,
                    cliente.Email,
                    TotalDividas = reader.GetDecimal(5)
                });
            }

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.NomeCompleto, c.Cpf, c.DataNascimento, c.Email,
                       COALESCE(SUM(CASE WHEN d.Situacao = 'Aberta' THEN d.Valor ELSE 0 END), 0) AS TotalDividas
                FROM Clientes c
                LEFT JOIN Dividas d ON d.ClienteId = c.Id
                WHERE c.Id = @id
                GROUP BY c.Id, c.NomeCompleto, c.Cpf, c.DataNascimento, c.Email;";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return NotFound("Cliente não encontrado.");
            }

            var cliente = new Cliente
            {
                Id = reader.GetInt32(0),
                NomeCompleto = reader.GetString(1),
                Cpf = reader.GetString(2),
                DataNascimento = DateTime.Parse(reader.GetString(3)),
                Email = reader.IsDBNull(4) ? null : reader.GetString(4)
            };

            return Ok(new
            {
                cliente.Id,
                cliente.NomeCompleto,
                cliente.Cpf,
                cliente.DataNascimento,
                cliente.Idade,
                cliente.Email,
                TotalDividas = reader.GetDecimal(5)
            });
        }

        [HttpPost]
        public IActionResult Criar(CriarClienteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NomeCompleto))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (!DocumentoUtils.CpfValido(request.Cpf))
            {
                return BadRequest("CPF inválido.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains("@"))
            {
                return BadRequest("E-mail inválido.");
            }

            string cpf = DocumentoUtils.SomenteNumeros(request.Cpf);

            using var connection = database.CriarConexao();
            connection.Open();

            using var verificar = connection.CreateCommand();
            verificar.CommandText = "SELECT COUNT(1) FROM Clientes WHERE Cpf = @cpf";
            verificar.Parameters.AddWithValue("@cpf", cpf);
            long existe = (long)(verificar.ExecuteScalar() ?? 0L);

            if (existe > 0)
            {
                return BadRequest("Já existe cliente com este CPF.");
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Clientes (NomeCompleto, Cpf, DataNascimento, Email)
                VALUES (@nomeCompleto, @cpf, @dataNascimento, @email);
                SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@nomeCompleto", request.NomeCompleto);
            command.Parameters.AddWithValue("@cpf", cpf);
            command.Parameters.AddWithValue("@dataNascimento", request.DataNascimento.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(request.Email) ? (object)DBNull.Value : request.Email.ToLower());

            long id = (long)(command.ExecuteScalar() ?? 0L);

            return Created($"/api/clientes/{id}", new { Id = id });
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, AtualizarClienteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NomeCompleto))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains("@"))
            {
                return BadRequest("E-mail inválido.");
            }

            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Clientes
                SET NomeCompleto = @nomeCompleto,
                    DataNascimento = @dataNascimento,
                    Email = @email
                WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@nomeCompleto", request.NomeCompleto);
            command.Parameters.AddWithValue("@dataNascimento", request.DataNascimento.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(request.Email) ? (object)DBNull.Value : request.Email.ToLower());

            int linhasAfetadas = command.ExecuteNonQuery();
            if (linhasAfetadas == 0)
            {
                return NotFound("Cliente não encontrado.");
            }

            return Ok("Cliente atualizado.");
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            using var connection = database.CriarConexao();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Clientes WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            int linhasAfetadas = command.ExecuteNonQuery();
            if (linhasAfetadas == 0)
            {
                return NotFound("Cliente não encontrado.");
            }

            return Ok("Cliente excluído.");
        }
    }
}
