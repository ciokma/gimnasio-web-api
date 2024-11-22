using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
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

    public PagosController(IRepository<Pago, int> repository, IRepository<Usuarios, int> usuariosRepository, ILogger<PagosController> logger)
    {
        _repository = repository;
        _usuariosRepository = usuariosRepository;
        _logger = logger;
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
            p.DetallePago
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
            pago.DetallePago
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

        var pago = new Pago
        {
            CodigoUsuario = pagoDto.CodigoUsuario,
            MesesPagados = pagoDto.MesesPagados,
            MesesPagadosA = pagoDto.MesesPagadosA,
            FechaPago = pagoDto.FechaPago,
            Monto = pagoDto.Monto,
            DetallePago = pagoDto.DetallePago
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
        existingPago.MesesPagadosA = pagoDto.MesesPagadosA;
        existingPago.FechaPago = pagoDto.FechaPago;
        existingPago.Monto = pagoDto.Monto;
        existingPago.DetallePago = pagoDto.DetallePago;

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
}