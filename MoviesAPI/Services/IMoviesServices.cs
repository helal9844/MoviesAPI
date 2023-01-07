using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public interface IMoviesServices
    {
        IEnumerable<Movie> GetAll(int genreId = 0);
        Movie GetById(int id);
        Movie Add(Movie movie); 
        Movie Update(Movie movie);
        Movie Delete(Movie movie);
    }
}
