
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using WcfRecepcionSOAP.Base;

namespace WcfRecepcionSOAP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class ReceptorWS : IReceptorWS
    {
        private MemoryCache _memorycache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto de ser null o vacio sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EstatusDocumentoResponse EstadoDocumento(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new EstatusDocumentoResponse { codigo = 105, mensaje = "Error en el servicio . Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new EstatusDocumentoResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    EstatusDocumentoResponse response = de.EstadoDocumento(request, _memorycache);
                    return response;
                }
                else
                {
                    return new EstatusDocumentoResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega El campo TipoDocumento no es obligatorio
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MetadataDocumentoResponse CosultaDocumentoMetadata(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new MetadataDocumentoResponse { codigo = 105, mensaje = "Error en el servicio . Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new MetadataDocumentoResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    MetadataDocumentoResponse response = de.MetadatosDocumento(request, _memorycache);
                    return response;
                }
                else
                {
                    return new MetadataDocumentoResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// No se agrega parametros extra
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReporteResponse Reporte(ReceptorReporteRequest request)
        {
            if (request == null)
            {
                return new ReporteResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    ReporteResponse response = de.Reporte(request, _memorycache);
                    if (response.codigo == 200)
                    {
                        response.resultado = "Procesado";
                    }
                    else
                    {
                        response.resultado = "Error";
                    }

                    return response;
                }
                else
                {
                    return new ReporteResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// No se agrega parametros extra
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReporteStatusResponse ReporteStatus(ReceptorReporteStatusRequest request)
        {
            if (request == null)
            {
                return new ReporteStatusResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    ReporteStatusResponse response = de.ReporteStatus(request, _memorycache);
                    if (response.codigo == 200)
                    {
                        response.resultado = "Procesado";
                    }
                    else
                    {
                        response.resultado = "Error";
                    }

                    return response;
                }
                else
                {
                    return new ReporteStatusResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral CambioEstatus(ReceptorCambioEstatusRequest request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    ResponseGeneral response = de.CambioEstatus(request, _memorycache);
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral AcuseRecibo(ReceptorCambioEstatusRequest request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    if ((request.status.Trim() == "2") || (request.status.Trim() == "3") || (request.status.Trim() == "4"))
                    {
                        DocumentoElectronico de = new DocumentoElectronico();
                        request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                        if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                        {
                            Match match;
                            match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                            if (!match.Success)
                            {
                                return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                            }
                        }
                        ResponseGeneral response = de.AcuseRecibo(request, _memorycache);
                        return response;
                    }
                    else
                    {
                        return new ResponseGeneral { codigo = 108, mensaje = "Código de Estado para Acuse de Recibo debe ser 2, 3 ó 4", resultado = "Error" };
                    }
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto de ser null o vacio sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarXML(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 1);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto de ser null o vacio sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarRepGrafica(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 2);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Parametro TipoDocumento opcional por defecto sera 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse1 DescargarAnexo(ReceptorRequestAnexo request)
        {
            if (request == null)
            {
                return new FileDownloadResponse1 { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse1 { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    FileDownloadResponse1 response = de.DescargarAnexo(request, _memorycache);
                    // var response = new FileDownloadResponse();
                    return response;
                }
                else
                {
                    return new FileDownloadResponse1 { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega el campo TipoDocumento no es de uso obligatoprio si el campo esta null o vacio se establece por defecto en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral EliminarAnexo(ReceptorRequestAnexo request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    ResponseGeneral response = de.EliminaAnexo(request, _memorycache);
                    // var response = new ResponseGeneral();
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega el campo TipoDocumento no es de uso obligatoprio si el campo esta null o vacio se establece por defecto en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ArchivoDocumentoResponse ListaAnexo(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new ArchivoDocumentoResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ArchivoDocumentoResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    ArchivoDocumentoResponse response = de.ListaAnexo(request, _memorycache);
                    // var response = new ArchivoDocumentoResponse();
                    return response;
                }
                else
                {
                    return new ArchivoDocumentoResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega el campo TipoDocumento no es de uso obligatorio si el campo esta null o vacio se establece por defecto en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarAcuseRecibidoXML(ReceptorRequestApplicationResponse request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                       WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 3);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarAcuseAceptacionXML(ReceptorRequestApplicationResponse request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 4);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarAcuseReclamoXML(ReceptorRequestApplicationResponse request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 5);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }
        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarRecepcionBienServicioXML(ReceptorRequestApplicationResponse request)
        {
            if (request == null)
            {
                return new FileDownloadResponse { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new FileDownloadResponse { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }

                    FileDownloadResponse response = de.DescargarArchivo(request, _memorycache, 6);
                    return response;
                }
                else
                {
                    return new FileDownloadResponse { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral EliminarRepGrafica(ReceptorRequestGeneral request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    ResponseGeneral response = de.EliminaRepGrafica(request, _memorycache);
                    // var response = new ResponseGeneral();
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneralInfo EnviarXML(EnviarXMLRequest request)

        {
            if (request == null)
            {
                return new ResponseGeneralInfo { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            // return new Models.Requests.ArchivoXML  { Archivo = "Error en el servicio"};
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();

                    ResponseGeneralInfo response = de.EnviarXMLReceptor(request, _memorycache);
                    return response;
                }
                else
                {
                    return new ResponseGeneralInfo { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                    //return new Models.Requests.ArchivoXML { Archivo = "Error en el servicio" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral EnviarRepGrafica(EnviarArchivoReceptorRequest request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    if (request.extension.Trim().ToLower() == "pdf")
                    {
                        DocumentoElectronico de = new DocumentoElectronico();
                        request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                        if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                        {
                            Match match;
                            match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                            if (!match.Success)
                            {
                                return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                            }
                        }
                        ResponseGeneral response = de.EnviarRepGraficaReceptor(request, _memorycache);
                        return response;
                    }
                    else
                    {
                        return new ResponseGeneral { codigo = 102, mensaje = "La extensión de la Representación Gráfica debe ser pdf", resultado = "Error" };
                    }
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }
        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral EnviarAnexo(EnviarArchivoReceptorRequest request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    ResponseGeneral response = de.EnviarRepGraficaReceptor(request, _memorycache, false);
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        /// <summary>
        /// HU-128
        /// Se agrega campo TipoDocumento el cual no es obligatorio de estar null ovacio se establece en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGeneral EnviarMetadata(EnviarMetadataReceptorRequest request)
        {
            if (request == null)
            {
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            }
            else
            {
                ValidationContext context = new ValidationContext(request, null, null);
                List<ValidationResult> result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    DocumentoElectronico de = new DocumentoElectronico();
                    request.tipoIdentificacionemisor = (string.IsNullOrEmpty(request.tipoIdentificacionemisor) || string.IsNullOrWhiteSpace(request.tipoIdentificacionemisor)) ?
                                                      WebConfigurationManager.AppSettings["defaultTypeIdentification"] : request.tipoIdentificacionemisor;
                    if (de.identification_Must_Be_Numerical(request.tipoIdentificacionemisor))
                    {
                        Match match;
                        match = Regex.Match(request.identificadorEmisor, @"^\d+$");
                        if (!match.Success)
                        {
                            return new ResponseGeneral { codigo = 102, mensaje = "Identificador del Emisor debe ser Numérico", resultado = "Error" };
                        }
                    }
                    ResponseGeneral response = de.EnviarMetadataReceptor(request, _memorycache);
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

    }


}
