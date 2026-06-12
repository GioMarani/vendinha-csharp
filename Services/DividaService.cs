using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VendinhaBackend.Data;
using VendinhaBackend.Models;
using VendinhaBackend.Requests;

namespace VendinhaBackend.Services
{
    public class DividaService
    {
        private readonly VendinhaDbContext context;

        public DividaService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public IActionResult ListarPorCliente(int clienteId)
        {
            if (clienteId <= 0)
            {
                return new BadRequestObjectResult("Cliente invalido.");
            }

            var clienteExiste = context.Clientes.Any(e => e.Id == clienteId);

            if (!clienteExiste)
            {
                return new NotFoundObjectResult("Cliente nao encontrado.");
            }

            var dividas = context.Dividas
                .Where(e => e.ClienteId == clienteId)
                .OrderByDescending(e => e.DataCriacao)
                .ToList();

            return new OkObjectResult(dividas);
        }

        public IActionResult Criar(CriarDividaRequest request)
        {
            if (request == null)
            {
                return new BadRequestObjectResult("Dados invalidos.");
            }

            var divida = new Divida
            {
                ClienteId = request.ClienteId,
                Valor = request.Valor,
                Situacao = "Aberta",
                DataCriacao = DateTime.Now,
                DataPagamento = null
            };

            if (!Validar(divida, out var erros))
            {
                return new BadRequestObjectResult(FormatarErros(erros));
            }

            var clienteExiste = context.Clientes.Any(e => e.Id == divida.ClienteId);

            if (!clienteExiste)
            {
                return new NotFoundObjectResult("Cliente nao encontrado.");
            }

            var temDividaAberta = context.Dividas.Any(e =>
                e.ClienteId == divida.ClienteId &&
                e.Situacao == "Aberta");

            if (temDividaAberta)
            {
                return new BadRequestObjectResult("O cliente ja possui divida em aberto.");
            }

            context.Dividas.Add(divida);
            context.SaveChanges();

            return new CreatedResult($"/api/dividas/{divida.Id}", divida);
        }

        public IActionResult MarcarComoPaga(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult("Divida invalida.");
            }

            var divida = context.Dividas.FirstOrDefault(e =>
                e.Id == id &&
                e.Situacao == "Aberta");

            if (divida == null)
            {
                return new NotFoundObjectResult("Divida nao encontrada ou ja paga.");
            }

            divida.Situacao = "Paga";
            divida.DataPagamento = DateTime.Now;

            if (!Validar(divida, out var erros))
            {
                return new BadRequestObjectResult(FormatarErros(erros));
            }

            context.SaveChanges();

            return new OkObjectResult("Divida paga.");
        }

        public IActionResult Excluir(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult("Divida invalida.");
            }

            var divida = context.Dividas.FirstOrDefault(e => e.Id == id);

            if (divida == null)
            {
                return new NotFoundObjectResult("Divida nao encontrada.");
            }

            context.Dividas.Remove(divida);
            context.SaveChanges();

            return new OkObjectResult("Divida excluida.");
        }

        private bool Validar(Divida divida, out List<ValidationResult> erros)
        {
            var contexto = new ValidationContext(divida);
            erros = new List<ValidationResult>();
            return Validator.TryValidateObject(divida, contexto, erros, true);
        }

        private static List<string> FormatarErros(List<ValidationResult> erros)
        {
            return erros
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
}
