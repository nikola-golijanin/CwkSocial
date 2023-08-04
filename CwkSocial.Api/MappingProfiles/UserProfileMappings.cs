using AutoMapper;
using CwkSocial.Api.Contracts.UserProfile.Requests;
using CwkSocial.Api.Contracts.UserProfile.Responses;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;

namespace CwkSocial.Api.MappingProfiles;

public class UserProfileMappings : Profile
{
    public UserProfileMappings()
    {
        CreateMap<UserProfile, UserProfileResponse>();
        CreateMap<BasicInfo, BasicInformation>();
        CreateMap<UserProfileCreateOrUpdate, UpdateUserProfileBasicInfoCommand>();
        CreateMap<UserProfile, InteractionUser>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.BasicInfo.FirstName + " " + src.BasicInfo.LastName))
                .ForMember(dest => dest.City,
                    opt => opt.MapFrom(src => src.BasicInfo.CurrentCity));
    }
}