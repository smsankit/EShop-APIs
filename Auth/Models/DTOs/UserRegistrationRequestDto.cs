using System.ComponentModel.DataAnnotations;

namespace Auth.Models.DTOs
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResult
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }

    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class UserRegistrationResponse : AuthResult
    {

    }

    public class UserLoginResponse : AuthResult
    {

    }


}
