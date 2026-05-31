using Microsoft.AspNetCore.Mvc;
using VendinhaBackend.Requests;
using VendinhaBackend.Services;

namespace VendinhaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService service;

        public ClientesController(ClienteService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Listar(string busca = "", int pagina = 1)
        {
            return service.Listar(busca, pagina);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            return service.ObterPorId(id);
        }

        [HttpPost]
        public IActionResult Criar(CriarClienteRequest request)
        {
            return service.Criar(request);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, AtualizarClienteRequest request)
        {
            return service.Atualizar(id, request);
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            return service.Excluir(id);
        }
    }
}
