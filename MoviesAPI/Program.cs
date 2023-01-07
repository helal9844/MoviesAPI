using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Configuration;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(C => C.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}