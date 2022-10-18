using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.DBManager
{
    public class ComunDBManager
    {
        static string _StrConnection = "";
        static string _StrProvider = "";
        private static string StrConnection
        {
            get {
                if (_StrConnection == "")
                {
                    _StrConnection = MConfig.ConnectionString();
                }
                return _StrConnection;
            }
        }
        private static string StrProvider
        {
            get {
                if (_StrProvider == "")
                {
                    _StrProvider = MConfig.Provider();
                }
                return _StrProvider;
            }
        }
        public static IDbConnection ObtenerConexion() 
        {
            switch (StrProvider)
            {
                case "System.Data.SqlClient":
                        return new SqlConnection(StrConnection);
            }
            return null;
        }
        private static IDbCommand Comando(string pStrConsulta, List<Parametro> pParametros)
        {
            switch (StrProvider)
            {
                case "System.Data.SqlClient":
                    {
                        var _comando = new SqlCommand(pStrConsulta);
                        foreach (var item in pParametros)
                        {
                            _comando.Parameters.AddWithValue(item.Name, item.Objeto);
                        }
                        return _comando;
                    }
            }
            return null;
        }
        public static int EjecutarComando(List<Transaccion> pTransaciones)
        {
            int resultado = 0;
            using (var _conn = ObtenerConexion())
            {
                var _conexionExistosa = true;
                try
                {
                    _conn.Open();
                }
                catch
                {
                    _conexionExistosa = false;
                }
                if (_conexionExistosa)
                {
                    var _transacion = _conn.BeginTransaction();
                    var _consultaActual = "";
                    try
                    {
                        foreach (var item in pTransaciones)
                        {
                            _consultaActual = item.devSqlParams;
                            var _comando = Comando(item.Consulta, item.Parametros);
                            _comando.Connection = _conn;
                            _comando.Transaction = _transacion;
                            resultado = _comando.ExecuteNonQuery();
                        }
                        if (pTransaciones.Count > 0)
                        {
                            _transacion.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        _transacion.Rollback();
                        resultado = 0;
                    }
                }
            }
            return resultado;
        }
        public static IDataReader Reader(Transaccion pTransacion)
        {
            IDataReader _reader = null;
            //validacion de repetidos
            if (pTransacion.IsRepetido == 1)
            {
                pTransacion.Consulta = pTransacion.Consulta.Replace("Like", "=");
                foreach (var item in pTransacion.Parametros)
                {
                    var _string = item.Objeto as string;
                    if (_string != null)
                    {
                        item.Objeto = _string.Replace("%", "");
                    }
                }
            }
            //***************************
            // Limitar los datos de la Consulta
            var _sql = pTransacion.Consulta;
            pTransacion.Consulta = SQLtoGetLIMIT(_sql, pTransacion.Top);
            //********************************
            //var _comando = Comando(pTransacion.Consulta, pTransacion.Parametros);
            try
            {
                using (var _comando = Comando(pTransacion.Consulta, pTransacion.Parametros))
                {
                    _comando.Connection = ObtenerConexion();
                    _comando.Connection.Open();
                    _reader = _comando.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception ex)
            {
                //Manejar excepciones
            }
            return _reader;
        }
        //*******************************************************
        public static string SQLGetLastId(string pTable, string pCampo)
        {
            switch (StrProvider)
            {
                case "System.Data.SqlClient":
                        return string.Format(" SELECT TOP 1 {1} FROM {0} ORDER BY {1} DESC ", pTable, pCampo);
            }
            return "";
        }
        public static string SQLtoGetLIMIT(string pSQl, int pLimit = 0)
        {
            var _limite = 500;
            if (pLimit > 0)
                _limite = pLimit;
            switch (StrProvider)
            {
                case "System.Data.SqlClient":
                    {
                        var _stringConsulta = pSQl.Trim().Split(' ');
                        var _strSQL = "";
                        foreach (var item in _stringConsulta)
                        {
                            if (item.ToUpper() == "SELECT")
                                _strSQL += item + " TOP " + _limite + " ";
                            else if (item.ToLower().Trim() != "distinct")
                                _strSQL += " " + item;
                        }
                        if (pSQl.ToLower().Contains("distinct"))
                            _strSQL = _strSQL.Replace("select", "select distinct");
                        return _strSQL;
                    }
            }
            return "";
        }
    }
}
