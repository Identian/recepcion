using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Invoice;
using CapaDominio.Response;
using System.Reflection;

namespace Infraestructura.Helpers
{
    public class DataBaseHelper : IDataBaseHelper
    {
        private readonly IDataBase _dataBase;

        public DataBaseHelper(IDataBase dataBase)
        {
            _dataBase = dataBase;
        }

        public async Task<IRespuesta> RegistrarErrorsDb(IReceptorBase receptorBase, ILogAzure logAzure, int codigo, string subject)
        {
            IRespuesta respuesta = new Respuesta();
            try
            {
                IDictionary<int, string> mensajes = new Dictionary<int, string>() {
                {70, $"Sin documento válido, hay más de un archivo (.zip). Asunto del correo: {subject}" },
                {71, $"Sin documento válido, no se encontró archivo (.zip). Asunto del correo: {subject}" },
                {72, $"Sin documento válido, no se encontró (.xml) dentro del (.zip). Asunto del correo: {subject}" },
                {73, $"Sin documento válido, el archivo (.zip) esta vacio. Asunto del correo: {subject}" },
                {74, $"Sin documento válido, el archivo (.zip) no se puede procesar. Asunto del correo: {subject}" },
            };

                mensajes.TryGetValue(codigo, out string? mensaje);
                respuesta = await _dataBase.RegistrarInvoiceReceptionErrors(new InvoiceReceptionError()
                {
                    IdEnterprise = receptorBase.IdReceptor,
                    Code = codigo,
                    CreatedAt = DateTime.UtcNow,
                    DateReception = DateTime.UtcNow,
                    DocumentType = 0,
                    Message = mensaje,
                    PartyIdentificationId = "--",
                    UblVersionId = "UBL 2.1",
                    UpdatedAt = DateTime.UtcNow,
                    MountTotal = 0,
                    SchemeId = "0",
                    DocumentId = "--",
                    IssueDate = DateTime.UtcNow,
                    IssueTime = DateTime.UtcNow
                }, logAzure);
            }
            catch (Exception ex)
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, ex.Message);
            }
            return respuesta;
        }
    }
}
