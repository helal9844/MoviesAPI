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
        private readonly UserManager<User> _userManager;

        public GenresController(IGenresServices genresServices,UserManager<User> userManager)
        {
            _genresServices = genresServices;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize("User")]
        public async Task<IActionResult> GetAll()
        {
            var genres = _genresServices.GetAll();
            if (genres == null)
                return BadRequest();
            User? user = await _userManager.GetUserAsync(User);

            return Ok(new 
            { 
                
                genres ,
                ID= user.Id, 
            
            });
        }
        [HttpGet("{id}")]
        [Authorize("User")]
        public async Task<IActionResult> GetById(int id)
        {
            
            var genres = _genresServices.GetById(id);
            if (genres == null)
                return NotFound();
            User? user = await _userManager.GetUserAsync(User);

            return Ok(new
            {
                genres,
                ID = user.Id   
            });
        }
        [HttpPost]
        [Authorize("AdminOnly")]
        public async Task<IActionResult> Add(AddGenreDTO addGenreDTO)
        {
            var genre = new Genre { Name = addGenreDTO.Name };

            _genresServices.Add(genre);
            User? user = await _userManager.GetUserAsync(User);

            return Ok( new 
            {
                genre,
                ID = user.Id,
            });
        }
        [HttpPut("{id}")]
        [Authorize("AdminOnly")]
        public async Task<IActionResult> EditById(int id, [FromBody]AddGenreDTO addGenreDTO)
        {
            var genre = _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre for id : {id}");
            genre.Name = addGenreDTO.Name;
            _genresServices.Update(genre);
            User? user = await _userManager.GetUserAsync(User);

            return Ok( new
            {
                genre,
                ID = user.Id,
            });
        }
        [HttpDelete("{id}")]
        [Authorize("AdminOnly")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var genre = _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre for id : {id}");
            _genresServices.Delete(genre);
            User? user = await _userManager.GetUserAsync(User);

            return Ok(new 
            {
                str =$"Genre for Id {id} Deleted",
                ID = user.Id,
            });
        }
    }
}
