using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaVirtual.DBManager;

namespace BibliotecaVirtual.DAL
{
    public class ValidacionDeDatos
    {
        public static bool ValidarString(string pString)
        {
            try
            {
                if (pString != null)
                {
                    pString = pString.Trim();
                    if (pString.Length > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public static bool ValidarStringV2(ref string pString)
        {
            try
            {
                if (pString != null)
                {
                    pString = pString.Trim();
                    if (pString.Length > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public static bool ValidarFecha(DateTime pDateTime)
        {
            try
            {
                if (pDateTime.Year > DateTime.Now.AddYears(-150).Year)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool ValidIntsArray(Int16[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static bool ValidIntsArray(UInt16[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static bool ValidIntsArray(Int32[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static bool ValidIntsArray(UInt32[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static bool ValidIntsArray(Int64[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static bool ValidIntsArray(UInt64[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != 0)
                    return true;
            return false;
        }
        public static string JoinIntsArray(Int16[] a, String sep = ", ")
        {
            return string.Join<Int16>(sep, a.Where(z => z != 0).Distinct());
        }
        public static string JoinIntsArray(Int32[] a, String sep = ", ")
        {
            return string.Join<Int32>(sep, a.Where(z => z != 0).Distinct());
        }
        public static string JoinIntsArray(Int64[] a, String sep = ", ")
        {
            return string.Join<Int64>(sep, a.Where(z => z != 0).Distinct());
        }
        public static bool ValidEnteroArray<T>(T[] a = null)
        {
            if (!(a != null && a.Length > 0))
                return false;
            for (int i = 0; i < a.Length; i++)
                if (Convert.ToInt64(a[i]) != 0)
                    return true;
            return false;
        }
        public static string JoinEnteroArray<T>(T[] a, String sep = ", ")
        {
            return string.Join<T>(sep, a.Where(z => Convert.ToInt64(z) != 0).Distinct());
        }
        public static bool ValidStringArray(String[] a = null)
        {
            if (a == null)
                return false;
            for (int i = 0; i < a.Length; i++)
                if (!string.IsNullOrWhiteSpace(a[i]))
                    return true;
            return false;
        }
        public static string JoinStringArray(String[] a, String setp = "', '")
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = string.IsNullOrWhiteSpace(a[i]) ? "" : a[i].Trim().Replace("'", "\'");//reemplazar comilla simple por caracter de escape para evitar sql injection
            return "'" + String.Join<String>(setp, a.Where(z => !String.IsNullOrWhiteSpace(z)).Distinct()) + "'";
        }
    }

    public class TransaccionBuscar
    {
        public static void SetIdArray(Transaccion pTransaccion, ref int pCount, Int64[] pIds, string pCampo, string pAlias, string pNameParameter = "", bool pApplyNotIn = false)
        {
            if (pNameParameter.Trim().Length == 0)
            {
                pNameParameter = pCampo + "_array";
            }
            if (ValidacionDeDatos.ValidIntsArray(pIds))
            {
                if (pCount > 0)
                    pTransaccion.Consulta += " And ";
                pCount++;
                pTransaccion.Consulta += " " + pAlias + "." + pCampo + (pApplyNotIn ? " not" : "") + " in (" + string.Join(", ", pIds.Where(n => n > 0)) + ")\n";
                pTransaccion.Parametros.Add(new Parametro { Name = pNameParameter, Objeto = 1 });
            }
        }
    }
}
