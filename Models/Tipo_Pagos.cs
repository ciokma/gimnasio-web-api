using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
[Table("tipo_pagos")]
public class Tipo_Pagos
{
    [Key]
    [JsonRequired]
    [StringLength(2)]
    public required string CodigoPago { get; set; }

    [JsonRequired]
    [StringLength(100)]
    public required string Descripcion { get; set; }

    [JsonRequired]
    [Range(0, double.MaxValue)]
    public decimal Monto { get; set; }
}
