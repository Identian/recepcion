using CapaDominio.Errors;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using CasosDeUso.Inputs.ConsultarEmail;
using CasosDeUso.Inputs.GestionarReceptor;
using CasosDeUso.Inputs.InactivarCorreo;
using CasosDeUso.Inputs.PingEmail;
using CasosDeUso.Inputs.RegistrarEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Radicacion.WebApi.Controllers
{
    /// <summary>
    /// Controlador de consulta y administración de cuentas de correo electrónico.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ManageEmailController : Controller
    {
        readonly IMediator _mediator;
        readonly ILogAzure _logAzure;

        public ManageEmailController(IMediator mediator, ILogAzure logAzure)
        {
            _mediator = mediator;
            _logAzure = logAzure;
        }

        /// <summary>
        /// Método de lectura, gestión y radicación de archivos .zip contenidos en el correo.
        /// </summary>
        /// <param name="info"></param>
        [HttpPost]
        public async Task<int> Post([FromBody] object info)
        {
            Stopwatch timeT = new();
            timeT.Start();
            IRespuesta respuesta = new Respuesta();
            string contenido = info.ToString() ?? "";
            try
            {
                if (contenido != "")
                {
                    InfoReceptorInput? input = JsonConvert.DeserializeObject<InfoReceptorInput>(contenido);
                    if (input!.r is not null && input.cb is not null)
                    {
                        respuesta = await _mediator.Send(input);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logAzure.SaveLog(ErrorsCodes._500I, "Consulta de correo", "0", "GestionCorreo. Error leyendo datos de entrada ", ex.Message + " --- " + ex.StackTrace, "", ref timeT);
                timeT.Stop();
            }
            return respuesta.MensajesProcesados;
        }

        /// <summary>
        /// Método de verificación de acceso al correo electrónico.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("emailPing")]
        public async Task<string> VerificarAccesoEmail([FromBody] object content)
        {
            string result;
            EmailPingInput? input = new();
            try
            {
                string? contenido = content.ToString();
                if (contenido != null)
                {
                    input = JsonConvert.DeserializeObject<EmailPingInput>(contenido);
                    result = await _mediator.Send(input!);
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = $"Error desconocido {ErrorsCodes._400}" });
                }
            }
            catch (Exception ex)
            {
                if (ex is FluentValidation.ValidationException)
                {
                    string fluentError = ex.Message.Split(":")[2].Replace("Severity", "").Trim();
                    return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = $"Error: {fluentError}" });
                }

                return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = string.Format(ErrorsCodes._400Ex1, input!.Usuario) + " por favor validar parámetros enviados" });
            }
            return result;
        }

        /// <summary>
        /// Método que registrar y/o actualiza la configuración de un correo electrónico
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("emailGuardar")]
        public async Task<string> RegistrarActualizarEmail([FromBody] object content)
        {
            string? contenido = content.ToString();
            string resultado = "";
            try
            {
                if (contenido != null)
                {
                    RegistrarEmailInput? emailInput = JsonConvert.DeserializeObject<RegistrarEmailInput>(contenido);

                    if (emailInput != null)
                    {
                        resultado = await _mediator.Send(emailInput);
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = ErrorsCodes._400 });
                }
            }
            catch (Exception ex)
            {
                if (ex is FluentValidation.ValidationException)
                {
                    string fluentError = ex.Message.Split(":")[2].Replace("Severity", "").Trim();
                    return JsonConvert.SerializeObject(new { Codigo = 500, Mensaje = $"Error: {fluentError}" });
                }
                return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = ErrorsCodes._400 + " por favor validar los parámetros enviados" });
            }
            return resultado;
        }

        /// <summary>
        /// Método que permite inactivar la configuración existente de un correo electrónico.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("emailInactivar")]
        public async Task<string> InactivarCorreoReceptor([FromBody] object content)
        {
            dynamic respuesta = new JObject();
            string? contenido = content.ToString();
            try
            {
                if (contenido != null)
                {
                    InactivarEmailInput? emailInput = JsonConvert.DeserializeObject<InactivarEmailInput>(contenido);
                    if (emailInput != null)
                    {
                        respuesta = await _mediator.Send(emailInput);
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Codigo = 500, Mensaje = ErrorsCodes._500A + " por favor validar parametros enviados" });
                }
            }
            catch (Exception ex)
            {
                if (ex is FluentValidation.ValidationException)
                {
                    string fluentError = ex.Message.Split(":")[2].Replace("Severity", "").Trim();
                    return JsonConvert.SerializeObject(new { Codigo = 500, Mensaje = $"Error: {fluentError}" });
                }

                return JsonConvert.SerializeObject(new { Codigo = 500, Mensaje = ErrorsCodes._500A + " por favor validar parametros enviados" });
            }
            return respuesta;
        }

        /// <summary>
        /// Método que permite consultar el estado de la configuración existente de un correo electrónico.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("emailConsultar")]
        public async Task<string> ConsultarEmail([FromBody] object content)
        {
            dynamic respuesta = new JObject();
            string? contenido = content.ToString();
            try
            {
                if (contenido != null)
                {
                    ConsultarEmailInput? emailInput = JsonConvert.DeserializeObject<ConsultarEmailInput>(contenido);
                    if (emailInput != null)
                    {
                        respuesta = await _mediator.Send(emailInput);
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = "Error campos vacíos, válida por favor los parámetros enviados." });
                }
            }
            catch (Exception ex)
            {
                if (ex is FluentValidation.ValidationException)
                {
                    string fluentError = ex.Message.Split(":")[2].Replace("Severity", "").Trim();
                    return JsonConvert.SerializeObject(new { Codigo = 400, Mensaje = $"Error: {fluentError}" });
                }
                respuesta.Codigo = 999;
                respuesta.Mensaje = $"Error desconocido, por favor validar los parámetros enviados";
                return JsonConvert.SerializeObject(new { Codigo = 999, Mensaje = "Error desconocido, válida por favor los parámetros enviados." });
            }
            return respuesta;
        }
    }
}
