using System.ComponentModel.DataAnnotations;
using VendinhaBackend.Utils;

namespace VendinhaBackend.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome completo e obrigatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome completo deve ter entre 3 e 100 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF e obrigatorio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 numeros.")]
        [CpfValido]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento e obrigatoria.")]
        [DataNascimentoValida]
        public DateTime DataNascimento { get; set; }

        private string email;

        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        [StringLength(120, ErrorMessage = "O e-mail deve ter no maximo 120 caracteres.")]
        public string Email
        {
            get { return email; }
            set { email = string.IsNullOrWhiteSpace(value) ? null : value.ToLower(); }
        }

        [Range(1, 120, ErrorMessage = "A idade deve estar entre 1 e 120 anos.")]
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

        public List<Divida> Dividas { get; set; } = new List<Divida>();
    }
}
