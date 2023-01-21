using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class RegisterCredentials
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        [EmailAddress]
        public string Email { get; set; } = "";  

        public string Section { get; set; } = "";

    }
}
