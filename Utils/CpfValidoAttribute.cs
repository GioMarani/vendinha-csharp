using System.ComponentModel.DataAnnotations;

namespace VendinhaBackend.Utils
{
    public class CpfValidoAttribute : ValidationAttribute
    {
        public CpfValidoAttribute()
        {
            ErrorMessage = "CPF invalido.";
        }

        public override bool IsValid(object value)
        {
            return DocumentoUtils.CpfValido(value?.ToString());
        }
    }
}
