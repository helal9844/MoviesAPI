using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresServices _genresServices;
        public GenresController(IGenresServices genresServices)
        {
            _genresServices = genresServices;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var genres = _genresServices.GetAll();
            if (genres == null)
                return BadRequest();
            return Ok(genres);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id == null)
                return BadRequest();
            var genres = _genresServices.GetById(id);
            if (genres == null)
                return NotFound();

            return Ok(genres);
        }
        [HttpPost]
        public IActionResult Add(AddGenreDTO addGenreDTO)
        {
            var genre = new Genre { Name = addGenreDTO.Name };

            _genresServices.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id}")]
        public IActionResult EditById(int id, [FromBody]AddGenreDTO addGenreDTO)
        {
            var genre = _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre for id : {id}");
            genre.Name = addGenreDTO.Name;
            _genresServices.Update(genre);
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var genre = _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre for id : {id}");
            _genresServices.Delete(genre);
            return Ok($"Genre for Id {id} Deleted");
        }
    }
}
