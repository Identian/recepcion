using AutoMapper;
using CapaDominio.Receptor;
using CapaDominio.RequestReceptor;
using DTO.ConsultarEmailDto;
using DTO.EmailPing;
using DTO.GestionCorreoDto;
using DTO.RegistrarEmailDto;

namespace DTO.Automapper
{
    public class MapperProfiles : Profile
    {
        /// <summary>
        /// Definición de perfiles para la libreria AutoMapper
        /// </summary>
        public MapperProfiles()
        {
            CreateMap<ReceptorDto, Receptor>();

            CreateMap<CuentaCorreoDto, CuentaCorreo>()
                 .ForMember(des => des.TipoAutenticacion, o => new TipoAutenticacionConverter());

            CreateMap<EmailPingDto, CuentaCorreo>();

            //Inactivar correo
            CreateMap<ConsultarCorreoDto, ConsultaEmail>()
                .ForMember(des => des.NumeroIdentificacion, o => o.MapFrom(or => or.NIT));

            //consultar correo
            CreateMap<ConsultarCorreoParamsDto, ConsultaEmail>()
                .ForMember(des => des.NumeroIdentificacion, o => o.MapFrom(or => or.numeroIdentificacion))
                .ForMember(des => des.TipoIdentificacion, o => o.MapFrom(or => or.tipoIdentificacion));

            CreateMap<GuardarEmailDto, CuentaCorreoGuardar>()
                .ForMember(des => des.AccessToken, o => o.MapFrom(or => or.access_token))
                .ForMember(des => des.RefreshToken, o => o.MapFrom(o => o.refresh_token))
                .ForMember(des => des.Expires_in, o => o.MapFrom(o => o.expires_in))
                .ForMember(des => des.Ext_expires_in, o => o.MapFrom(o => o.ext_expires_in))
                .ForMember(des => des.TipoAutenticacion, o => new TipoAutenticacionConverter());

            CreateMap<Receptor, ReceptorBase>()
                .ForMember(des => des.IdReceptor, o => o.MapFrom(or => or.IdReceptor))
                .ForMember(des => des.NumeroIdentificacionReceptor, o => o.MapFrom(or => or.NumeroIdentificacionReceptor))
                .ForMember(des => des.TipoIdentificacionReceptor, o => o.MapFrom(or => or.TipoIdentificacionReceptor))
                .ForMember(des => des.TokenEnterprise, o => o.MapFrom(or => or.TokenEnterprise))
                .ForMember(des => des.TokenPassword, o => o.MapFrom(or => or.TokenPassword));
        }
    }
}
