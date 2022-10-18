using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.EN
{
    public class DetalleCompraLibro
    {
        public int Id { get; set; }
        public int IdCompraLibro { get; set; }
        public int IdLibro { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal SubTotal { get; set; }
    }
}
