using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using gimnasio_web_api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdministradorController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Administrador>> GetAdministradores()
        {
            var administradores = _context.Administrador.ToList();

            if (!administradores.Any())
            {
                return NotFound("No se encontraron administradores.");
            }

            return Ok(administradores);
        }
    }
}