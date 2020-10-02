using AutoMapper;
using CourseSysAPI.Entities;
using CourseSysAPI.Models.Users;

namespace CourseSysAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<User, UserModel>();
        }
    }
}