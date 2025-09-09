using AutoMapper;
using SimpleMarketplace.Api.DTOs;
using SimpleMarketplace.Api.Entities;

namespace SimpleMarketplace.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<Usuario, UsuarioDetailDto>();
            CreateMap<CrearUsuarioDto, Usuario>()
                .ForMember(dest => dest.ContrasenaHash, opt => opt.Ignore());

            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.Imagenes, opt => opt.MapFrom(src => 
                    new List<string> { src.ImagenUrl }
                        .Concat(new[] { src.ImagenUrl2, src.ImagenUrl3, src.ImagenUrl4, src.ImagenUrl5, src.ImagenUrl6, src.ImagenUrl7 }
                            .Where(url => !string.IsNullOrEmpty(url)))
                        .ToList()
                ));
            CreateMap<CrearProductoDto, Producto>();
            CreateMap<CrearProductoDto, Producto>()
                .ForMember(dest => dest.ImagenUrl2, opt => opt.MapFrom(src => src.ImagenUrl2))
                .ForMember(dest => dest.ImagenUrl3, opt => opt.MapFrom(src => src.ImagenUrl3))
                .ForMember(dest => dest.ImagenUrl4, opt => opt.MapFrom(src => src.ImagenUrl4))
                .ForMember(dest => dest.ImagenUrl5, opt => opt.MapFrom(src => src.ImagenUrl5))
                .ForMember(dest => dest.ImagenUrl6, opt => opt.MapFrom(src => src.ImagenUrl6))
                .ForMember(dest => dest.ImagenUrl7, opt => opt.MapFrom(src => src.ImagenUrl7));
            
            CreateMap<Administrador, AdministradorDto>();
            CreateMap<CrearAdministradorDto, Administrador>()
                .ForMember(dest => dest.ContrasenaHash, opt => opt.Ignore());

            CreateMap<Configuracion, ConfiguracionDto>();
            CreateMap<CrearConfiguracionDto, Configuracion>();

            CreateMap<LogAdministrativo, LogAdministrativoDto>();
            CreateMap<CrearLogAdministrativoDto, LogAdministrativo>();
            
            CreateMap<Direccion, DireccionDto>();
            CreateMap<CrearDireccionDto, Direccion>();

            CreateMap<MetodoPago, MetodoPagoDto>()
                .ForMember(dest => dest.Titular, opt => opt.MapFrom(src => src.Usuario.Nombre));
            CreateMap<CrearMetodoPagoDto, MetodoPago>();

            CreateMap<CarritoItem, CarritoItemDto>();
            CreateMap<AgregarCarritoDto, CarritoItem>();

            CreateMap<DetallePedido, DetallePedidoDto>();

            CreateMap<Pedido, PedidoDto>();
            CreateMap<Pedido, PedidoWithUsuarioDto>()
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario));
            CreateMap<CrearPedidoCompletoDto, Pedido>();

            CreateMap<Factura, FacturaDto>()
                .ForMember(dest => dest.Pedido, opt => opt.MapFrom(src => src.Pedido));

            CreateMap<Comentario, ComentarioDto>()
                .ForMember(dest => dest.Texto, opt => opt.MapFrom(src => src.Texto))
                .ForMember(dest => dest.FechaComentario, opt => opt.MapFrom(src => src.FechaComentario));
            CreateMap<CrearComentarioDto, Comentario>()
                .ForMember(dest => dest.Texto, opt => opt.MapFrom(src => src.Texto));
        }
    }
}
