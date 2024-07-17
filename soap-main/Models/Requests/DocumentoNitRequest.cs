using Newtonsoft.Json;

namespace WcfRecepcionSOAP.Models.Requests
{
    public class DocumentoNitRequest
    {
        [JsonProperty("nitemisor", Required = Required.Always)]
        public string NitEmisor { get; set; }

        [JsonProperty("numerodocumento", Required = Required.Always)]
        public string NumeroDocumento { get; set; }
        [JsonProperty("tipoIdentificacionemisor", Required = Required.Default)]
        public string tipoIdentificacionemisor { get; set; }

        public string TipoDocumento { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
    public class DocumentoNitArchivoRequest
    {
        [JsonProperty("nitemisor", Required = Required.Always)]
        public string NitEmisor { get; set; }

        [JsonProperty("numerodocumento", Required = Required.Always)]
        public string NumeroDocumento { get; set; }

        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }

        [JsonProperty("tipoIdentificacionemisor", Required = Required.Default)]
        public string tipoIdentificacionemisor { get; set; }

        public string TipoDocumento { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
    public class ApplicationResponseDownloadRequest
    {

        public string NitEmisor { get; set; }
        public string NumeroDocumento { get; set; }
        public string nombre { get; set; }
        public string tipoIdentificacionemisor { get; set; }
        public int? type_download { get; set; }
        public string TipoDocumento { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}