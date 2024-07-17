using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfRecepcionSOAP.Models.Responses;

namespace WcfRecepcionSOAP.Invitaciones
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service.svc or Service.svc.cs at the Solution Explorer and start debugging.
	public class Service : IService
	{
		public InvitacionResponse InvitarProveedor(String tokenEmpresa, String tokenPassword, String tipoContribuyente, String razonSocial, String nombre, String apellido, String tipoDocumentoIdentidad, String numeroIdentidad, String correo, String comentario)
		{
			return new InvitacionResponse {
				codigo = 200,
				mensaje = "Invitacion enviada al Proveedor"
			};
		}
	}
}
