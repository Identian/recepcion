using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
/* using RestSharp;
using WcfRecepcionSOAP.Models.Requests;
using WcfRecepcionSOAP.Models.Responses;*/
using WcfRecepcionSOAP.Base;

namespace WcfRecepcionSOAP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class EmisorWS : IEmisorWS
    {
        private MemoryCache _memorycache = new MemoryCache(new MemoryCacheOptions());

        public ResponseGeneral EnviarXML(EnviarXMLRequest request)
        {
            if (request == null)
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            else
            {
                var context = new ValidationContext(request, null, null);
                var result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    var de = new DocumentoElectronico();
                    var response = de.EnviarXML(request, _memorycache);
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

        public ResponseGeneral EnviarRepGrafica(EnviarArchivoRequest request)
        {
            if (request == null)
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            else
            {
                var context = new ValidationContext(request, null, null);
                var result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    if (request.extension.Trim().ToLower() == "pdf")
                    {
                        var de = new DocumentoElectronico();
                        var response = de.EnviarRepGrafica(request, _memorycache);
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

        public ResponseGeneral EnviarMetadata(EnviarMetadataEmisorRequest request)
        {
            if (request == null)
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            else
            {
                var context = new ValidationContext(request, null, null);
                var result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    var de = new DocumentoElectronico();
                    var response = de.EnviarMetadataEmisor(request, _memorycache);
                    return response;
                }
                else
                {
                    return new ResponseGeneral { codigo = 102, mensaje = result[0].ErrorMessage, resultado = "Error" };
                }
            }
        }

   

    public ResponseGeneral EnviarAnexo(EnviarArchivoRequest request)
        {
            if (request == null)
                return new ResponseGeneral { codigo = 105, mensaje = "Error en el servicio. Datos suministrados no pueden ser nulos", resultado = "Error" };
            else
            {
                var context = new ValidationContext(request, null, null);
                var result = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, context, result, true);
                if (isValid)
                {
                    var de = new DocumentoElectronico();
                    var response = de.EnviarRepGrafica(request, _memorycache, false);
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