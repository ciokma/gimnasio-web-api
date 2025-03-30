using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Models;
using gimnasio_web_api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace gimnasio_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaRepository _asistenciaRepository;

        public AsistenciaController(IAsistenciaRepository asistenciaRepository)
        {
            _asistenciaRepository = asistenciaRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuarioInfo(int id)
        {
            var resultado = await _asistenciaRepository.ObtenerUsuarioInfoAsync(id);

            if (resultado == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(resultado);
        }

        [HttpGet("ultima-informacion-pago/{usuarioId}")]
        public async Task<IActionResult> ObtenerUltimaInformacionPago(int usuarioId)
        {
            var resultado = await _asistenciaRepository.ObtenerUltimaInformacionPagoAsync(usuarioId);

            if (resultado == null || (resultado.UltimaFechaUsuario == null && resultado.UltimoPago == null))
            {
                return NotFound("No se encontraron pagos para el usuario.");
            }

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult<Asistencia>> PostAsistencia(Asistencia asistencia)
        {
            await _asistenciaRepository.AddAsync(asistencia);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsistencia(int id, Asistencia asistencia)
        {
            if (id != asistencia.Codigo)
            {
                return BadRequest();
            }

            try
            {
                await _asistenciaRepository.UpdateAsync(asistencia);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return Conflict("Error de concurrencia al actualizar la asistencia");
            }

            return NoContent();
        }

        [HttpGet("resumen-asistencia/años")]
        public async Task<IActionResult> GetAñosConAsistencias()
        {
            var añosConAsistencias = await _asistenciaRepository.GetAñosConAsistenciasAsync();

            if (!añosConAsistencias.Any())
            {
                return NotFound(new { message = "No se encontraron asistencias registradas." });
            }

            return Ok(añosConAsistencias);
        }

        [HttpGet("resumen-asistencia/{year}/meses")]
        public async Task<IActionResult> GetMesesConAsistencias(int year)
        {
            var mesesConAsistencias = await _asistenciaRepository.GetMesesConAsistenciasAsync(year);

            if (!mesesConAsistencias.Any())
            {
                return NotFound(new { message = "No se encontraron asistencias para el año especificado." });
            }

            return Ok(mesesConAsistencias);
        }

        [HttpGet("resumen-asistencia/{year}/{month}/dias")]
        public async Task<IActionResult> GetDiasConAsistencias(int year, int month)
        {
            var diasConAsistencias = await _asistenciaRepository.GetDiasConAsistenciasAsync(year, month);

            if (!diasConAsistencias.Any())
            {
                return NotFound(new { message = "No se encontraron asistencias para la fecha especificada." });
            }

            return Ok(diasConAsistencias);
        }

        [HttpGet("fecha/{primerafecha}")]
        public async Task<ActionResult<IEnumerable<Asistencia>>> GetAsistenciaPorFecha([FromRoute] string primerafecha, [FromQuery] string? segundafecha = null)
        {
            if (!DateTime.TryParse(primerafecha, out DateTime primerafechaParsed)){
                return BadRequest("Formato de la fecha inválida en 'primerafecha'");
            }
            DateTime? segundafechaParsed = null;
            if (!string.IsNullOrEmpty(segundafecha)){
                if (!DateTime.TryParse(segundafecha, out DateTime segundafechaValue)){
                    return BadRequest("Formato de la fecha inválida en 'segundafecha'");
                }
                segundafechaParsed = segundafechaValue;
            }
            var asistencias = await _asistenciaRepository.GetAsistenciaPorFechaAsync(primerafechaParsed, segundafechaParsed);
            return asistencias.Any() ? Ok(asistencias) : NotFound("No se encontraron fechas en el rango especificado.");
        }
    }
}