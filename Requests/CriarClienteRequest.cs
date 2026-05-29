namespace VendinhaBackend.Requests
{
    public class CriarClienteRequest
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; }
    }
}
