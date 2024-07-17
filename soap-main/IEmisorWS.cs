using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfRecepcionSOAP
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IEmisorWS
    {

        [OperationContract]
        ResponseGeneral EnviarXML(EnviarXMLRequest request);

        [OperationContract]
        ResponseGeneral EnviarRepGrafica(EnviarArchivoRequest request);

        [OperationContract]
        ResponseGeneral EnviarAnexo(EnviarArchivoRequest request);

        [OperationContract]
        ResponseGeneral EnviarMetadata(EnviarMetadataEmisorRequest request);
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.


}
