using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.EN
{
    public class Libro
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Editorial { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int Stock { get; set; }
        public byte Estado { get; set; }
        public int TopRegistro { get; set; }
    }
}
