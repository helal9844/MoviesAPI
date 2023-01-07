using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public interface IGenresServices
    {
        IEnumerable<Genre> GetAll();
        Genre GetById(int id);
        Genre Add(Genre genre);
        Genre Update(Genre genre);
        Genre Delete(Genre genre);
        bool IsValidGenre(int id);      
    }
}
