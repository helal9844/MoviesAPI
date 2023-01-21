using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresServices _genresServices;
        private readonly UserManager<User> userManager;

        public GenresController(IGenresServices genresServices,UserManager<User> userManager)
        {
            _genresServices = genresServices;
            this.userManager = userManager;
        }
        [HttpGet]
        [Authorize("User")]
        public IActionResult GetAll()
        {
            var genres = _genresServices.GetAll();
            if (genres == null)
                return BadRequest();
            return Ok(genres);
        }
        [HttpGet("{id}")]
        [Authorize("User")]
        public IActionResult GetById(int id)
        {
            
            var genres = _genresServices.GetById(id);
            if (genres == null)
                return NotFound();

            return Ok(genres);
        }
        [HttpPost]
        [Authorize("AdminOnly")]
        public IActionResult Add(AddGenreDTO addGenreDTO)
        {
            var genre = new Genre { Name = addGenreDTO.Name };

            _genresServices.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id}")]
        [Authorize("AdminOnly")]
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
        [Authorize("AdminOnly")]
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
