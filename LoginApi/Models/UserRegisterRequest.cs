using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class UserRegisterRequest
    {
        [Required,EmailAddress]
        public string Email { get; set; }  
        [Required,MinLength(6, ErrorMessage ="Please enter @least 6 characters dude!")]  
        public string Password { get; set; }
        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; }   
    }
}
