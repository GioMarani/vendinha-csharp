using System.ComponentModel.DataAnnotations;

namespace VendinhaBackend.Models
{
    public class Divida
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O cliente e obrigatorio.")]
        public int ClienteId { get; set; }

        [Range(typeof(decimal), "0,01", "999999999", ErrorMessage = "O valor da divida deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A situacao e obrigatoria.")]
        [StringLength(20, ErrorMessage = "A situacao deve ter no maximo 20 caracteres.")]
        public string Situacao { get; set; } = "Aberta";

        [Required(ErrorMessage = "A data de criacao e obrigatoria.")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataPagamento { get; set; }
        public Cliente Cliente { get; set; }
    }
}
