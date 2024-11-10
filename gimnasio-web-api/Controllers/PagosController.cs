using Microsoft.AspNetCore.Mvc;
using gimnasio_web_api.Models;
using gimnasio_web_api.DTOs;
using gimnasio_web_api.Repositories;
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

    public PagosController(IRepository<Pago, int> repository, IRepository<Usuarios, int> usuariosRepository)
    {
        _repository = repository;
        _usuariosRepository = usuariosRepository;
    }

    [HttpGet]
    public async Task<List<PagoDto>> GetPagos()
    {
        var pagos = await _repository.GetAllAsync();
        return pagos.Select(p => new PagoDto
        {
            CodigoPago = p.CodigoPago,
            CodigoUsuario = p.CodigoUsuario,
            MesesPagados = p.MesesPagados,
            MesesPagadosA = p.MesesPagadosA,
            FechaPago = p.FechaPago,
            Monto = p.Monto,
            DetallePago = p.DetallePago
        }).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PagoDto>> GetPago(int id)
    {
        var pago = await _repository.GetByIdAsync(id);
        if (pago == null)
        {
            return NotFound();
        }

        var usuarios = await _usuariosRepository.GetAllAsync();
        var usuariosDto = usuarios.Select(u => new UsuarioDto
        {
            Codigo = u.Codigo,
            Nombres = u.Nombres
        }).ToList();

        var pagoDto = new PagoDto
        {
            CodigoPago = pago.CodigoPago,
            CodigoUsuario = pago.CodigoUsuario,
            MesesPagados = pago.MesesPagados,
            MesesPagadosA = pago.MesesPagadosA,
            FechaPago = pago.FechaPago,
            Monto = pago.Monto,
            DetallePago = pago.DetallePago,
            Usuarios = usuariosDto
        };

        return pagoDto;
    }

    [HttpPost]
    public async Task<ActionResult<PagoDto>> PostPago([FromBody] PagoDto pagoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pago = new Pago
        {
            CodigoUsuario = pagoDto.CodigoUsuario,
            MesesPagados = pagoDto.MesesPagados,
            MesesPagadosA = pagoDto.MesesPagadosA,
            FechaPago = pagoDto.FechaPago,
            Monto = pagoDto.Monto,
            DetallePago = pagoDto.DetallePago
        };

        await _repository.AddAsync(pago);
        return CreatedAtAction("GetPago", new { id = pago.CodigoPago }, pagoDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPago(int id, [FromBody] PagoDto pagoDto)
    {
        if (id != pagoDto.CodigoPago)
        {
            return BadRequest($"El ID del pago no coincide: {id} se esperaba {pagoDto.CodigoPago}.");
        }

        var existingPago = await _repository.GetByIdAsync(id);
        if (existingPago == null)
        {
            return NotFound();
        }

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
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el pago: {ex.Message}");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePago(int id)
    {
        var pago = await _repository.GetByIdAsync(id);
        if (pago == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(pago.CodigoPago);
        return Ok();
    }
    [HttpGet("usuarios")]
    public async Task<ActionResult<List<UsuarioDto>>> GetUsuarios()
    {
        var usuarios = await _usuariosRepository.GetAllAsync();
        var usuariosDto = usuarios.Select(u => new UsuarioDto
        {
            Codigo = u.Codigo,
            Nombres = u.Nombres,
            Apellidos = u.Apellidos
        }).ToList();

        return Ok(usuariosDto);
    }
}