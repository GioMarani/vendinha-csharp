using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VendinhaBackend.Data;
using VendinhaBackend.Models;
using VendinhaBackend.Requests;
using VendinhaBackend.Utils;

namespace VendinhaBackend.Services
{
    public class ClienteService
    {
        private readonly VendinhaDbContext context;

        public ClienteService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public IActionResult Listar(string busca, int pagina)
        {
            if (pagina <= 0)
            {
                return new BadRequestObjectResult("Pagina invalida.");
            }

            int tamanhoPagina = 10;
            int pular = (pagina - 1) * tamanhoPagina;

            var consulta = context.Clientes
                .Include(e => e.Dividas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                consulta = consulta.Where(e => e.NomeCompleto.Contains(busca));
            }

            var clientes = consulta
                .AsEnumerable()
                .Select(cliente => new
                {
                    cliente.Id,
                    cliente.NomeCompleto,
                    cliente.Cpf,
                    cliente.DataNascimento,
                    cliente.Idade,
                    cliente.Email,
                    TotalDividas = cliente.Dividas
                        .Where(divida => divida.Situacao == "Aberta")
                        .Sum(divida => divida.Valor)
                })
                .OrderByDescending(e => e.TotalDividas)
                .ThenBy(e => e.NomeCompleto)
                .Skip(pular)
                .Take(tamanhoPagina)
                .ToList();

            return new OkObjectResult(clientes);
        }

        public IActionResult ObterPorId(int id)
        {
            var cliente = context.Clientes
                .Include(e => e.Dividas)
                .FirstOrDefault(e => e.Id == id);

            if (cliente == null)
            {
                return new NotFoundObjectResult("Cliente nao encontrado.");
            }

            return new OkObjectResult(MontarResposta(cliente));
        }

        public IActionResult Criar(CriarClienteRequest request)
        {
            if (request == null)
            {
                return new BadRequestObjectResult("Dados invalidos.");
            }

            var cliente = new Cliente
            {
                NomeCompleto = request.NomeCompleto,
                Cpf = DocumentoUtils.SomenteNumeros(request.Cpf),
                DataNascimento = request.DataNascimento,
                Email = request.Email
            };

            if (!Validar(cliente, out var erros))
            {
                return new BadRequestObjectResult(FormatarErros(erros));
            }

            var cpfJaExiste = context.Clientes.Any(e => e.Cpf == cliente.Cpf);

            if (cpfJaExiste)
            {
                return new BadRequestObjectResult("Ja existe cliente com este CPF.");
            }

            context.Clientes.Add(cliente);
            context.SaveChanges();

            return new CreatedResult($"/api/clientes/{cliente.Id}", MontarResposta(cliente));
        }

        public IActionResult Atualizar(int id, AtualizarClienteRequest request)
        {
            if (request == null)
            {
                return new BadRequestObjectResult("Dados invalidos.");
            }

            var cliente = context.Clientes.FirstOrDefault(e => e.Id == id);

            if (cliente == null)
            {
                return new NotFoundObjectResult("Cliente nao encontrado.");
            }

            cliente.NomeCompleto = request.NomeCompleto;
            cliente.DataNascimento = request.DataNascimento;
            cliente.Email = request.Email;

            if (!Validar(cliente, out var erros))
            {
                return new BadRequestObjectResult(FormatarErros(erros));
            }

            context.SaveChanges();

            return new OkObjectResult(MontarResposta(cliente));
        }

        public IActionResult Excluir(int id)
        {
            var cliente = context.Clientes.FirstOrDefault(e => e.Id == id);

            if (cliente == null)
            {
                return new NotFoundObjectResult("Cliente nao encontrado.");
            }

            context.Clientes.Remove(cliente);
            context.SaveChanges();

            return new OkObjectResult("Cliente excluido.");
        }

        private bool Validar(Cliente cliente, out List<ValidationResult> erros)
        {
            var contexto = new ValidationContext(cliente);
            erros = new List<ValidationResult>();
            return Validator.TryValidateObject(cliente, contexto, erros, true);
        }

        private static List<string> FormatarErros(List<ValidationResult> erros)
        {
            return erros
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        private static object MontarResposta(Cliente cliente)
        {
            return new
            {
                cliente.Id,
                cliente.NomeCompleto,
                cliente.Cpf,
                cliente.DataNascimento,
                cliente.Idade,
                cliente.Email,
                TotalDividas = cliente.Dividas?
                    .Where(e => e.Situacao == "Aberta")
                    .Sum(e => e.Valor) ?? 0
            };
        }
    }
}
