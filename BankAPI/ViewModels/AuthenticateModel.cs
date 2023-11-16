using System.ComponentModel.DataAnnotations;

namespace BankAPI.ViewModels
{
    public class AuthenticateModel
    {
        [Required(ErrorMessage = "Account Number is Required")]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$", ErrorMessage = "Account Number must be 10-digit.")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Pin is Required")]
        public string Pin { get; set; }
    }
}
