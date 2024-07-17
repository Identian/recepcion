using Newtonsoft.Json;

namespace WcfRecepcionSOAP.Models.Requests
{
    public class SendFileRequest
    {
        [JsonProperty("numerodocumento", Required = Required.Always)]
        public string NumeroDocumento { get; set; }
        [JsonProperty("archivo", Required = Required.Always)]
        public byte[] Archivo { get; set; }
        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }
        [JsonProperty("extension", Required = Required.Always)]
        public string Extension { get; set; }
        [JsonProperty("visible", Required = Required.Always)]
        public string Visible { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
    public class SendFileReceptorRequest
    {
        [JsonProperty("numerodocumento", Required = Required.Always)]
        public string NumeroDocumento { get; set; }
        [JsonProperty("nitemisor", Required = Required.Always)]
        public string NitEmisor { get; set; }
        [JsonProperty("tipoIdentificacionemisor", Required = Required.Default)]
        public string tipoIdentificacionemisor { get; set; }
        [JsonProperty("archivo", Required = Required.Always)]
        public byte[] Archivo { get; set; }
        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }
        [JsonProperty("extension", Required = Required.Always)]
        public string Extension { get; set; }
        [JsonProperty("visible", Required = Required.Always)]
        public string Visible { get; set; }

        public string TipoDocumento { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}