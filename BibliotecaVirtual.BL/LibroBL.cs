using BibliotecaVirtual.DAL;
using BibliotecaVirtual.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.BL
{
    public class LibroBL
    {
        public int Guardar(Libro pLibro)
        {
            return LibroDAL.Guardar(pLibro);
        }
        public int Modificar(Libro pLibro)
        {
            return LibroDAL.Modificar(pLibro);
        }
        public int Eliminar(Libro pLibro)
        {
            pLibro.Estado = 0;
            return LibroDAL.Eliminar(pLibro);
        }
        public List<Libro> Buscar(Libro pLibro)
        {
            pLibro.Estado = 1;
            return LibroDAL.Buscar(pLibro);
        }
        public Libro ObtenerPorId(Libro pLibro)
        {
            return LibroDAL.ObtenerPorId(pLibro);
        }
        public List<Libro> Obtener()
        {
            return LibroDAL.Obtener();
        }
    }
}
