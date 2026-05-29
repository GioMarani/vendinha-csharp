namespace VendinhaBackend.Requests
{
    public class AtualizarClienteRequest
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; }
    }
}
