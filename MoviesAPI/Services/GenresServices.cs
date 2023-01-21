using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class GenresServices : IGenresServices
    {
        private readonly AppDbcontext _context;
        public GenresServices(AppDbcontext context)
        {
            _context = context;
        }
        public Genre Add(Genre genre)
        {
            _context.Add(genre);
            _context.SaveChanges();
            return genre;
        }

        public Genre Delete(Genre genre)
        {
            _context.Remove( genre);
            _context.SaveChanges();
            return genre;
        }

        public Genre Update(Genre genre)
        {
            _context.Update(genre);
            _context.SaveChanges();
            return genre;
        }

        public IEnumerable<Genre> GetAll()
        {
            return _context.Genres.OrderBy(P => P.Name).ToList();
        }

        public Genre GetById(int id)
        {
            return _context.Genres.Single(p => p.ID == id);
        }

        public bool IsValidGenre(int id)
        {
            return _context.Genres.Any(P => P.ID == id);
        }
    }
}
