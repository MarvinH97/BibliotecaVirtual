using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.EN
{
    public class ReservaLibro
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdBibliotecario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaDevolucion { get; set; }
    }
}
