using System.ComponentModel.DataAnnotations;

namespace VendinhaBackend.Utils
{
    public class DataNascimentoValidaAttribute : ValidationAttribute
    {
        public DataNascimentoValidaAttribute()
        {
            ErrorMessage = "Informe uma data de nascimento valida.";
        }

        public override bool IsValid(object value)
        {
            if (value is not DateTime dataNascimento)
            {
                return false;
            }

            return dataNascimento != DateTime.MinValue &&
                   dataNascimento.Date < DateTime.Today;
        }
    }
}
