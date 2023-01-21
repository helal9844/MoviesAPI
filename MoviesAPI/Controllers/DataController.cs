using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;
using System.Security.Claims;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        public readonly UserManager<User> _usermanager;

        public DataController(UserManager<User> usermanager)
        {
            _usermanager = usermanager;
        }

        
        [HttpGet]
        [Authorize]
        [Route("secure-data")]
        public ActionResult GetSecureData()
        {
            return Ok(new
            {
                Name = "Ahmed Tarek"
            });
        }
        [HttpGet]
        [Authorize(policy:"AdminOnly")]
        [Route("secure-management-data")]
        public ActionResult GetDataFromManager()
        {
            return Ok(new
            {
                Data = "Secured Management Data"
            });
        }
        [HttpGet]
        [Authorize]
        [Route("user-details")]
        public async Task<ActionResult> GetLogedInUserDetails()
        {
            /*var nameIdClaims = User.Claims.First(C => C.Type == ClaimTypes.NameIdentifier);
            var userId = nameIdClaims.Value;
            var user = await _usermanager.FindByIdAsync(userId);*/
           
            //Token To C# Object (User)
            User? user = await _usermanager.GetUserAsync(User);
            return Ok(new
            {
                Id = user.Id,
                Data = user.Section
            }); ;
        }
    }
}
