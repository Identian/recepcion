using Newtonsoft.Json;
using System.Collections.Generic;

namespace WcfRecepcionSOAP.Models.Requests
{
    public class ArchivoXML
    {
        [JsonProperty("archivo", Required = Required.Always)]
        public string Archivo { get; set; }
        [JsonProperty("metadata", Required = Required.Default)]
        public List<Metadata> metadata { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
    public class SendMetadataReceptor
    {
        [JsonProperty("NitEmisor", Required = Required.Always)]
        public string NitEmisor { get; set; }
        [JsonProperty("NumeroDocumento", Required = Required.Always)]
        public string NumeroDocumento { get; set; }
        [JsonProperty("Metadata", Required = Required.Always)]
        public List<Metadata> metadata { get; set; }
        [JsonProperty("TipoIdentificacionEmisor", Required = Required.Default)]
        public string tipoIdentificacionemisor { get; set; }
        public string TipoDocumento { get; set; }
        public int ApplicationType { get; set; } = 0;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
    public class SendMetadataEmisor
    {

        [JsonProperty("NumeroDocumento", Required = Required.Always)]
        public string numerodocumento { get; set; }
        [JsonProperty("metadata", Required = Required.Always)]
        public List<Metadata> metadata { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}