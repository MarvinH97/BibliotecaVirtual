using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.EN
{
    public class CompraLibro
    {
        public int Id { get; set; }
        public int FechaCompra { get; set; }
        public int FechaFactura { get; set; }
        public int NumeroFactura { get; set; }
        public string Preveedor { get; set; }
        public decimal Monto { get; set; }
    }
}
