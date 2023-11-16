using BankAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.ViewModels
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public AccountType AccountType { get; set; }
        [Required(ErrorMessage = "Card Number is Required")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin must not be more than 4 digits")]
        public string Pin { get; set; }
        [Required]
        [Compare("Pin", ErrorMessage = "Pins do not match")]
        public string ConfirmPin { get; set; }
    }
}
