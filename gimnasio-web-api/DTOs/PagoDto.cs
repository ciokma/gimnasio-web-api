using System;

namespace gimnasio_web_api.DTOs
{
    public class PagoDto
    {
        public int CodigoPago { get; set; }
        public int CodigoUsuario { get; set; }
        public int MesesPagados { get; set; }
        public int MesesPagadosA { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string DetallePago { get; set; } = string.Empty;
        public bool IntervaloPago { get; set; } = false;
        public PagoDto(int codigoPago, int codigoUsuario, int mesesPagados, int mesesPagadosA, DateTime fechaPago, decimal monto, string detallePago, bool intervalopago)
        {
            CodigoPago = codigoPago;
            CodigoUsuario = codigoUsuario;
            MesesPagados = mesesPagados;
            MesesPagadosA = mesesPagadosA;
            FechaPago = fechaPago;
            Monto = monto;
            DetallePago = detallePago;
            IntervaloPago = intervalopago;
        }
    }
}