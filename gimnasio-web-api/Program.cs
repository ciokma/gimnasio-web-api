using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using gimnasio_web_api.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using gimnasio_web_api.Repositories;
using gimnasio_web_api.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

namespace gimnasio_web_api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:5211");
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseInMemoryDatabase("GimnasioInMemoryDb"));

            var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 41)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalNetwork",
                    policy =>
                    {
                        policy.SetIsOriginAllowed(origin =>
                        {
                            return origin.StartsWith("http://192.168.") ||
                                origin.StartsWith("http://10.") ||
                                origin.StartsWith("http://172.") ||
                                origin.StartsWith("http://localhost");
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            /*builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                        //policy.WithOrigins("http://192.168.1.14:3000")
                        //policy.WithOrigins("http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });*/

            //Inyectando dependencias
            builder.Services.AddScoped<IRepository<Usuarios, int>, UsuarioRepository>();
            builder.Services.AddScoped<IRepository<Producto, int>, ProductoRepository>();
            builder.Services.AddScoped<IRepository<Fechas_Usuario, int>, Fechas_UsuarioRepository>();
            builder.Services.AddScoped<IRepository<Tipo_Ejercicio, int>, Tipo_EjercicioRepository>();
            builder.Services.AddScoped<IRepository<Tipo_Pagos, string>, Tipo_PagoRepository>();
            builder.Services.AddScoped<IRepository<Pago, int>, PagoRepository>();
            builder.Services.AddScoped<IRepository<Mensaje, int>, MensajeRepository>();
            builder.Services.AddScoped<IVentaRepository, VentaRepository>();
            builder.Services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();

            Log.Logger = new LoggerConfiguration()
                //.WriteTo.Console()
                .WriteTo.File("Logs/myapp.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.AddSerilog();

            // JsonWebToken 8.0.0 Autenticacion
            #region JsonWebToken
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "gymsys.com",
                        ValidAudience = "gymsys.com",
                        //Cambiar luego a appconfig
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue("JsonWebTokenSecret", "")))
                    };
                });

            builder.Services.AddAuthorization();

            #endregion JsonWebToken
            var app = builder.Build();
            app.UseCors("AllowLocalNetwork");
            app.UseAuthentication();
            app.UseAuthorization();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gimnasio API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();

            var summaries = new[]
            {
                        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                    };

            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}