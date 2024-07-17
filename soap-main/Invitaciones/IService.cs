using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfRecepcionSOAP.Models.Responses;

namespace WcfRecepcionSOAP.Invitaciones
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		InvitacionResponse InvitarProveedor(String tokenEmpresa, String tokenPassword, String tipoContribuyente, String razonSocial, String nombre, String apellido, String tipoDocumentoIdentidad, String numeroIdentidad, String correo, String comentario);
	}
}
