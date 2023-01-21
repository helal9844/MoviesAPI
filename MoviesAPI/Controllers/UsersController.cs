using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //Get From Gonfigration
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        public UsersController(IConfiguration configuration,UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager; 
        }
        [HttpPost]
        [Route("static-login")]
        public ActionResult<TokenDTO> StaticLogin(LoginCredentionals input)
        {
            
            //Check Authrize    
            if (input.UserName != "Admin" || input.Password != "Password")
                return Unauthorized();
            //Claims
            var userclaims = new List<Claim>
            {
                new Claim("Name","Ahmed"),
                new Claim("Nationality","Egyption"),
                new Claim(ClaimTypes.GivenName,"Helal"),

            };
            //SecretKey
            //get key from config
            var KeyFromConfig = _configuration.GetValue<string>("SecretKey");
            //convert to bytes ascii(bytes[])
            var KeyInBytes = Encoding.ASCII.GetBytes(KeyFromConfig);
            //secret key in symmetric 
            var secretkey = new SymmetricSecurityKey(KeyInBytes);

            //choose hashing algorithm
            var signingcredentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256Signature);
            // using System.IdentityModel.Tokens.Jwt;
            //object token
            var jwt = new JwtSecurityToken(
                    //claims
                    claims: userclaims,
                    expires: DateTime.Now.AddMinutes(15),
                    notBefore: DateTime.Now,
                    //algorithms and secret key singingcredentials
                    signingCredentials: signingcredentials

                );
            //create token
            var tokenhandler = new JwtSecurityTokenHandler();

            return new TokenDTO
            {
                Token = tokenhandler.WriteToken(jwt)
            };
            
            
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> RegisterAdmin (RegisterCredentials input)
        {
            var user = new User
            {
                UserName =input.UserName,
                Section = input.Section,
                Email = input.Email
            };
           //to hash passwword and save to database
           var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var calims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),// Recommended for user manager 
                new Claim(ClaimTypes.Role,"Admin")

            };
            await _userManager.AddClaimsAsync(user, calims);
            return NoContent();
        }

        [HttpPost]
        [Route("register-user")]
        public async Task<ActionResult> RegisterUser(RegisterCredentials input)
        {
            var user = new User
            {
                UserName = input.UserName,
                Section = input.Section,
                Email = input.Email
            };
            //to hash passwword and save to database
            var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var calims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),// Recommended for user manager 
                new Claim(ClaimTypes.Role,"User")

            };
            await _userManager.AddClaimsAsync(user, calims);
            return NoContent();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<TokenDTO>> Login(LoginCredentionals input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            //Check Invalid UserName    
            if (user == null)
                return BadRequest("User Not Found");

            //Check if user is locked
            var islocked = await _userManager.IsLockedOutAsync(user);
            if (islocked)
                return BadRequest("You are Locked");

            var checkpassword = await _userManager.CheckPasswordAsync(user,input.Password);
            if (!checkpassword)
            {
                await _userManager.AccessFailedAsync(user);//Increase number of fail attempts
                return Unauthorized();
            }


            //Claims
            var userclaims = await _userManager.GetClaimsAsync(user);
            //SecretKey
            //get key from config
            var KeyFromConfig = _configuration.GetValue<string>("SecretKey");
            //convert to bytes ascii(bytes[])
            var KeyInBytes = Encoding.ASCII.GetBytes(KeyFromConfig);
            //secret key in symmetric 
            var secretkey = new SymmetricSecurityKey(KeyInBytes);

            //choose hashing algorithm
            var signingcredentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256Signature);
            // using System.IdentityModel.Tokens.Jwt;
            //object token
            var jwt = new JwtSecurityToken(
                    //claims
                    claims: userclaims,
                    expires: DateTime.Now.AddMinutes(15),
                    notBefore: DateTime.Now,
                    //algorithms and secret key singingcredentials
                    signingCredentials: signingcredentials

                );
            //create token
            var tokenhandler = new JwtSecurityTokenHandler();

            return new TokenDTO
            {
                Token = tokenhandler.WriteToken(jwt)
            };

        }
    }
}
