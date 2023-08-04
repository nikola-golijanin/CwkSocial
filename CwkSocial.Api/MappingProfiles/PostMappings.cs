using AutoMapper;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Domain.Aggregate.PostAggregate;

namespace CwkSocial.Api.MappingProfiles;

public class PostMappings : Profile
{
    public PostMappings()
    {
        CreateMap<Post, PostResponse>();
        CreateMap<PostComment, PostCommentResponse>();
        CreateMap<PostInteraction, PostInteractionResponse>()
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => src.InteractionType.ToString()))
            .ForMember(dest => dest.Author,
                opt  => opt.MapFrom(src => src.UserProfile));
    }
}