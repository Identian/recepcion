using Newtonsoft.Json;

namespace WcfRecepcionSOAP.Models.Requests
{
    public class CambioEstatusRequest
    {
        public string NumeroDocumento { get; set; }
        public string Estatus { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public string TipoDocumento { get; set; }
        public string NitEmisor { get; set; }
        public string TipoIdentificacionEmisor { get; set; }
        public string CodigoRechazo { get; set; }
        public EjecutadoPorRequest EjecutadoPor { get; set; }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public class EjecutadoPorRequest
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public IdentificacionRequest Identificacion { get; set; }
            public string Cargo { get; set; }
            public string Departamento { get; set; }

            public class IdentificacionRequest
            {
                public string NumeroIdentificacion { get; set; }
                public string TipoIdentificacion { get; set; }
                public string DV { get; set; }
            }
        }
    }
}