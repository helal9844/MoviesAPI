using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Models;
using MoviesAPI.Services;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesServices _moviesServices;
        private readonly IGenresServices _genresServices;
        private readonly IMapper _mapper;

        private long _postersize = 1048576;
        private List<string> _allowExtentions = new List<string> { ".jpg", ".png" };
        public MoviesController(IMoviesServices moviesServices,IGenresServices genresServices, IMapper mapper)
        {
            _genresServices = genresServices;
            _moviesServices = moviesServices;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()   
        {
            var movies = _moviesServices.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            return Ok(data);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            
            var movies = _moviesServices.GetById(id);
            if (movies == null)
                return BadRequest("No Movie Found");
            var dto = _mapper.Map<MovieDetailsDTO>(movies); 

            return Ok(dto);
        }
        [HttpGet("GenreId")]
        public IActionResult GetByGenreId(int genreid)
        {
            var movies = _moviesServices.GetAll(genreid);
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult Add([FromForm] MovieDTO movieDTO)
        {
            if (movieDTO.Poster == null)
                return BadRequest("Poster Is Required");

            if (!_allowExtentions.Contains(Path.GetExtension(movieDTO.Poster.FileName).ToLower()))
                return BadRequest("File Must be PNG or JPG");

            if (movieDTO.Poster.Length > _postersize)
                return BadRequest("File Must be less than 1MB ");

            var checkGenre = _genresServices.IsValidGenre(movieDTO.GenreId);

            if (!checkGenre)
                return BadRequest("Invalid GenreId");

            using var datastream = new MemoryStream();
            movieDTO.Poster.CopyTo(datastream);


            var movie = _mapper.Map<Movie>(movieDTO);

            movie.Poster = datastream.ToArray();
            _moviesServices.Add(movie);
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public IActionResult EditById(int id,[FromForm]MovieDTO movieDTO)
        {
            
            
            var movie = _moviesServices.GetById(id);
            if (movie == null)
                return BadRequest("No Movie Found");
            var checkGenre = _genresServices.IsValidGenre(movieDTO.GenreId);

            if (!checkGenre)
                return BadRequest("Invalid GenreId");

            
            if (movieDTO.Poster != null)
            {
                if (!_allowExtentions.Contains(Path.GetExtension(movieDTO.Poster.FileName).ToLower()))
                    return BadRequest("File Must be PNG or JPG");

                if (movieDTO.Poster.Length > _postersize)
                    return BadRequest("File Must be less than 1MB ");
                using var datastream = new MemoryStream();
                movieDTO.Poster.CopyTo(datastream);
                movie.Poster = datastream.ToArray();
            }

            movie.Title = movieDTO.Title;
            movie.StoryLine = movieDTO.StoryLine;
            movie.Rate = movieDTO.Rate;
            movie.GenreId = movieDTO.GenreId;

            _moviesServices.Update(movie);
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            
            var movie = _moviesServices.GetById(id);
            if (movie == null)
                return BadRequest("No Movie Found");
            _moviesServices.Delete(movie);
            return Ok("Movie Deleted");
        }
    }
}
