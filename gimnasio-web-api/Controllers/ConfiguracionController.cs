using gimnasio_web_api.DTOs;
using gimnasio_web_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gimnasio_web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ConfiguracionService _configuracionService;
        private readonly ILogger<ConfiguracionController> _logger;

        public ConfiguracionController(ConfiguracionService configuracionService, ILogger<ConfiguracionController> logger)
        {
            _configuracionService = configuracionService;
            _logger = logger;
        }

        [HttpPost("ejecutar")]
        public async Task<IActionResult> EjecutarConfiguraciones()
        {
            _logger.LogInformation("Inicio ejecución manual de configuraciones.");
            await _configuracionService.EjecutarConfiguracionesAsync();
            _logger.LogInformation("Ejecución manual de configuraciones finalizada.");
            return Ok(new { mensaje = "Configuraciones ejecutadas correctamente." });
        }

        [HttpGet("usuarios-inactivos")]
        public async Task<ActionResult<List<UsuariosInactivosDto>>> ObtenerUsuariosInactivos()
        {
            var usuarios = await _configuracionService.ObtenerUsuariosInactivosAsync();
            return Ok(usuarios);
        }
    }
}