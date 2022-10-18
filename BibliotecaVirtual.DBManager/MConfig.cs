using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BibliotecaVirtual.DBManager
{
    public class MConfig
    {
        public static string ConnectionString() 
        {
            var _conexionesString = ObtenerConnectionStringSettings();
            var connectionString = _conexionesString.ConnectionString;
            return connectionString;
        }
        public static string Provider() 
        {
            var _conexionesString = ObtenerConnectionStringSettings();
            var provider = _conexionesString.ProviderName;
            return provider;
        }
        private static ConnectionStringSettings ObtenerConnectionStringSettings()
        {
            var _connectionStringSettings = new ConnectionStringSettings();
            try
            {
                ConnectionStringSettingsCollection _conexionesString = ConfigurationManager.ConnectionStrings;
                var _nameConection = ConfigurationManager.AppSettings["NameConexion"];

                foreach (ConnectionStringSettings item in _conexionesString)
                {
                    if (item.Name == _nameConection)
                    {
                        _connectionStringSettings = item;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                //Controlar excepcion
            }
            return _connectionStringSettings;
        }
    }
}
