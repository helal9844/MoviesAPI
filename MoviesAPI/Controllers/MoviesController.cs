using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly UserManager<User> _userManger;
        private readonly IMoviesServices _moviesServices;
        private readonly IGenresServices _genresServices;
        private readonly IMapper _mapper;

        private long _postersize = 1048576;
        private List<string> _allowExtentions = new List<string> { ".jpg", ".png" };
        public MoviesController(IMoviesServices moviesServices,IGenresServices genresServices, IMapper mapper, UserManager<User> userManger)
        {
            _genresServices = genresServices;
            _moviesServices = moviesServices;
            _mapper = mapper;
            _userManger = userManger;
        }
        [HttpGet]
        [Authorize(policy:"User")]
        public async Task<IActionResult> GetAll()   
        {
            var movies = _moviesServices.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            User? user = await _userManger.GetUserAsync(User);
            return Ok(new
            {
                data,
                ID = user.Id,
            });
        }
        [HttpGet("{id}")]
        [Authorize(policy: "User")]
        public async Task<IActionResult> GetById(int id)
        {
            
            var movies = _moviesServices.GetById(id);
            if (movies == null)
                return BadRequest("No Movie Found");
            var dto = _mapper.Map<MovieDetailsDTO>(movies);
            User? user = await _userManger.GetUserAsync(User);

            return Ok(new 
            {
                dto,
                ID = user.Id,
                
            });
        }
        [HttpGet("GenreId")]
        [Authorize(policy: "User")]
        public async Task<IActionResult> GetByGenreId(int genreid)
        {
            var movies = _moviesServices.GetAll(genreid);
            var data = _mapper.Map<IEnumerable<MovieDetailsDTO>>(movies);
            User? user = await _userManger.GetUserAsync(User);

            return Ok(new
            { 
                data,
                ID = user.Id
            });
        }
        [HttpPost]
        [Authorize(policy: "AdminOnly")]
        public async Task<IActionResult> Add([FromForm] MovieDTO movieDTO)
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

            User? user = await _userManger.GetUserAsync(User);

            return Ok( new 
            {
                movie,
                ID = user.Id,
            });
        }

        [HttpPut("{id}")]
        [Authorize(policy: "AdminOnly")]
        public async Task<IActionResult> EditById(int id,[FromForm]MovieDTO movieDTO)
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
            User? user = await _userManger.GetUserAsync(User);

            return Ok( new
            { 
                movie,
                ID = user.Id,
            });
        }

        [HttpDelete("{id}")]
        [Authorize(policy: "AdminOnly")]
        public async Task<IActionResult> DeleteById(int id)
        {
            
            var movie = _moviesServices.GetById(id);
            if (movie == null)
                return BadRequest("No Movie Found");
            _moviesServices.Delete(movie);
            User? user = await _userManger.GetUserAsync(User);

            return Ok(new {
                Del = "Movie Deleted", 
                ID = user.Id,
            });
        }
    }
}
