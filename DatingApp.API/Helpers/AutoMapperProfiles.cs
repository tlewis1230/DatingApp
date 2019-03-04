using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //<source (From), destination (to)>
            CreateMap<User, UserForListDto>()
                // PhotoUrl
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                // Age
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d => d.DateofBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
                 // PhotoUrl
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                // Age
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d => d.DateofBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(m=>m.SenderPhotoUrl, opt => opt
                .MapFrom(u =>u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m=>m.RecipientPhotoUrl, opt => opt
                .MapFrom(u =>u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}