using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using WcfRecepcionSOAP.Models.Requests;
using WcfRecepcionSOAP.Models.Responses;


namespace WcfRecepcionSOAP.Base
{
    public class DocumentoElectronico
    {
        public Boolean identification_Must_Be_Numerical(string type)
        {

            return (type != "42");
        }

        public EstatusDocumentoResponse EstadoDocumento(ReceptorRequestGeneral request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);

            //string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionAks"];

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new EstatusDocumentoResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/estadosdocumento", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            /*DocumentoNitRequest rqs = new DocumentoNitRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;*/


            var requestService = new
            {
                NitEmisor = request.identificadorEmisor,
                NumeroDocumento = request.numeroDocumento,
                TipoIdentificacionemisor = request.tipoIdentificacionemisor,
                ApplicationType = 0,
                TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento
            };



            rq.AddJsonBody(requestService);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseDto>> response = ws.ExecutePostTaskAsync<ResponseDto>(rq);
            response.Wait();

            ResponseDto jsonResult = response.Result.IsSuccessful ? response.Result.Data : null;

            if (jsonResult == null)
            {
                return new EstatusDocumentoResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }


            EstatusDocumentoResponse data = new EstatusDocumentoResponse()
            {
                codigo = jsonResult.codigo,
                mensaje = jsonResult.mensaje,
                resultado = jsonResult.resultado,

                estatusDIANcodigo = jsonResult.Response.estatusDIANcodigo,
                estatusDIANDescripcion = jsonResult.Response.estatusDIANDescripcion,
                estatusDIANfecha = jsonResult.Response.estatusDIANfecha,
                estatusDocumento = jsonResult.Response.estatusDocumento,
                fechaDocumento = jsonResult.Response.fechaDocumento,
                tipoDocumento = jsonResult.Response.tipoDocumento,
                uuid = jsonResult.Response.uuid,
                ListEstatusHistory = jsonResult.Response.ListEstatusHistory
            };

            return data;

            //return response.Data;
        }

        public MetadataDocumentoResponse MetadatosDocumento(ReceptorRequestGeneral request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new MetadataDocumentoResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/getmetadata", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitRequest rqs = new DocumentoNitRequest
            {
                NitEmisor = request.identificadorEmisor,
                NumeroDocumento = request.numeroDocumento,
                tipoIdentificacionemisor = request.tipoIdentificacionemisor,
                TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento
            };
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<MetadataDocumentoResponse>> response = ws.ExecuteTaskAsync<MetadataDocumentoResponse>(rq);
            response.Wait();
            MetadataDocumentoResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new MetadataDocumentoResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }



            return data;

