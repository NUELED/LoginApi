using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class ResetPasswordRequest
    {

        [Required]
        public string Token { get; set; } = string.Empty;   
        [Required, MinLength(6, ErrorMessage = "Please enter @least 6 characters dude!")]
        public string Password { get; set; } = String.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = String.Empty;
    }
}
