using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.Data;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PagosController : ControllerBase
{
    private readonly IRepository<Pago, int> _repository;
    private readonly IRepository<Usuarios, int> _usuariosRepository;
    private readonly ILogger<PagosController> _logger;
    private readonly AppDbContext _context;
    public PagosController(IRepository<Pago, int> repository, IRepository<Usuarios, int> usuariosRepository, ILogger<PagosController> logger, AppDbContext context)
    {
        _repository = repository;
        _usuariosRepository = usuariosRepository;
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<List<PagoDto>> GetPagos()
    {
        var pagos = await _repository.GetAllAsync();
        return pagos.Select(p => new PagoDto(
            p.CodigoPago, 
            p.CodigoUsuario, 
            p.MesesPagados, 
            p.MesesPagadosA, 
            p.FechaPago, 
            p.Monto, 
            p.DetallePago,
            p.IntervaloPago 
        )).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PagoDto>> GetPago(int id)
    {
        var pago = await _repository.GetByIdAsync(id);
        if (pago == null)
        {
            return NotFound();
        }

        var pagoDto = new PagoDto(
            pago.CodigoPago, 
            pago.CodigoUsuario, 
            pago.MesesPagados, 
            pago.MesesPagadosA, 
            pago.FechaPago, 
            pago.Monto, 
            pago.DetallePago,
            pago.IntervaloPago 
        );

        return pagoDto;
    }

    [HttpPost]
    public async Task<ActionResult<PagoDto>> PostPago([FromBody] PagoDto pagoDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Modelo inválido recibido para el pago: {errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creación de pago. Código de usuario: {codigoUsuario}", pagoDto.CodigoUsuario);

        var usuario = await _usuariosRepository.GetByIdAsync(pagoDto.CodigoUsuario);
        if (usuario == null)
        {
            _logger.LogError("Usuario con código {codigoUsuario} no existe.", pagoDto.CodigoUsuario);
            return BadRequest("El usuario con el código proporcionado no existe.");
        }

        _logger.LogInformation("Usuario encontrado: {usuarioId}, {usuarioNombres}", usuario.Codigo, usuario.Nombres);

        var pagoExistente = await _context.Pagos
            .Where(p => p.CodigoUsuario == pagoDto.CodigoUsuario && p.FechaPago.Date == pagoDto.FechaPago.Date)
            .FirstOrDefaultAsync();

        if (pagoExistente != null)
        {
            _logger.LogWarning("El usuario {codigoUsuario} ya tiene un pago registrado para el día {fechaPago}", pagoDto.CodigoUsuario, pagoDto.FechaPago.Date);
            return BadRequest("El usuario ya tiene un pago registrado para el día seleccionado.");
        }
        pagoDto.MesesPagadosA = (pagoDto.MesesPagadosA == 0) ? 0 : pagoDto.MesesPagadosA;
        pagoDto.DetallePago = string.IsNullOrEmpty(pagoDto.DetallePago) ? "Sin comentario" : pagoDto.DetallePago;
        var pago = new Pago
        {
            CodigoUsuario = pagoDto.CodigoUsuario,
            MesesPagados = pagoDto.MesesPagados,
            MesesPagadosA = pagoDto.MesesPagadosA,
            FechaPago = pagoDto.FechaPago,
            Monto = pagoDto.Monto,
            DetallePago = pagoDto.DetallePago,
            IntervaloPago = pagoDto.IntervaloPago
        };

        try
        {
            _logger.LogInformation("Creando pago con código de usuario: {codigoUsuario}", pagoDto.CodigoUsuario);
            await _repository.AddAsync(pago);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al crear el pago para el usuario {codigoUsuario}: {exceptionMessage}", pagoDto.CodigoUsuario, ex.Message);
            return BadRequest($"Error al crear el pago: {ex.Message}");
        }

        _logger.LogInformation("Pago creado exitosamente para el usuario {codigoUsuario}. ID de pago: {codigoPago}", pagoDto.CodigoUsuario, pago.CodigoPago);
        return CreatedAtAction("GetPago", new { id = pago.CodigoPago }, pagoDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPago(int id, [FromBody] PagoDto pagoDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
            _logger.LogWarning("Errores de validación: {Errors}", string.Join(", ", errors));
            return BadRequest(ModelState);
        }
        if (id != pagoDto.CodigoPago)
        {
            _logger.LogWarning("El ID del pago no coincide: {id} se esperaba {codigoPago}", id, pagoDto.CodigoPago);
            return BadRequest($"El ID del pago no coincide: {id} se esperaba {pagoDto.CodigoPago}.");
        }

        var existingPago = await _repository.GetByIdAsync(id);
        if (existingPago == null)
        {
            _logger.LogWarning("Pago con ID {id} no encontrado para actualización", id);
            return NotFound();
        }

        var usuario = await _usuariosRepository.GetByIdAsync(pagoDto.CodigoUsuario);
        if (usuario == null)
        {
            _logger.LogError("El usuario con el código proporcionado no existe: {codigoUsuario}", pagoDto.CodigoUsuario);
            return BadRequest("El usuario con el código proporcionado no existe.");
        }

        _logger.LogInformation("Actualizando pago con ID {id} para el usuario {codigoUsuario}", id, pagoDto.CodigoUsuario);

        existingPago.CodigoUsuario = pagoDto.CodigoUsuario;
        existingPago.MesesPagados = pagoDto.MesesPagados;
        if (pagoDto.MesesPagadosA == 0)
        {
            existingPago.MesesPagadosA = 0;
        }
        else
        {
            existingPago.MesesPagadosA = pagoDto.MesesPagadosA;
        }
        existingPago.FechaPago = pagoDto.FechaPago;
        existingPago.Monto = pagoDto.Monto;
        existingPago.DetallePago = pagoDto.DetallePago;
        existingPago.IntervaloPago = pagoDto.IntervaloPago;

        try
        {
            await _repository.UpdateAsync(existingPago);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError("Error de concurrencia al actualizar el pago con ID {id}: {exceptionMessage}", id, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el pago: {ex.Message}");
        }

        _logger.LogInformation("Pago con ID {id} actualizado exitosamente", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePago(int id)
    {
        var pago = await _repository.GetByIdAsync(id);
        if (pago == null)
        {
            _logger.LogWarning("Pago con ID {id} no encontrado para eliminar", id);
            return NotFound();
        }

        await _repository.DeleteAsync(pago.CodigoPago);
        _logger.LogInformation("Pago con ID {id} eliminado exitosamente", id);
        return Ok();
    }
    [HttpPut("EditFechas_Usuario")]
    public async Task<IActionResult> PutFechasUsuario([FromBody] Fechas_Usuario fechasUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(fechasUsuario.UsuarioId);
        
        if (usuario == null)
        {
            ModelState.AddModelError("UsuarioId", "UsuarioId es requerido y debe ser un usuario existente.");
            return BadRequest(ModelState);
        }
        else
        {
            _logger.LogInformation("Usuario encontrado con ID {UsuarioId}", fechasUsuario.UsuarioId);
        }

        if (fechasUsuario.FechaPago > fechasUsuario.FechaVencimiento)
        {
            ModelState.AddModelError("FechaPago", "La fecha de pago no puede ser mayor que la fecha de vencimiento.");
            return BadRequest(ModelState);
        }
        else
        {
            _logger.LogInformation("Las fechas son válidas. FechaPago no es mayor que FechaVencimiento.");
        }

        var existingFechaUsuario = await _context.Fechas_Usuarios
            .Where(f => f.UsuarioId == fechasUsuario.UsuarioId && f.FechaPago == fechasUsuario.FechaPago)
            .FirstOrDefaultAsync();

        if (existingFechaUsuario == null)
        {
            _logger.LogWarning("No se encontró el registro de Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}", fechasUsuario.UsuarioId, fechasUsuario.FechaPago);
            return NotFound();
        }
        else
        {
            _logger.LogInformation("Registro de Fechas_Usuario encontrado. Procediendo con la actualización.");
        }

        existingFechaUsuario.FechaPagoA = fechasUsuario.FechaPagoA;
        existingFechaUsuario.FechaVencimiento = fechasUsuario.FechaVencimiento;

        try
        {
            _logger.LogInformation("Guardando los cambios en la base de datos...");
            _context.Entry(existingFechaUsuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingFechaUsuario);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al actualizar Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}: {errorMessage}", fechasUsuario.UsuarioId, fechasUsuario.FechaPago, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
        }
    [HttpDelete("DeleteFechas_Usuario")]
    public async Task<IActionResult> DeleteFechasUsuario([FromBody] Fechas_Usuario fechasUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(fechasUsuario.UsuarioId);
        
        if (usuario == null)
        {
            ModelState.AddModelError("UsuarioId", "UsuarioId es requerido y debe ser un usuario existente.");
            return BadRequest(ModelState);
        }

        var existingFechaUsuario = await _context.Fechas_Usuarios
            .Where(f => f.UsuarioId == fechasUsuario.UsuarioId && f.FechaPago == fechasUsuario.FechaPago)
            .FirstOrDefaultAsync();

        if (existingFechaUsuario == null)
        {
            _logger.LogWarning("No se encontró el registro de Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}", fechasUsuario.UsuarioId, fechasUsuario.FechaPago);
            return NotFound();
        }
        _context.Fechas_Usuarios.Remove(existingFechaUsuario);
        
        try
        {
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al eliminar Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}: {errorMessage}", fechasUsuario.UsuarioId, fechasUsuario.FechaPago, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpGet("ultimo-pago-vigente")]
    public async Task<ActionResult<object>> GetUltimoPagoVigente([FromQuery] int usuarioId, [FromQuery] bool esEdicion = false)
    {
        try
        {
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Codigo == usuarioId);

            if (!usuarioExiste)
            {
                return NotFound($"El usuario con el ID {usuarioId} no existe.");
            }

            var fechasUsuarios = await _context.Fechas_Usuarios
                .Where(f => f.UsuarioId == usuarioId)
                .OrderByDescending(f => f.FechaPago)
                .ToListAsync();

            if (fechasUsuarios == null || !fechasUsuarios.Any())
            {
                return NotFound($"No se encontraron pagos para el usuario con ID {usuarioId}.");
            }

            var pagoReferencia = esEdicion && fechasUsuarios.Count > 1 ? fechasUsuarios.Skip(1).First() : fechasUsuarios.First();

            if (pagoReferencia.FechaVencimiento == null)
            {
                return BadRequest("La fecha de vencimiento del último pago es nula.");
            }

            var diasRestantes = (pagoReferencia.FechaVencimiento.Value - DateTime.Now).Days;

            var resultado = new
            {
                usuarioId = pagoReferencia.UsuarioId,
                pagoReferencia.FechaPago,
                pagoReferencia.FechaVencimiento,
                diasRestantes
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpGet("GetFechas_Usuario")]
    public async Task<IActionResult> GetFechasUsuario([FromQuery] int usuarioId, [FromQuery] DateTime fechaPago)
    {
        try
        {
            var fechasUsuario = await _context.Fechas_Usuarios
                .Where(f => f.UsuarioId == usuarioId && f.FechaPago == fechaPago)
                .FirstOrDefaultAsync();

            if (fechasUsuario == null)
            {
                _logger.LogWarning("No se encontró el registro de Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}", usuarioId, fechaPago);
                return NotFound(new { message = "Registro no encontrado." });
            }
            
            return Ok(fechasUsuario);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}: {errorMessage}", usuarioId, fechaPago, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}