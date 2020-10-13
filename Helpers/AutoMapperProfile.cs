using AutoMapper;
using CourseSysAPI.Entities;
using CourseSysAPI.Models.Users;
using CourseSysAPI.Models.Courses;
using CourseSysAPI.Models.Materials;

namespace CourseSysAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<User, UserModel>();
            CreateMap<AddEditCourseModel, Course>();
            CreateMap<AddEditMaterialModel, CourseMaterial>();
            CreateMap<SubmitAssignmentModel, Assignment>();
        }
    }
}