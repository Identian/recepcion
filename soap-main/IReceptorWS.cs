using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfRecepcionSOAP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IReceptorWS
    {
        [OperationContract]
        EstatusDocumentoResponse EstadoDocumento(ReceptorRequestGeneral request);

        [OperationContract]
        ReporteResponse Reporte(ReceptorReporteRequest request);

        [OperationContract]
        ReporteStatusResponse ReporteStatus(ReceptorReporteStatusRequest request);

        [OperationContract]
        ResponseGeneral CambioEstatus(ReceptorCambioEstatusRequest request);

        [OperationContract]
        ResponseGeneral AcuseRecibo(ReceptorCambioEstatusRequest request);

        [OperationContract]
        FileDownloadResponse DescargarXML(ReceptorRequestGeneral request);

        [OperationContract]
        FileDownloadResponse DescargarRepGrafica(ReceptorRequestGeneral request);

        [OperationContract]
        FileDownloadResponse DescargarAcuseRecibidoXML(ReceptorRequestApplicationResponse request);

        [OperationContract]
        FileDownloadResponse DescargarAcuseAceptacionXML(ReceptorRequestApplicationResponse request);

        [OperationContract]
        FileDownloadResponse DescargarAcuseReclamoXML(ReceptorRequestApplicationResponse request);
        [OperationContract]
        FileDownloadResponse DescargarRecepcionBienServicioXML(ReceptorRequestApplicationResponse request);

        [OperationContract]
        FileDownloadResponse1 DescargarAnexo(ReceptorRequestAnexo request);

        [OperationContract]
        ArchivoDocumentoResponse ListaAnexo(ReceptorRequestGeneral request);

        [OperationContract]
        ResponseGeneral EliminarAnexo(ReceptorRequestAnexo request);

        [OperationContract]
        ResponseGeneral EliminarRepGrafica(ReceptorRequestGeneral request);

        [OperationContract]
        //ResponseGeneral EnviarXML(EnviarXMLRequest request);
        ResponseGeneralInfo EnviarXML(EnviarXMLRequest request);

        [OperationContract]
        ResponseGeneral EnviarRepGrafica(EnviarArchivoReceptorRequest request);
        [OperationContract]
        ResponseGeneral EnviarAnexo(EnviarArchivoReceptorRequest request);

        [OperationContract]
        ResponseGeneral EnviarMetadata(EnviarMetadataReceptorRequest request);

        [OperationContract]
        MetadataDocumentoResponse CosultaDocumentoMetadata(ReceptorRequestGeneral request);

    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    [DataContract]
    public class ReceptorRequestGeneral
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }
        [DataMember]
        public string TipoDocumento { get; set; }
    }

    public class ReceptorRequestAnexo
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Nombre Interno de Anexo Requerido")]
        [StringLength(96, ErrorMessage = "Longitud No Válida para el Nombre Interno de Anexo")]
        public string identificadorInternoAnexo { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }

        [DataMember]
        public string TipoDocumento { get; set; }

    }

    public class ReceptorRequestApplicationResponse
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [StringLength(96, ErrorMessage = "Longitud No Válida para el Nombre Interno Application Response")]
        public string nombreFileApplicationResponse { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }
        [DataMember]
        public string tipoDescarga { get; set; }

        [DataMember]
        public string TipoDocumento { get; set; }

    }
    public class ListMetadata
    {
        public List<Metadata> metadata { get; set; }
    }

    public class EnviarXMLRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Archivo es Requerido")]
        public string archivo { get; set; }
        [DataMember]
        public ListMetadata metadata { get; set; }

    }

    public class EnviarMetadataReceptorRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Información de Metadata Requerida")]
        public ListMetadata metadata { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }

        [DataMember]
        public string TipoDocumento { get; set; }

    }

    public class EnviarMetadataEmisorRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Información de Metadata Requerida")]
        public ListMetadata metadata { get; set; }

    }


    public class EnviarArchivoRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Archivo es Requerido")]
        public byte[] archivo { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Nombre del Archivo es Requerido")]
        [StringLength(100, ErrorMessage = "Longitud del Nombre del Archivo no debe superar los 100 caracteres")]
        public string nombre { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Extensión del Archivo es Requerida")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "La longitud de la extensión del archivo debe ser de 3 ó 4 caracteres")]
        [RegularExpression("^(png|bmp|jpg|pdf|doc|docx|xls|xlsx|ppt|pptx|rar|zip)$", ErrorMessage = "Extensiones válidas para el archivo son png|bmp|jpg|pdf|doc|docx|xls|xlsx|ppt|pptx|rar|zip")]
        public string extension { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El atributo sobre si el archivo será visible o no es Requerido")]
        [RegularExpression("^(0|1)$", ErrorMessage = "El Atributo Visible del Archivo sólo admite los valore 1 ó 0")]
        public string visible { get; set; }
    }

    public class EnviarArchivoReceptorRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Archivo es Requerido")]
        public byte[] archivo { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Nombre del Archivo es Requerido")]
        [StringLength(100, ErrorMessage = "Longitud del Nombre del Archivo no debe superar los 100 caracteres")]
        public string nombre { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Extensión del Archivo es Requerida")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "La longitud de la extensión del archivo debe ser de 3 ó 4 caracteres")]
        [RegularExpression("^(png|bmp|jpg|pdf|doc|docx|xls|xlsx|ppt|pptx|rar|zip)$", ErrorMessage = "Extensiones válidas para el archivo son png|bmp|jpg|pdf|doc|docx|xls|xlsx|ppt|pptx|rar|zip")]
        public string extension { get; set; }
        [DataMember]
        [Required(ErrorMessage = "El atributo sobre si el archivo será visible o no es Requerido")]
        [RegularExpression("^(0|1)$", ErrorMessage = "El Atributo Visible del Archivo sólo admite los valore 1 ó 0")]
        public string visible { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }
        [DataMember]
        public string TipoDocumento { get; set; }
    }
    public class ResponseGeneral
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }

    }

    public class ResponseGeneralInfo
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }
        [DataMember]
        public string documentoId { get; set; }
        [DataMember]
        public string numeroIdentificacion { get; set; }
        [DataMember]
        public string tipoIdentificacion { get; set; }
        [DataMember]
        public string estatusDocumento { get; set; }



    }

    public class ReceptorReporteRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [RegularExpression("^\\d+$", ErrorMessage = "El consecutivo debe ser un número natural")]
        public string consecutivo { get; set; }


    }

    public class ReceptorReporteStatusRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [RegularExpression("^\\d+$", ErrorMessage = "El consecutivo debe ser un número natural")]
        public string consecutivo { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Código de Estatus es Requerido")]
        [RegularExpression("^\\d{1,3}$", ErrorMessage = "El estado debe ser un entero entre 0 y 999")]
        public string status_code { get; set; }


    }

    public class EstatusDocumentoResponse
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }
        [DataMember]
        public string uuid { get; set; }
        [DataMember]
        public string estatusDocumento { get; set; }
        [DataMember]
        public string tipoDocumento { get; set; }
        [DataMember]
        public string fechaDocumento { get; set; }
        [DataMember]
        public string estatusDIANcodigo { get; set; }
        [DataMember]
        public string estatusDIANDescripcion { get; set; }
        [DataMember]
        public string estatusDIANfecha { get; set; }
        [DataMember]
        public List<HistorialEstatus> ListEstatusHistory;
    }

    public class Response
    {
        public string uuid { get; set; }
        [DataMember]
        public string estatusDocumento { get; set; }
        [DataMember]
        public string tipoDocumento { get; set; }
        [DataMember]
        public string fechaDocumento { get; set; }
        [DataMember]
        public string estatusDIANcodigo { get; set; }
        [DataMember]
        public string estatusDIANDescripcion { get; set; }
        [DataMember]
        public string estatusDIANfecha { get; set; }
        [DataMember]
        public List<HistorialEstatus> ListEstatusHistory;
    }

    public class ResponseDto
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }

        public Response Response { get; set; }
    }


    public class MetadataDocumentoResponse
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }
        [DataMember]
        public string uuid { get; set; }
        [DataMember]
        public string estatusDocumento { get; set; }
        [DataMember]
        public string tipoDocumento { get; set; }
        [DataMember]
        public string fechaDocumento { get; set; }
        [DataMember]
        public string estatusDIANcodigo { get; set; }
        [DataMember]
        public string estatusDIANDescripcion { get; set; }
        [DataMember]
        public string estatusDIANfecha { get; set; }
        [DataMember]
        public List<SPPostMetadata> ListMetadata { get; set; }
    }

    public class HistorialEstatus
    {
        [DataMember]
        public string fecha { get; set; }
        [DataMember]
        public int codigoEstatus { get; set; }
        [DataMember]
        public String Descripcion { get; set; }
        [DataMember]
        public String Comentarios { get; set; }
    }

    public class SPPostMetadata
    {

        [DataMember]
        public string createdat { get; set; }
        [DataMember]
        public string createdby { get; set; }
        [DataMember]
        public string updatedat { get; set; }
        [DataMember]
        public string updatedby { get; set; }
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string value { get; set; }

    }
    public class ReporteResponse
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public Boolean pendiente { get; set; }
        [DataMember]
        public int ultimoEnviado { get; set; }
        [DataMember]
        public List<InfoDocumento> documentoselectronicos;
        [DataMember]
        public string resultado { get; set; }
    }

    public class ReporteStatusResponse
    {
        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public Boolean pendiente { get; set; }
        [DataMember]
        public int ultimoEnviado { get; set; }
        [DataMember]
        public List<InfoDocumentoStatus> documentoselectronicos;
        [DataMember]
        public string resultado { get; set; }
    }
    public class InfoDocumento
    {
        [DataMember]
        public string cufe { get; set; }
        [DataMember]
        public string fechaemision { get; set; }
        [DataMember]
        public string fecharecepcion { get; set; }
        [DataMember]
        public string horaemision { get; set; }
        [DataMember]
        public string montototal { get; set; }
        [DataMember]
        public string numerodocumento { get; set; }
        [DataMember]
        public string numeroidentificacion { get; set; }
        [DataMember]
        public string razonsocial { get; set; }
        [DataMember]
        public string tipodocumento { get; set; }
        [DataMember]
        public string tipoemisor { get; set; }
        [DataMember]
        public string tipoidentidad { get; set; }
        [DataMember]
        public string estatusDIANcodigo { get; set; }
        [DataMember]
        public string estatusDIANdescripcion { get; set; }
        [DataMember]
        public string estatusDIANfecha { get; set; }
        [DataMember]
        public int correlativoempresa { get; set; }
        /* [DataMember]
         public String Comentarios { get; set; }*/
    }

    public class InfoDocumentoStatus
    {
        [DataMember]
        public string cufe { get; set; }

        [DataMember]
        public string estatus { get; set; }

        [DataMember]
        public string estatusnombre { get; set; }


        [DataMember]
        public string fechaemision { get; set; }
        [DataMember]
        public string fecharecepcion { get; set; }
        [DataMember]
        public string horaemision { get; set; }
        [DataMember]
        public string montototal { get; set; }
        [DataMember]
        public string numerodocumento { get; set; }
        [DataMember]
        public string numeroidentificacion { get; set; }
        [DataMember]
        public string razonsocial { get; set; }
        [DataMember]
        public string tipodocumento { get; set; }
        [DataMember]
        public string tipoemisor { get; set; }
        [DataMember]
        public string tipoidentidad { get; set; }
        [DataMember]
        public string estatusDIANcodigo { get; set; }
        [DataMember]
        public string estatusDIANdescripcion { get; set; }
        [DataMember]
        public string estatusDIANfecha { get; set; }
        [DataMember]
        public int correlativoempresa { get; set; }
        /* [DataMember]
         public String Comentarios { get; set; }*/
    }

    public class ReceptorCambioEstatusRequest
    {

        [DataMember]
        [Required(ErrorMessage = "Token de la Empresa es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token de Empresa No Válida")]
        public string tokenEmpresa { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Token Password es Requerido")]
        [StringLength(40, MinimumLength = 40, ErrorMessage = "Longitud del Token Password No Válida")]
        public string tokenPassword { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Número de Documento Requerido")]
        [StringLength(20, ErrorMessage = "Longitud del Número de Documento No Válida")]
        public string numeroDocumento { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Identificador Emisor Requerida")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Emisor")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Emisor")]
        public string identificadorEmisor { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Código de Cambio de Estado Requerido")]
        [RegularExpression(@"^\d{1,3}$", ErrorMessage = "Valor No Válido para el Código de Cambio de Estado")]
        public string status { get; set; }
        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Emisor")]
        [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
        public string tipoIdentificacionemisor { get; set; }

        [DataMember]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Código de Rechazo")]
        [RegularExpression("^(01|02|03|04)$", ErrorMessage = "Código de Rechazo No Soportado; solo se admite uno de los siguientes valores 01,02,03 y 04")]
        public string codigoRechazo { get; set; }

        [DataMember]
        public EjecutadoPorRequest EjecutadoPor { get; set; }

        [DataMember]
        public string TipoDocumento { get; set; }

        public class EjecutadoPorRequest
        {

            public string Nombre { get; set; }

            public string Apellido { get; set; }

            public IdentificacionRequest Identificacion { get; set; }

            public string Cargo { get; set; }

            public string Departamento { get; set; }

            public class IdentificacionRequest /*TODO: Las validaciones no están funcionando para las propiedades de este objeto */
            {
                [DataMember]
                [Required(ErrorMessage = "Identificador Ejecutor Requerida")]
                [StringLength(20, MinimumLength = 6, ErrorMessage = "Longitud No Válida para el Identificador del Ejecutor")]
                [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Valor No Válido para el Identificador del Ejecutor")]
                public string NumeroIdentificacion { get; set; }

                [DataMember]
                [StringLength(2, MinimumLength = 2, ErrorMessage = "Longitud No Válida para el Tipo de Identificador del Ejecutor")]
                [RegularExpression("^(11|12|13|21|22|31|41|42|91)$", ErrorMessage = "Tipo de Identificación del Ejecutor No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91")]
                public string TipoIdentificacion { get; set; }
                [DataMember]
                [Required(ErrorMessage = "Dígito Verificador del Ejecutor Requerido")]
                [StringLength(1, MinimumLength = 1, ErrorMessage = "Longitud No Válida para el Dígito Verificador del Ejecutor")]
                [RegularExpression(@"^[0-9]+$", ErrorMessage = "Valor No Válido para el Dígito Verificador del Ejecutor")]
                public string Dv { get; set; }
            }
        }
    }

    public class FileDownloadResponse1
    {

        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }
        [DataMember]
        public string archivo { get; set; }
        [DataMember]
        public string cufe { get; set; }
        [DataMember]
        public string crc { get; set; }
        [DataMember]
        public string extension { get; set; }
        [DataMember]
        public string displayname { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int? idfile { get; set; }
        [DataMember]
        public int? sizefile { get; set; }
    }


    public class FileDownloadResponse
    {

        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }
        [DataMember]
        public string archivo { get; set; }
        [DataMember]
        public string cufe { get; set; }
        [DataMember]
        public string crc { get; set; }
        [DataMember]
        public int? size { get; set; }
    }

    public class ArchivoDocumento
    {
        [DataMember]
        public int InvoiceId { get; set; }
        [DataMember]
        public int InvoiceFileId { get; set; }
        [DataMember]
        public string NameFile { get; set; }
        [DataMember]
        public string PathFile { get; set; }
        [DataMember]
        public string Format { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public Boolean ProviderVisible { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public DateTime UpdatedAt { get; set; }
        [DataMember]
        public string NameDisplay { get; set; }
        [DataMember]
        public int? TypeCode { get; set; }
        [DataMember]
        public int? size { get; set; }
    }

    public class ArchivoDocumentoResponse
    {

        [DataMember]
        public int codigo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string resultado { get; set; }

        [DataMember]
        public List<ArchivoDocumento> ListArchivoDocumento { get; set; }
    }

    public class Metadata : IEquatable<Metadata>
    {
        [DataMember]
        [Required(ErrorMessage = "Código de Metadato es Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Valor No Válido para Código de Metadato")]
        [JsonProperty("Code", Required = Required.Default)]
        public string code { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Valor del Metadato es Requerido")]
        [JsonProperty("Value", Required = Required.Default)]
        public string value { get; set; }
        [DataMember]
        [JsonProperty("Internal1", Required = Required.Default)]
        public string internal1 { get; set; }
        [DataMember]
        [JsonProperty("Internal2", Required = Required.Default)]
        public string internal2 { get; set; }

        [JsonProperty("Id")]
        protected string Id { get; set; } = string.Empty;
        public bool Equals(Metadata other)
        {

            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            //Check whether the products' properties are equal. 
            return code.Equals(other.code) && value.Equals(other.value);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null. 
            int hashvalue = value == null ? 0 : value.GetHashCode();

            //Get hash code for the Code field. 
            int hashcode = code.GetHashCode();

            //Calculate the hash code for the product. 
            return hashvalue ^ hashcode;
        }


    }


}

