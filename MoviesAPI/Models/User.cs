using Microsoft.AspNetCore.Identity;

namespace MoviesAPI.Models
{
    public class User:IdentityUser
    {
        public string Section { get; set; } = "";

    }
}
