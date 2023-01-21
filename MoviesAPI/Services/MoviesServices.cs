using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class MoviesServices : IMoviesServices   
    {
        private readonly AppDbcontext _context;
        private long _postersize = 1048576;
        private List<string> _allowExtentions = new List<string> { ".jpg", ".png" };
        public MoviesServices(AppDbcontext context)
        {
            _context = context;
        }
        public Movie Add(Movie movie)
        {
            _context.Add(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public IEnumerable<Movie> GetAll(int genreId = 0)
        {
            return _context.Movies
                .Where(p => p.GenreId == genreId || genreId == 0)
                .Include(P => P.Genre)
                .OrderByDescending(p => p.Rate)
                .ToList();
        }

        public Movie GetById(int id)
        {
            return _context.Movies.Include(p => p.Genre).Single(p => p.ID == id);
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}
