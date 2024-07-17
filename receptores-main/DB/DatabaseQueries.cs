using Dapper;
using Microsoft.Data.SqlClient;
using Receptores.Interfaces;
using Receptores.Model.Correo;
using Receptores.Model.Estructuras;
using System.Data;

namespace Receptores.DB
{
    public class DatabaseQueries : DataBase, IDatabaseQueries
    {

        private readonly ILogger<DatabaseQueries> _logger;

        public DatabaseQueries(IConfiguration configuration, ILogger<DatabaseQueries> logger) : base(configuration)
        {
            _logger = logger;
        }

        /// <summary>
        /// Metodo que obtiene las cuentas de los receptores activos 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Estructura>> GetAccounts()
        {
            List<Estructura> listaestructuras = new();
            try
            {

                


                StatusConnection();
                SqlConnection connection = this.Connection();
                IEnumerable<CuentaCorreoReceptorBase> cuentas = await connection.QueryAsync<CuentaCorreoReceptorBase>("sp_AGE_ConsultarCorreosReceptores", null);

                foreach (CuentaCorreoReceptorBase cuenta in cuentas)
                {
                    cuenta.BandejaDescargados ??= string.Empty;
                    cuenta.BandejaEntrada ??= string.Empty;
                    cuenta.BandejaErroneos ??= string.Empty;
                    cuenta.BandejaOtros ??= string.Empty;
                    cuenta.RefreshToken ??= string.Empty;
                }
                List<CuentaCorreoReceptorBase> listcount = cuentas.ToList();

                int cantidadDeRegistros = listcount.Count;
                int receptorActual = 0;
                for (int i = 0; i < cantidadDeRegistros; i++)
                {
                    List<Model.Correo.Receptor> receptores = await this.GetReceptorList(connection, listcount[receptorActual].IdCuentaCorreoReceptor);

                    if (receptores.Count > 0)
                    {
                        listaestructuras.Add(new Estructura(i, listcount[receptorActual], receptores[0]));
                    }

                    receptorActual++;
                    if (receptorActual == cuentas.Count())
                    {
                        receptorActual = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: Exception {exception}", ex.Message);
            }
            return listaestructuras;
        }

        private async Task<List<Model.Correo.Receptor>> GetReceptorList(SqlConnection connection, int idCorreoReceptor)
        {
            List<Model.Correo.Receptor> receptorList = new();
            try
            {
                if (!int.IsNegative(idCorreoReceptor))
                {
                    DynamicParameters parameters = new();
                    parameters.Add("@IdCorreoReceptor", idCorreoReceptor);

                    //Execute stored procedure and map the returned result to a Customer object  
                    IEnumerable<Model.Correo.Receptor> customer = await connection.QueryAsync<Model.Correo.Receptor>("sp_AGE_ConsultarReceptor", parameters, commandType: CommandType.StoredProcedure);
                    receptorList = customer.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: Exception: {exception}", ex.Message);
            }
            return receptorList;
        }

        /// <summary>
        /// Metodo que permite marcar las cuentas en la base de datos como ocupadas
        /// Proceso realizado en la lectura de cada cuenta de receptor
        /// Cuando la cuenta tiene correos por procesar se marca como ocupada para evitar 
        /// la relectura en un mismo ciclo.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="IdCuentaCorreoReceptor"></param>
        /// <returns></returns>
        public async Task<int> UpdateAccounts(int status, int IdCuentaCorreoReceptor)
        {
            int rowsAffected = 0;
            try
            {
                SqlConnection connection = Connection();

                DynamicParameters parameters = new();
                parameters.Add("@IdCuentaCorreo", IdCuentaCorreoReceptor);
                parameters.Add("@status", status);
                parameters.Add("@date", DateTime.UtcNow);

                IEnumerable<dynamic> query = await connection.QueryAsync("UPDATE dbo.CuentasCorreosReceptores SET Task_status = @status, Date_status = @date WHERE IdCuentaCorreo = @IdCuentaCorreo", parameters);

                rowsAffected = query.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: Exception: {exception}", ex.Message);
            }

            return rowsAffected;
        }

        public async Task<bool> UpdateAcountsFree()
        {
            bool success = false;
            try
            {
                SqlConnection connection = this.Connection();

                DynamicParameters parameters = new();
                parameters.Add("@status", 1);
                parameters.Add("@date", DateTime.UtcNow);
                parameters.Add("@lastStatus", 2);

                IEnumerable<dynamic> query = await connection.QueryAsync("UPDATE dbo.CuentasCorreosReceptores SET Task_status = @status, Date_status = @date WHERE Task_status = @lastStatus", parameters);

                int rowsAffected = query.Count();

                success = (rowsAffected != 0) || (rowsAffected == 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return success;
        }
    }


}

