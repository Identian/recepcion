using Microsoft.Data.SqlClient;
using System.Data;

namespace Receptores.DB
{
    public class DataBase
    {
        private readonly IConfiguration _configuration;

        public DataBase(IConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// Conectar a bases de datos
        /// </summary>
        /// <returns></returns>
        public SqlConnection Connection()
        {
            string connectionString = _configuration.GetValue("ConnectionString", "") ?? "";
            SqlConnection conection = new(connectionString);
            conection.Open();
            return conection;
        }

        /// <summary>
        /// Cerrar la conexión de base de datos
        /// </summary>
        public void CloseConnection()
        {
            this.Connection().Close();
        }

        /// <summary>
        /// Verificar estado de la conexión
        /// </summary>
        public void StatusConnection()
        {
            if (this.Connection().State != ConnectionState.Open)
            {
                this.Connection();
            }
        }
    }
}
