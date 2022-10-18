using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibliotecaVirtual.DBManager
{
    public class Transaccion
    {
        public string Consulta { get; set; }
        public List<Parametro> Parametros { get; set; }
        public short IsRepetido { get; set; }
        // Tope de registros a devolver a darle en todos
        public int Top { get; set; }
        public Transaccion(string pConsulta, List<Parametro> pParametros)
        {
            Consulta = pConsulta;
            Parametros = pParametros;
        }
        public Transaccion()
        {
            Parametros = new List<Parametro>();
        }
        private string _tmpSqlParams;
        public string devSqlParams
        {
            get {
                if (string.IsNullOrWhiteSpace(this._tmpSqlParams))
                {
                    Parametros = Parametros.OrderByDescending(n => n.Name).ToList();//ordenar descendente para obtener el nombre mas largo al principio
                    _tmpSqlParams = Consulta;
                    foreach (var item in Parametros)
                    {
                        if (item.Objeto is string)
                            _tmpSqlParams = ReplaceIgnoreCase(_tmpSqlParams, ("@" + item.Name), ("'" + item.Objeto.ToString()) + "'");
                        else if (item.Objeto is DateTime)
                            _tmpSqlParams = ReplaceIgnoreCase(_tmpSqlParams, ("@" + item.Name), ("'" + Convert.ToDateTime(item.Objeto).ToString("yyyy-MM-dd HH:mm:ss")) + "'");
                        else if (item.Objeto is byte[])
                            _tmpSqlParams = ReplaceIgnoreCase(_tmpSqlParams, ("@" + item.Name), "0x");
                        else if (item.Objeto == null)
                            _tmpSqlParams = ReplaceIgnoreCase(_tmpSqlParams, ("@" + item.Name), "null");
                        else
                            _tmpSqlParams = ReplaceIgnoreCase(_tmpSqlParams, ("@" + item.Name), item.Objeto.ToString());
                    }
                }
                return _tmpSqlParams;
            }
        }
        protected string ReplaceIgnoreCase(string str, string srch, string rep)
        {
            try { str = Regex.Replace(str, srch, rep, System.Text.RegularExpressions.RegexOptions.IgnoreCase); }
            catch (Exception ex) { str = ""; }
            return str;
        }
    }
}
