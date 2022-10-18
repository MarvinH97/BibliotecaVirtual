using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.DBManager
{
    public class Parametro
    {
        public string Name { get; set; }
        public object Objeto { get; set; }

        public Parametro(string pName, object pObjeto)
        {
            this.Name = pName;
            this.Objeto = pObjeto;
        }
        public Parametro() { }
    }
}
