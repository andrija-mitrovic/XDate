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
            CreateMap<UserForUpdateDto,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<UserForRegisterDto,User>();
            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message,MessageToReturnDto>()
                .ForMember(d=>d.SenderPhotoUrl, opt => {
                    opt.MapFrom(s => s.Sender.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(d=>d.RecipientPhotoUrl, opt => {
                    opt.MapFrom(s => s.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url);
                });
        }
    }
}