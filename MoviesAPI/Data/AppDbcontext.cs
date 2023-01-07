using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;

namespace MoviesAPI.Data
{
    public class AppDbcontext:DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options ):base(options) { }
        

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }    
    }
}
