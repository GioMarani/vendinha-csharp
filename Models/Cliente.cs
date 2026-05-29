namespace VendinhaBackend.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        private string email;

        public string Email
        {
            get { return email; }
            set { email = string.IsNullOrWhiteSpace(value) ? null : value.ToLower(); }
        }

        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var totalAnos = hoje.Year - DataNascimento.Year;
                var aniversarioAnoAtual = DataNascimento.AddYears(totalAnos);

                if (aniversarioAnoAtual > hoje)
                {
                    totalAnos--;
                }

                return totalAnos;
            }
        }
    }
}
