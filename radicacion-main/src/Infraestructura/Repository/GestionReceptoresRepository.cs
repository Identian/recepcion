using AutoMapper;
using CapaDominio.Enums.Logs;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Receptor;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using Infraestructura.Helpers;
using Infraestructura.Logs;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Repository
{
    public class GestionReceptoresRepository : IGestionReceptoresRepository
    {
        readonly IConfiguration _configuration;
        private IReceptor _receptorDb;
        private ICuentaCorreo _cuentaCorreoDb;
        private readonly IEmailManagement _emailManagement;
        private readonly IEmailConectar _emailConectar;
        private readonly IMapper _mapper;
        private readonly IRespuesta _respuesta;

        public GestionReceptoresRepository(
            IConfiguration configuration,
            IReceptor receptorDb,
            ICuentaCorreo CuentaCorreoDb,
            IEmailManagement emailManagement,
            IEmailConectar emailConectar,
            IMapper mapper, IRespuesta respuesta)
        {
            _configuration = configuration;
            _receptorDb = receptorDb;
            _cuentaCorreoDb = CuentaCorreoDb;
            _emailManagement = emailManagement;
            _emailConectar = emailConectar;
            _mapper = mapper;
            _respuesta = respuesta;
        }

        public async Task<IRespuesta> GestionarReceptor(InfoReceptor infoReceptor)
        {
            Stopwatch timeT = new();
            timeT.Start();
            LogAzure logAzure = new(_configuration, infoReceptor.r.IdReceptor.ToString());
            IRespuesta respuesta = _respuesta;
            try
            {
                try
                {
                    Stopwatch timeS = new();
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "begin GestionCorreo: " + infoReceptor.cb.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);

                    //Receptor
                    _receptorDb = infoReceptor.r;
                    //Cuenta base
                    _cuentaCorreoDb = infoReceptor.cb;

                    //se obtiene la base del receptor
                    IReceptorBase receptorBase = _mapper.Map<ReceptorBase>(_receptorDb);

                    respuesta = await _emailManagement.ProcesarMensajesPendientes(receptorBase, _cuentaCorreoDb, logAzure);

                    #region opción que desactiva la cuenta si no coniciden las contraseña
                    if (respuesta.Codigo == 401)
                    {
                        await _emailConectar.InactivarCorreoReceptor(_receptorDb.NumeroIdentificacionReceptor!, _receptorDb.TipoIdentificacionReceptor!, logAzure);
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Inactivar correo por problemas de clave de correo para " + _receptorDb.NumeroIdentificacionReceptor, LevelMsn.Info, timeS.ElapsedMilliseconds);
                    }
                    #endregion

                    if (respuesta.MensajesProcesados == 0)
                    {
                        logAzure.SaveLog(respuesta.Codigo.ToString(), "Consulta de correo", _receptorDb.NumeroIdentificacionReceptor!, "GestionCorreo", respuesta.Descripcion + " -- " + respuesta.Detalles + " -- " + respuesta.DetallesAdicionales, _cuentaCorreoDb.Usuario!, ref timeT);
                    }
                    respuesta.MensajesProcesados = respuesta.MensajesProcesados == -1 ? 0 : respuesta.MensajesProcesados;
                }
                catch (Exception ex1)
                {
                    logAzure.SaveLog(ErrorsCodes._500I, "Consulta de correo - GestionarReceptor - eX1", _receptorDb.NumeroIdentificacionReceptor!, "GestionCorreo", ex1.Message + " --- " + ex1.StackTrace!.ToString(), _cuentaCorreoDb.Usuario!, ref timeT);
                }
            }
            catch (Exception ex2)
            {
                LogAzure logex = new(_configuration, "0");
                logex.SaveLog(ErrorsCodes._500I, "Consulta de correo - GestionarReceptor - eX2", "0", "GestionCorreo", ex2.Message + " --- " + ex2.StackTrace!.ToString(), (infoReceptor.ToString() ?? "").DesEncriptar(), ref timeT);
            }
            return respuesta;
        }
    }
}
