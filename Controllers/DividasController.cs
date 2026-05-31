using Microsoft.AspNetCore.Mvc;
using VendinhaBackend.Requests;
using VendinhaBackend.Services;

namespace VendinhaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DividasController : ControllerBase
    {
        private readonly DividaService service;

        public DividasController(DividaService service)
        {
            this.service = service;
        }

        [HttpGet("cliente/{clienteId}")]
        public IActionResult ListarPorCliente(int clienteId)
        {
            return service.ListarPorCliente(clienteId);
        }

        [HttpPost]
        public IActionResult Criar(CriarDividaRequest request)
        {
            return service.Criar(request);
        }

        [HttpPut("{id}/pagar")]
        public IActionResult MarcarComoPaga(int id)
        {
            return service.MarcarComoPaga(id);
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            return service.Excluir(id);
        }
    }
}
