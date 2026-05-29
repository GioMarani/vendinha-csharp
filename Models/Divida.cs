namespace VendinhaBackend.Models
{
    public class Divida
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public decimal Valor { get; set; }
        public string Situacao { get; set; } = "Aberta";
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataPagamento { get; set; }
    }
}
