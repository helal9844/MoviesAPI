using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Models;
using MoviesAPI.Services;
using System.Configuration;
using System.Security.Claims;
using System.Text;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(name:"Con1"))
            );;
            builder.Services.AddDbContext<UsersContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(name: "Con1"))
            );;
            //ASP identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);

            })
                //From Which context 
                .AddEntityFrameworkStores<UsersContext>();
                


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddTransient<IGenresServices, GenresServices>();
            builder.Services.AddTransient<IMoviesServices, MoviesServices>();

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(name: "v1", info: new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MovieAPI",
                    Description = "API",
                    TermsOfService = new Uri(uriString: "https://www.google.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "Ahmed",
                        Email = "AHmed@dom,com",
                        Url = new Uri(uriString: "https://www.google.com"),

                    },
                    License = new OpenApiLicense
                    {
                        Name = "A year",
                        Url = new Uri(uriString: "https://www.google.com"),

                    },
                });
                options.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme 
                { 
                    Name = "Autherization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Key"

                });
                options.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            #region Default JWT
            /*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {     
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer =true,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };


                }) ;*/
            #endregion

            builder.Services.AddAuthentication(options =>
            {
                //To avoid override authentecation identity
                options.DefaultAuthenticateScheme = "AuthenticationSchema";
                options.DefaultChallengeScheme = "AuthenticationSchema";
            })
                .AddJwtBearer("AuthenticationSchema",options => 
                {
                    var KeyFromConfig = builder.Configuration.GetValue<string>("SecretKey");
                    var KeyInBytes = Encoding.ASCII.GetBytes(KeyFromConfig);
                    var secretkey = new SymmetricSecurityKey(KeyInBytes);
                    var signingcredentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256Signature);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = secretkey,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });



            builder.Services.AddAuthorization(options => 
            {
                options.AddPolicy("AdminOnly",
                    policy => policy
                    .RequireClaim(ClaimTypes.Role, "Admin", "CEO")
                    .RequireClaim(ClaimTypes.NameIdentifier)
                    );
                options.AddPolicy("User",
                    policy => policy
                    .RequireClaim(ClaimTypes.Role, "User", "Admin")
                    .RequireClaim(ClaimTypes.NameIdentifier)
                        ); 

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(C => C.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}