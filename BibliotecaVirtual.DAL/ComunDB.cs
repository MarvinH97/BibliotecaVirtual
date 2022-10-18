using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BibliotecaVirtual.DBManager;

namespace BibliotecaVirtual.DAL
{
    public enum TipoTransacion
    {
        Guardar = 1,
        Modificar = 2,
        HabilitarODeshabilitar = 3
    }
    public class ComunDB
    {
        public static int EjecutarComando(List<Transaccion> pTransaciones) 
        {
            return ComunDBManager.EjecutarComando(pTransaciones);
        }
        public static IDataReader Reader(Transaccion pTransacion)
        {
            IDataReader _reader = null;
            try
            {
                foreach (var item2 in pTransacion.Parametros)
                    if (item2.Objeto is string)
                        item2.Objeto = item2.Objeto.ToString().ToUpper();
                pTransacion.Consulta = pTransacion.Consulta.ToLower();
                _reader = ComunDBManager.Reader(pTransacion);
            }
            catch (Exception ex)
            {
                //Manejar excepciones
            }
            return _reader;
        }
        public static string SQLtoGetTheLastId(string pTable, string pCampo)
        {
            return ComunDBManager.SQLGetLastId(pTable, pCampo);
        }   
    }

    public static class HandlerData 
    {
        public static object GetValuePropExt(this object pObj, string pNameProp, BindingFlags? pBindingFlags = null)
        {
            if (pBindingFlags == null)
            {
                var xl = pObj.GetType().GetProperty(pNameProp);
                return pObj.GetType().GetProperty(pNameProp).GetValue(pObj, null);
            }
            else
            {
                var xl = pObj.GetType().GetProperty(pNameProp, bindingAttr: (BindingFlags)pBindingFlags);
                return xl.GetValue(pObj, null);
            }
        }
        public static void ExtUpdateWhere<T>(this List<Transaccion> pTransas, T pT, Func<T, object> pfFields, Func<T, object> pfWhereAnd)
        {
            string xNameTable = pT.GetType().Name;
            var _transaccion = new Transaccion
            {
                Consulta = "Update " + xNameTable + " set {0} where {1}",
            };
            var xObjsField = pfFields(pT);
            var xProps = xObjsField.GetType().GetProperties();
            if (xProps.Length == 0)
                throw new System.ArgumentException("No se seleccionaron propiedades a Modificar en la clase y tabla " + xNameTable, "No hay propiedades a modificar");
            string xParameters = "";
            foreach (var item in xProps)
            {
                xParameters += xParameters.Trim().Length > 0 ? ("," + item.Name + "=@" + item.Name) : (item.Name + "=@" + item.Name);
                _transaccion.Parametros.Add(new Parametro { Name = item.Name, Objeto = xObjsField.GetValuePropExt(item.Name) });
            }
            var xObjsWhereAnd = pfWhereAnd(pT);
            var xPropsWhere = xObjsWhereAnd.GetType().GetProperties();
            if (xPropsWhere.Length == 0)
                throw new System.ArgumentException("No se seleccionaron propiedades en la condicion where  en la clase y tabla " + xNameTable, "No hay propiedades en el Where");
            string xParametersWhere = "";
            foreach (var item in xPropsWhere)
            {
                xParametersWhere += xParametersWhere.Trim().Length > 0 ? (" AND " + item.Name + "=@" + item.Name) : (item.Name + "=@" + item.Name);
                _transaccion.Parametros.Add(new Parametro { Name = item.Name, Objeto = xObjsWhereAnd.GetValuePropExt(item.Name) });
            }
            _transaccion.Consulta = string.Format(_transaccion.Consulta, xParameters, xParametersWhere);
            pTransas.Add(_transaccion);
        }
    }
}
