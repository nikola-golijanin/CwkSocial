using AutoMapper;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;

namespace CwkSocial.Application.MappingProfiles;

internal class UserProfileMap : Profile
{
    public UserProfileMap()
    {
        CreateMap<CreateUserCommand, BasicInfo>();
    }
}