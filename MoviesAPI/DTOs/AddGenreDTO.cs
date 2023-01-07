using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class AddGenreDTO
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
