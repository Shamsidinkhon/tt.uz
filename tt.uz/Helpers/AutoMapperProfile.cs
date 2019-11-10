using AutoMapper;
using tt.uz.Dtos;
using tt.uz.Entities;

namespace tt.uz.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<TempUser, TempUserDto>();
            CreateMap<TempUserDto, TempUser>();
            CreateMap<TempUser, User>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<User, TempUser>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<NewsDTO, News>();
            CreateMap<ExternalLoginDTO, ExternalLogin>().ForMember(x => x.Id, opt => opt.Ignore()).ForMember(x => x.ClientId, opt => opt.MapFrom(src => src.Id));
            CreateMap<Category, CategoryDTO>().ForMember(x => x.Value, opt => opt.MapFrom(src => src.Id)).ForMember(x => x.Label, opt => opt.MapFrom(src => src.Name));
            CreateMap<TariffDTO, Tariff>();
        }
    }
}
