using BibliotecaVirtual.BL;
using BibliotecaVirtual.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BibliotecaVirtual.AppUI.MVC.Controllers
{
    public class LibroController : Controller
    {
        public ActionResult ObtenerLibros() 
        {
            return Json(new LibroBL().Obtener(), JsonRequestBehavior.AllowGet);
        }

        public int Guardar(Libro pLibro)
        {
            return new LibroBL().Guardar(pLibro);
        }

        public int Modificar(Libro pLibro) 
        {
            return new LibroBL().Modificar(pLibro);
        }

        public int Eliminar(int idLibro) 
        {
            return new LibroBL().Eliminar(new Libro { Id = idLibro });
        }
    }
}