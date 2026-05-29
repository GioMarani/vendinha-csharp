namespace VendinhaBackend.Requests
{
    public class CriarDividaRequest
    {
        public int ClienteId { get; set; }
        public decimal Valor { get; set; }
    }
}
