using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gimnasio_web_api.DTOs
{
    public class UsuarioDto
    {
        public required int Codigo { get; set; }
        public required string Nombres { get; set; }
        public string? Apellidos { get; set; }
        [StringLength(15)]
        public string? Telefono { get; set; }

        public string? Foto { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? FechaIngreso { get; set; }
        public List<PagoDto>? Pago { get; set; }
    }
}