using System;
using System.Collections.Generic;

public class PagoDto
{
    public required int CodigoPago { get; set; }
    public required int CodigoUsuario { get; set; }
    public required int MesesPagados { get; set; }
    public required int MesesPagadosA { get; set; }
    public required DateTime FechaPago { get; set; }
    public required decimal Monto { get; set; }
    public required string DetallePago { get; set; } = string.Empty;
    public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();
}