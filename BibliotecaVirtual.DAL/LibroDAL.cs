using BibliotecaVirtual.DBManager;
using BibliotecaVirtual.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaVirtual.DAL
{
    public class LibroDAL
    {
        public class Comun
        {
            public static void Insert<T>(Transaccion pTransacion, T pT, Func<T, object> pFuncFields, Dictionary<string, Func<string, object, string>> pExcepciones = null)
            {
                var xObjs = pFuncFields(pT);
                var xProps = xObjs.GetType().GetProperties();
                string xCampos = "";
                string xParametros = "";
                if (pExcepciones == null)
                    pExcepciones = new Dictionary<string, Func<string, object, string>>();
                if (xProps == null || xProps.Length == 0)
                    throw new ArgumentException("Error los campos son obligatorios", "");
                foreach (var item in xProps)
                {
                    var name = item.Name;
                    xCampos += xCampos.Trim().Length > 0 ? "," + name : name;
                    if (pExcepciones.ContainsKey(name) == false)
                    {
                        xParametros += xParametros.Trim().Length > 0 ? ",@" + item.Name : "@" + item.Name;
                        pTransacion.Parametros.Add(new Parametro { Name = item.Name, Objeto = xObjs.GetValuePropExt(item.Name) });
                    }
                    else
                        xParametros += pExcepciones[name](xParametros, xObjs.GetValuePropExt(item.Name));
                }
                pTransacion.Consulta = string.Format(pTransacion.Consulta, xCampos, xParametros);
            }
            public static void Update<T>(Transaccion pTransacion, T pT, Func<T, object> pFuncFields, Func<T, object> pFuncWhere, Dictionary<string, Func<string, object, string>> pExcepciones = null)
            {
                var xObjsCampos = pFuncFields(pT);
                var xPropsCampos = xObjsCampos.GetType().GetProperties();
                var xObjsWhere = pFuncWhere(pT);
                var xPropsWhere = xObjsWhere.GetType().GetProperties();
                if (pExcepciones == null)
                    pExcepciones = new Dictionary<string, Func<string, object, string>>();
                string xCampos = "";
                string xWhere = "";
                if (xPropsCampos == null || xPropsCampos.Length == 0)
                    throw new ArgumentException("Error los campos son obligatorios", "");
                foreach (var item in xPropsCampos)
                {
                    var name = item.Name;
                    if (pExcepciones.ContainsKey(name) == false)
                    {
                        var strCampoParametro = string.Format("{0}=@{0}", name);
                        xCampos += xCampos.Trim().Length > 0 ? "," + strCampoParametro : strCampoParametro;
                        pTransacion.Parametros.Add(new Parametro { Name = item.Name, Objeto = xObjsCampos.GetValuePropExt(item.Name) });
                    }
                    else
                    {
                        pExcepciones[name](xCampos, xObjsCampos.GetValuePropExt(item.Name));
                    }
                }
                if (xPropsWhere == null || xPropsWhere.Length == 0)
                    throw new ArgumentException("Error los campos where son obligatorios", "");
                foreach (var item in xPropsWhere)
                {
                    var name = item.Name;
                    var strCampoParametro = string.Format("{0}=@{0}", name);
                    xWhere += xWhere.Trim().Length > 0 ? " and " + strCampoParametro : strCampoParametro;
                    pTransacion.Parametros.Add(new Parametro { Name = item.Name, Objeto = xObjsWhere.GetValuePropExt(item.Name) });
                }
                pTransacion.Consulta = string.Format(pTransacion.Consulta, xCampos, xWhere);
            }
        }
        internal static Transaccion InsertTransa(Libro pLibro, Func<Libro, object> pFuncFields)
        {
            var _transaccion = new Transaccion
            {
                Consulta = "Insert Into Libro({0}) values({1})"
            };
            Comun.Insert(_transaccion, pLibro, pFuncFields);
            return _transaccion;
        }
        internal static Transaccion UpdateTransa(Libro pLibro, Func<Libro, object> pFuncFields, Func<Libro, object> pFuncWhere)
        {
            var _transaccion = new Transaccion
            {
                Consulta = "Update Libro Set {0} Where {1}",
            };
            var excepciones = new Dictionary<string, Func<string, object, string>>();
            Comun.Update(_transaccion, pLibro, pFuncFields, pFuncWhere, excepciones);
            return _transaccion;
        }
        public static Transaccion ObtenerTransacion(TipoTransacion pTipoTransacion, Libro pLibro)
        {
            if (pLibro == null)
                return new Transaccion();
            var _transaccion = new Transaccion();
            switch (pTipoTransacion)
            {
                // Guardar Transaccion 
                case TipoTransacion.Guardar:
                    pLibro.Estado = 1;
                    _transaccion = InsertTransa(pLibro, s => new
                    {
                        s.Codigo,
                        s.Titulo,
                        s.Editorial,
                        s.FechaPublicacion,
                        s.Stock,
                        s.Estado
                    });
                    break;
                case TipoTransacion.Modificar:
                    _transaccion = UpdateTransa(pLibro, s => new
                    {
                        s.Codigo,
                        s.Titulo,
                        s.Editorial,
                        s.FechaPublicacion,
                        s.Stock
                    }, y => new { y.Id });
                    break;
                case TipoTransacion.HabilitarODeshabilitar:
                    _transaccion = new Transaccion
                    {
                        Consulta = "Update Libro set Estado=@Estado Where Id=@Id",
                        Parametros = new List<Parametro>
                        {
                          new Parametro { Name="Estado", Objeto=pLibro.Estado},
                          new Parametro { Name="Id", Objeto=pLibro.Id},
                        },
                        //**************************
                    };
                    break;
            }
            return _transaccion;
        }
        public static int Guardar(Libro pLibro)
        {
            return ComunDB.EjecutarComando(new List<Transaccion> { ObtenerTransacion(TipoTransacion.Guardar, pLibro) });
        }
        public static int Modificar(Libro pLibro)
        {
            return ComunDB.EjecutarComando(new List<Transaccion> { ObtenerTransacion(TipoTransacion.Modificar, pLibro) });
        }
        public static int Eliminar(Libro pLibro)
        {
            pLibro.Estado = 0;
            return ComunDB.EjecutarComando(new List<Transaccion> { ObtenerTransacion(TipoTransacion.HabilitarODeshabilitar, pLibro) });
        }
        private static List<Libro> ObtenerDatos(Transaccion pTransacion)
        {
            var _lista = new List<Libro>();
            using (var _reader = ComunDB.Reader(pTransacion))
            {
                if (_reader != null)
                {
                    int index = 0;
                    Func<int> getIndex = () =>
                    {
                        index++;
                        return index;
                    };
                    while (_reader.Read())
                    {
                        index = -1;
                        _lista.Add(new Libro
                        {
                            Id = _reader.GetInt32(getIndex()),
                            Codigo = _reader.GetString(getIndex()),
                            Titulo = _reader.GetString(getIndex()),
                            Editorial = _reader.GetString(getIndex()),
                            FechaPublicacion = _reader.GetDateTime(getIndex()),
                            Stock = _reader.GetInt32(getIndex()),
                            Estado = _reader.GetByte(getIndex())
                        });
                    }
                }
            }
            return _lista;
        }
        public static Transaccion ObtenerTransacionSelect(int pIncluirDistinct = 0)
        {
            var sql = "Select ";
            if (pIncluirDistinct == 1)
                sql += "distinct ";
            sql += "lib.Id, lib.Codigo, lib.Titulo, lib.Editorial, lib.FechaPublicacion, lib.Stock, ";
            sql += "lib.Estado From Libro lib";
            return new Transaccion
            {
                Consulta = sql,
            };
        }
        public static Transaccion ObtenerTransacionBuscar(Libro pLibro)
        {
            var _transacion = new Transaccion();
            var _countCampo = 0;
            // Logica de agregacion de campos al where
            //****************************************************************************************
            if (pLibro.Id > 0)
            {
                if (_countCampo > 0)
                    _transacion.Consulta += " AND ";
                _countCampo += 1;
                //***********************
                _transacion.Consulta += " lib.Id = @Id ";
                _transacion.Parametros.Add(new Parametro { Name = "Id", Objeto = pLibro.Id });
            }
            if (!string.IsNullOrWhiteSpace(pLibro.Codigo))
            {
                if (_countCampo > 0)
                    _transacion.Consulta += " AND ";
                _countCampo += 1;
                //***********************
                _transacion.Consulta += " lib.Codigo Like @Codigo ";
                _transacion.Parametros.Add(new Parametro { Name = "Codigo", Objeto = "%" + pLibro.Codigo + "%" });
            }
            if (!string.IsNullOrWhiteSpace(pLibro.Titulo))
            {
                if (_countCampo > 0)
                    _transacion.Consulta += " AND ";
                _countCampo += 1;
                //***********************
                _transacion.Consulta += " lib.Titulo Like @Titulo ";
                _transacion.Parametros.Add(new Parametro { Name = "Titulo", Objeto = "%" + pLibro.Titulo + "%" });
            }
            if (pLibro.Estado >= 0 && pLibro.Estado <= 1)
            {
                if (_countCampo > 0)
                    _transacion.Consulta += " AND ";
                _countCampo += 1;
                //***********************
                _transacion.Consulta += " lib.Estado = @Estado ";
                _transacion.Parametros.Add(new Parametro { Name = "Estado", Objeto = pLibro.Estado });
            }
            //****************************************************************************************
            return _transacion;
        }
        public static List<Libro> Buscar(Libro pLibro)
        {
            try
            {
                // Validar si EstadoPO trae algo que buscar
                if (pLibro == null)
                    return new List<Libro>();
                // Obtener la transacion con los datos del where y comprobar si hay datos para buscar o no
                var _transacionWhere = ObtenerTransacionBuscar(pLibro);
                // Comprobar si hay parametro de busqueda o si en realidad se realizara un inner join en la consulta de busquedad 
                if (_transacionWhere.Parametros.Count < 1)
                    return new List<Libro>();
                //*************************************************************
                // Obtener la transacion del select para la obtencion de los datos               
                int IncluirDistinct = 1;
                var _transacion = ObtenerTransacionSelect(IncluirDistinct);
                // Agregar inner join a la consulta
                _transacion.Consulta += " Where " + _transacionWhere.Consulta;
                _transacion.Parametros.AddRange(_transacionWhere.Parametros);
                _transacion.Top = pLibro.TopRegistro;
                return ObtenerDatos(_transacion);
            }
            catch (Exception ex)
            {
                return new List<Libro>();
            }
        }
        public static Libro ObtenerPorId(Libro pLibro)
        {
            var _transacion = ObtenerTransacionSelect();
            _transacion.Consulta += " Where lib.Id = @Id AND lib.Estado = @Estado";
            _transacion.Parametros = new List<Parametro> {
               new Parametro { Name = "Id", Objeto = pLibro.Id },
               new Parametro { Name = "Estado", Objeto = pLibro.Estado },
            };
            var _entidad = ObtenerDatos(_transacion).FirstOrDefault();
            if (_entidad != null)
                return _entidad;
            return new Libro();
        }
        public static List<Libro> Obtener()
        {
            var _transacion = ObtenerTransacionSelect();
            _transacion.Consulta += " Where lib.Estado=1";
            _transacion.Top = int.MaxValue;
            return ObtenerDatos(_transacion);
        }
    }
}
