using XDate.BackEnd.Models;

using AutoMapper;
using System.Linq;
using XDate.BackEnd.Dtos;

namespace XDate.BackEnd.Helpers
{
        public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User,UserForListDto>()
                .ForMember(dest=>dest.PhotoUrl, opt=> {
                    opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                })
                .ForMember(dest=>dest.Age, opt => 
                    opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<User,UserForDetailedDto>()
                .ForMember(dest=>dest.PhotoUrl, opt=> {
                    opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                })
                .ForMember(dest=>dest.Age, opt => 
                    opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoForDetailedDto>();
        }
    }
}