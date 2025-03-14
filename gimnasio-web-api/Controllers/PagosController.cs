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
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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

    [HttpGet("{year}/{month}/{day}")]
    public async Task<IActionResult> GetPagosByMonthYearAndDay(int year, int month, int day)
    {
        try
        {
            var pagos = await _context.Pagos
                .Include(p => p.Usuario)
                .Where(p => p.FechaPago.Year == year && p.FechaPago.Month == month && p.FechaPago.Day == day)
                .Select(p => new
                {
                    p.CodigoPago,
                    Usuario = new
                    {
                        NombreCompleto = p.Usuario != null ? p.Usuario.Nombres + " " + p.Usuario.Apellidos : "Desconocido"
                    },
                    p.CodigoUsuario,
                    p.MesesPagados,
                    p.MesesPagadosA,
                    p.FechaPago,
                    p.Monto,
                    p.DetallePago,
                    p.IntervaloPago
                })
                .ToListAsync();

            if (pagos == null || pagos.Count == 0)
            {
                return NotFound(new { message = "No se encontraron pagos para esta fecha." });
            }

            return Ok(pagos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
        }
    }
    [HttpGet("resumen-pagos/años")]
    public async Task<IActionResult> GetAñosConPagos()
    {
        try
        {
            var añosConPagos = await _context.Pagos
                .GroupBy(p => p.FechaPago.Year)
                .Select(g => new 
                {
                    Año = g.Key,
                    PagosRealizados = g.Count(),
                    TotalGanancias = g.Sum(p => p.Monto)
                })
                .OrderBy(r => r.Año)
                .ToListAsync();

            if (añosConPagos == null || añosConPagos.Count == 0)
            {
                return NotFound(new { message = "No se encontraron pagos registrados." });
            }

            return Ok(añosConPagos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
        }
    }
    [HttpGet("resumen-pagos/{year}/meses")]
    public async Task<IActionResult> GetMesesConPagos(int year)
    {
        try
        {
            var mesesConPagos = await _context.Pagos
                .Where(p => p.FechaPago.Year == year)
                .GroupBy(p => p.FechaPago.Month)
                .Select(g => new 
                {
                    Año = g.First().FechaPago.Year,
                    Mes = g.Key,
                    PagosRealizados = g.Count(),
                    TotalGanancias = g.Sum(p => p.Monto)
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();

            if (mesesConPagos == null || mesesConPagos.Count == 0)
            {
                return NotFound(new { message = "No se encontraron pagos para el año especificado." });
            }

            return Ok(mesesConPagos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
        }
    }
    [HttpGet("resumen-pagos/{year}/{month}/dias")]
    public async Task<IActionResult> GetDiasConPagos(int year, int month)
    {
        try
        {
            var diasConPagos = await _context.Pagos
                .Where(p => p.FechaPago.Year == year && p.FechaPago.Month == month)
                .GroupBy(p => p.FechaPago.Day)
                .Select(g => new 
                {
                    Año = g.First().FechaPago.Year,
                    Mes = g.First().FechaPago.Month,
                    Dia = g.Key,
                    PagosRealizados = g.Count(),
                    TotalGanancias = g.Sum(p => p.Monto)
                })
                .OrderBy(r => r.Dia)
                .ToListAsync();

            if (diasConPagos == null || diasConPagos.Count == 0)
            {
                return NotFound(new { message = "No se encontraron pagos para la fecha especificada." });
            }

            return Ok(diasConPagos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
        }
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

            if (fechasUsuarios.Count == 1 && esEdicion)
            {
                return Ok(new { message = "El usuario no tiene un pago anterior." });
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
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Codigo == usuarioId);

            if (!usuarioExiste)
            {
                return NotFound($"El usuario con el ID {usuarioId} no existe.");
            }

            var fechasUsuario = await _context.Fechas_Usuarios
                .Where(f => f.UsuarioId == usuarioId && f.FechaPago == fechaPago)
                .FirstOrDefaultAsync();

            if (fechasUsuario == null)
            {
                _logger.LogWarning("No se encontró el registro de Fechas_Usuario con UsuarioId {UsuarioId} y FechaPago {FechaPago}", usuarioId, fechaPago);
                return NotFound(new { message = "Registro no encontrado en Fechas_Usuario." });
            }

            var ultimoPago = await _context.Pagos
                .Where(p => p.CodigoUsuario == usuarioId)
                .OrderByDescending(p => p.FechaPago)
                .FirstOrDefaultAsync();

            if (ultimoPago == null)
            {
                return NotFound($"No se encontraron pagos para el usuario con ID {usuarioId}.");
            }

            var pagoDto = new PagoDto(
                ultimoPago.CodigoPago,
                ultimoPago.CodigoUsuario,
                ultimoPago.MesesPagados,
                ultimoPago.MesesPagadosA,
                ultimoPago.FechaPago,
                ultimoPago.Monto,
                ultimoPago.DetallePago,
                ultimoPago.IntervaloPago
            );

            var resultado = new
            {
                FechasUsuario = fechasUsuario,
                UltimoPago = pagoDto
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener Fechas_Usuario y el último pago del usuario con ID {UsuarioId}: {errorMessage}", usuarioId, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpGet("CheckFechaUsuarioExist/{usuarioId}/{fechaPago}")]
    public async Task<IActionResult> CheckFechaUsuarioExist(int usuarioId, DateTime fechaPago)
    {
        try
        {
            var existeFechaUsuario = await _context.Fechas_Usuarios
                .AnyAsync(f => f.UsuarioId == usuarioId && f.FechaPago == fechaPago);

            if (existeFechaUsuario)
            {
                return Ok(new { exists = true });
            }
            else
            {
                return Ok(new { exists = false });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al verificar el registro de Fechas_Usuario: {errorMessage}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
    [HttpGet("ultimo-pago-usuario/{usuarioId}")]
    public async Task<IActionResult> GetUltimoPagoPorUsuario(int usuarioId)
    {
        try
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Codigo == usuarioId);
            
            if (usuario == null)
            {
                return NotFound(new { message = $"El usuario con ID {usuarioId} no existe." });
            }
            var ultimoPago = await _context.Pagos
                .Where(p => p.CodigoUsuario == usuarioId)
                .OrderByDescending(p => p.FechaPago)
                .FirstOrDefaultAsync();

            if (ultimoPago == null)
            {
                return NotFound(new { message = $"No se encontraron pagos para el usuario con ID {usuarioId}." });
            }

            var pagoDto = new PagoDto(
                ultimoPago.CodigoPago,
                ultimoPago.CodigoUsuario,
                ultimoPago.MesesPagados,
                ultimoPago.MesesPagadosA,
                ultimoPago.FechaPago,
                ultimoPago.Monto,
                ultimoPago.DetallePago,
                ultimoPago.IntervaloPago
            );

            return Ok(pagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener el último pago del usuario con ID {usuarioId}: {errorMessage}", usuarioId, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}