﻿using AutoMapper;
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
            CreateMap<TempUser, User>();
            CreateMap<User, TempUser>();
        }
    }
}