            //return response.Data;
        }

        public ReporteResponse Reporte(ReceptorReporteRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ReporteResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/reporte/" + (string.IsNullOrEmpty(request.consecutivo) ? "null" : request.consecutivo) + "/null", Method.GET) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            //DocumentoNitRequest rqs = new DocumentoNitRequest();
            //rqs.NitEmisor = request.identificadorEmisor;
            //rqs.NumeroDocumento = request.numeroDocumento;
            //rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ReporteStatusResponse>> response = ws.ExecuteTaskAsync<ReporteStatusResponse>(rq);
            response.Wait();
            ReporteStatusResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ReporteResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde" };
            }

            ReporteResponse result = new ReporteResponse
            {
                mensaje = data.mensaje,
                pendiente = data.pendiente,
                codigo = data.codigo,
                ultimoEnviado = data.documentoselectronicos.Last().correlativoempresa,
                documentoselectronicos = new List<InfoDocumento>(data.documentoselectronicos.Select(d => new InfoDocumento
                {
                    cufe = d.cufe,
                    fechaemision = d.fechaemision,
                    fecharecepcion = d.fecharecepcion,
                    horaemision = d.horaemision,
                    montototal = d.montototal,
                    numerodocumento = d.numerodocumento,
                    tipoidentidad = d.tipoidentidad,
                    numeroidentificacion = d.numeroidentificacion,
                    razonsocial = d.razonsocial,
                    tipodocumento = d.tipodocumento,
                    tipoemisor = d.tipoemisor,
                    estatusDIANcodigo = d.estatusDIANcodigo,
                    estatusDIANfecha = d.estatusDIANfecha,
                    estatusDIANdescripcion = d.estatusDIANdescripcion,
                    correlativoempresa = d.correlativoempresa
                }))
            };




            return result;

            //return response.Data;
        }

        public ReporteStatusResponse ReporteStatus(ReceptorReporteStatusRequest request, MemoryCache memorycache)
        {
            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ReporteStatusResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/reporte/" + (string.IsNullOrEmpty(request.consecutivo) ? "null" : request.consecutivo) + "/" + (string.IsNullOrEmpty(request.status_code) ? "null" : request.status_code), Method.GET) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");

            System.Threading.Tasks.Task<IRestResponse<ReporteStatusResponse>> response = ws.ExecuteTaskAsync<ReporteStatusResponse>(rq);
            response.Wait();
            ReporteStatusResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ReporteStatusResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde" };
            }
            return data;

            //return response.Data;
        }

        public ResponseGeneral CambioEstatus(ReceptorCambioEstatusRequest request, MemoryCache memorycache)
        {
            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            //string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionAks"];

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }


            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/cambiarestatus", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            CambioEstatusRequest rqs = new CambioEstatusRequest
            {
                NitEmisor = request.identificadorEmisor,
                NumeroDocumento = request.numeroDocumento,
                Estatus = request.status,
                TipoIdentificacionEmisor = request.tipoIdentificacionemisor,
                CodigoRechazo = request.codigoRechazo,
                EjecutadoPor = new CambioEstatusRequest.EjecutadoPorRequest
                {
                    Apellido = request.EjecutadoPor.Apellido,
                    Cargo = request.EjecutadoPor.Cargo,
                    Departamento = request.EjecutadoPor.Departamento,
                    Nombre = request.EjecutadoPor.Nombre,
                    Identificacion = new CambioEstatusRequest.EjecutadoPorRequest.IdentificacionRequest
                    {
                        DV = request.EjecutadoPor.Identificacion.Dv,
                        NumeroIdentificacion = request.EjecutadoPor.Identificacion.NumeroIdentificacion,
                        TipoIdentificacion = request.EjecutadoPor.Identificacion.TipoIdentificacion
                    }


                },
                TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento
            };

            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral EnviarXML(EnviarXMLRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);
            RestRequest rq = new RestRequest("/api/integracion/enviarXML", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            ArchivoXML rqs = new ArchivoXML();
            rqs.Archivo = request.archivo;
            if (request.metadata != null)
            {
                if (request.metadata.metadata != null)
                {
                    if (request.metadata.metadata.Count() > 0)
                    {
                        rqs.metadata = new List<Metadata>();
                        foreach (Metadata m in request.metadata.metadata)
                        {
                            rqs.metadata.Add(m);
                        }
                    }
                }
            }

            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneralInfo EnviarXMLReceptor(EnviarXMLRequest request, MemoryCache memorycache)
        // public ArchivoXML EnviarXMLReceptor(EnviarXMLRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneralInfo { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
                //return new ArchivoXML { Archivo="Problema con el token "};
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/integracion/enviarXMLReceptor", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            ArchivoXML rqs = new ArchivoXML();
            rqs.Archivo = request.archivo;
            if (request.metadata != null)
            {
                if (request.metadata.metadata != null)
                {
                    if (request.metadata.metadata.Count() > 0)
                    {
                        rqs.metadata = new List<Metadata>();
                        foreach (Metadata m in request.metadata.metadata)
                        {
                            rqs.metadata.Add(m);
                        }
                    }
                }
            }

            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneralInfo>> response = ws.ExecuteTaskAsync<ResponseGeneralInfo>(rq);
            response.Wait();
            ResponseGeneralInfo data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneralInfo { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
                // return rqs;
            }

            // return rqs;

            return data;
        }


        public ResponseGeneral EnviarMetadataReceptor(EnviarMetadataReceptorRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionAks"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/integracion/metadata/enviar", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"Bearer {token}");
            SendMetadataReceptor rqs = new SendMetadataReceptor();
            rqs.metadata = new List<Metadata>();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;


            foreach (Metadata m in request.metadata.metadata)
            {
                rqs.metadata.Add(m);
            }

            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);



            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral EnviarMetadataEmisor(EnviarMetadataEmisorRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/integracion/metadata/enviarEmisor", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            SendMetadataEmisor rqs = new SendMetadataEmisor();
            rqs.metadata = new List<Metadata>();
            rqs.numerodocumento = request.numeroDocumento;
            foreach (Metadata m in request.metadata.metadata)
            {
                rqs.metadata.Add(m);
            }

            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }


        public ResponseGeneral EnviarRepGrafica(EnviarArchivoRequest request, MemoryCache memorycache, Boolean isrepgrafica = true)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);
            string ls_api = (isrepgrafica) ? "/api/archivos/repgrafica/enviar" : "/api/archivos/anexo/enviarAnexo";
            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            SendFileRequest rqs = new SendFileRequest();

            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.Archivo = request.archivo;
            rqs.Nombre = request.nombre;
            rqs.Extension = request.extension.Trim().ToLower();
            rqs.Visible = request.visible;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral EnviarRepGraficaReceptor(EnviarArchivoReceptorRequest request, MemoryCache memorycache, Boolean isrepgrafica = true)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);
            string ls_api = (isrepgrafica) ? "/api/archivos/repgrafica/enviarReceptor" : "/api/archivos/anexo/enviarAnexoReceptor";
            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            SendFileReceptorRequest rqs = new SendFileReceptorRequest();

            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.Archivo = request.archivo;
            rqs.Nombre = request.nombre;
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.Extension = request.extension.Trim().ToLower();
            rqs.Visible = request.visible;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral AcuseRecibo(ReceptorCambioEstatusRequest request, MemoryCache memorycache)
        {

            //Buscar tus valores de configuración donde los tengas guardados
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);
            //string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionAks"];



            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            RestRequest rq = new RestRequest("/api/gestion/documentos/cambiarestatus", Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            CambioEstatusRequest rqs = new CambioEstatusRequest
            {
                NitEmisor = request.identificadorEmisor,
                NumeroDocumento = request.numeroDocumento,
                Estatus = request.status,
                CodigoRechazo = request.codigoRechazo,
                TipoIdentificacionEmisor = request.tipoIdentificacionemisor,
                TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento,
                EjecutadoPor = new CambioEstatusRequest.EjecutadoPorRequest
                {
                    Apellido = request.EjecutadoPor.Apellido,
                    Cargo = request.EjecutadoPor.Cargo,
                    Departamento = request.EjecutadoPor.Departamento,
                    Nombre = request.EjecutadoPor.Nombre,
                    Identificacion = new CambioEstatusRequest.EjecutadoPorRequest.IdentificacionRequest
                    {
                        DV = request.EjecutadoPor.Identificacion.Dv,
                        NumeroIdentificacion = request.EjecutadoPor.Identificacion.NumeroIdentificacion,
                        TipoIdentificacion = request.EjecutadoPor.Identificacion.TipoIdentificacion
                    }
                }
            };
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public FileDownloadResponse DescargarArchivo(ReceptorRequestGeneral request, MemoryCache memorycache, int typefile)
        {

            /* typefile representa el tipo de archivo que se desea descargar
             * 1 = XML
             * 2 = Representación Gráfica
             * 3 = Acuse de Recibido
             * 4 = Acuse de Aceptación / Rechazo 
             */


            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new FileDownloadResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = (typefile == 1) ? "/api/archivos/xml/descargar" : ((typefile == 2) ? "/api/archivos/repgrafica/descargar" : ((typefile == 3) ? "/api/archivos/xml/descargar/acuse/recibidoxml" : "/api/archivos/xml/descargar/acuse/aceptacionxml"));

            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitRequest rqs = new DocumentoNitRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<FileDownloadResponse>> response = ws.ExecuteTaskAsync<FileDownloadResponse>(rq);
            response.Wait();
            FileDownloadResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new FileDownloadResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        /// <summary>
        /// HU-128
        /// Se agrega el campo TipoDocumento no es obligatorio en caso de ser null o estar vacio se establece por defecto en 01 Factura
        /// </summary>
        /// <param name="request"></param>
        /// <param name="memorycache"></param>
        /// <param name="typefile"></param>
        /// <returns></returns>
        public FileDownloadResponse DescargarArchivo(ReceptorRequestApplicationResponse request, MemoryCache memorycache, int typefile)
        {

            /* typefile representa el tipo de archivo que se desea descargar
             * 
             * 3 = Acuse de Recibido
             * 4 = Acuse de Aceptación 
             * 5 = Acuse de Reclamo
             * 6 = Acuse de Recepción de Bien o Prestación de Servicios
             */


            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //validar tipo de descarga

            if (!Regex.IsMatch(request.tipoDescarga, "^(1|2)$"))
            {
                return new FileDownloadResponse { codigo = 101, mensaje = "Tipo de descarga No Soportada; solo se admite uno de los siguientes valores: 1 y 2", resultado = "Error" };
            }

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new FileDownloadResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = (typefile == 1) ? "/api/archivos/xml/descargar" : ((typefile == 2) ? "/api/archivos/repgrafica/descargar" : ((typefile == 3) ? "/api/archivos/xml/descargar/acuse/recibidoxml" : (typefile == 4) ? "/api/archivos/xml/descargar/acuse/aceptacionxml" : (typefile == 5) ? "/api/archivos/xml/descargar/acuse/reclamoxml" : "/api/archivos/xml/descargar/acuse/recibidobienxml"));

            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            ApplicationResponseDownloadRequest rqs = new ApplicationResponseDownloadRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.nombre = request.nombreFileApplicationResponse;
            rqs.type_download = (request.tipoDescarga == null) ? 1 : int.Parse(request.tipoDescarga);
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<FileDownloadResponse>> response = ws.ExecuteTaskAsync<FileDownloadResponse>(rq);
            response.Wait();
            FileDownloadResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new FileDownloadResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }


        public FileDownloadResponse1 DescargarAnexo(ReceptorRequestAnexo request, MemoryCache memorycache)
        {
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];
            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new FileDownloadResponse1 { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = "/api/archivos/anexo/descargar";

            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitArchivoRequest rqs = new DocumentoNitArchivoRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.Nombre = request.identificadorInternoAnexo;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<FileDownloadResponse1>> response = ws.ExecuteTaskAsync<FileDownloadResponse1>(rq);
            response.Wait();
            FileDownloadResponse1 data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new FileDownloadResponse1 { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral EliminaAnexo(ReceptorRequestAnexo request, MemoryCache memorycache)
        {




            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = "/api/archivos/anexo/eliminar";

            RestRequest rq = new RestRequest(ls_api, Method.DELETE) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitArchivoRequest rqs = new DocumentoNitArchivoRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.Nombre = request.identificadorInternoAnexo;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }

        public ResponseGeneral EliminaRepGrafica(ReceptorRequestGeneral request, MemoryCache memorycache)
        {




            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];


            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseGeneral { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = "/api/archivos/repgrafica/eliminar";

            RestRequest rq = new RestRequest(ls_api, Method.DELETE) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitArchivoRequest rqs = new DocumentoNitArchivoRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ResponseGeneral>> response = ws.ExecuteTaskAsync<ResponseGeneral>(rq);
            response.Wait();
            ResponseGeneral data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ResponseGeneral { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }



        public ArchivoDocumentoResponse ListaAnexo(ReceptorRequestGeneral request, MemoryCache memorycache)
        {
            string urlRestIntegracion = WebConfigurationManager.AppSettings["urlRestIntegracionWSRecepcion"];

            //Se procede a autenticar
            //primero autenticarse
            string token = AutenticarRestIntegracion(request.tokenEmpresa, request.tokenPassword, memorycache);
            if (string.IsNullOrEmpty(token))
            {
                return new ArchivoDocumentoResponse { codigo = 101, mensaje = "Usuario No Autorizado - Datos Empresa no Válidos", resultado = "Error" };
            }

            RestClient ws = new RestClient(urlRestIntegracion);

            // Override with Newtonsoft JSON Handler

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);

            string ls_api = "/api/archivos/anexo/listaarchivo";

            RestRequest rq = new RestRequest(ls_api, Method.POST) { JsonSerializer = RecepcionJsonSerializer.Default };

            rq.AddHeader("Content-type", "application/json");
            rq.AddHeader("Accept", "application/json");

            rq.AddHeader("Authorization", $"bearer {token}");
            DocumentoNitRequest rqs = new DocumentoNitRequest();
            rqs.NitEmisor = request.identificadorEmisor;
            rqs.NumeroDocumento = request.numeroDocumento;
            rqs.tipoIdentificacionemisor = request.tipoIdentificacionemisor;
            rqs.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
            rq.AddJsonBody(rqs);
            //rq.Timeout = 500000;
            System.Threading.Tasks.Task<IRestResponse<ArchivoDocumentoResponse>> response = ws.ExecuteTaskAsync<ArchivoDocumentoResponse>(rq);
            response.Wait();
            ArchivoDocumentoResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return new ArchivoDocumentoResponse { codigo = 103, mensaje = "Se produjo un error en el servicio..." + response.Result.Content + " por favor inténtelo más tarde", resultado = "Error" };
            }

            return data;

            //return response.Data;
        }


        private static string AutenticarRestIntegracion(string tokenEnterprise, string tokenPassword, MemoryCache memorycache)
        {

            //Obtener los datos de tu repositorio de configuraciones
            // var cfg = new AdministradorDeConfiguracion<ApplicationDbContext>(_db);

            // var urlLoginIntegracion = await _configuration.GetValueOfAsync("UrlLoginIntegracion");
            //var apiLoginIntegracion = await cfg.GetValueOfAsync("ApiLoginIntegracion");

            string apiLoginIntegracion = "/api/account/Loginsoap";
            string urlLoginIntegracion = WebConfigurationManager.AppSettings["urlRestLoginRecepcion"];

            // Ejemplo 	http://testlogin.thefactoryhka.com.co



            //	Ejemplo /api/account/Loginsoap

            string usrRestIntegracion = tokenEnterprise;
            string pwdRestIntegracion = tokenPassword;

            //Necesitamos cachear el token
            //verificando que no esté en el cache:

            //Para obtner el token
            DateTime expiration;
            if (memorycache.TryGetValue($"jwt-i-{usrRestIntegracion}", out string token))
            {
                //chequeando la expiración
                if (memorycache.TryGetValue($"jwt-i-ed-{usrRestIntegracion}", out expiration))
                {
                    if (expiration > DateTime.Now)
                    {
                        return token;
                    }
                }
            }
            RestClient ws = new RestSharp.RestClient(urlLoginIntegracion); // http://testlogin.thefactoryhka.com.co

            /* Handlers */

            ws.AddHandler("application/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/x-json", RecepcionJsonSerializer.Default);
            ws.AddHandler("text/javascript", RecepcionJsonSerializer.Default);
            ws.AddHandler("*+json", RecepcionJsonSerializer.Default);



            RestRequest request = new RestSharp.RestRequest(apiLoginIntegracion, Method.POST); // /api/account/Loginsoap 
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Accept", "application/json");

            LoginIntegracionRequest loginfo = new LoginIntegracionRequest { Password = pwdRestIntegracion, User = usrRestIntegracion };
            request.AddJsonBody(loginfo);
            System.Threading.Tasks.Task<IRestResponse<LoginIntegracionResponse>> response = ws.ExecuteTaskAsync<LoginIntegracionResponse>(request);
            response.Wait();
            LoginIntegracionResponse data = !response.Result.IsSuccessful ? null : response.Result.Data;
            if (data == null)
            {
                return string.Empty;
            }
            token = data.Token;
            expiration = data.PasswordExpiration;
            //guardar en cache
            // store in the cache
            memorycache.Set($"jwt-i-{usrRestIntegracion}", token, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration));
            memorycache.Set($"jwt-i-ed-{usrRestIntegracion}", expiration, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration));
            return token;

        }

    }
}