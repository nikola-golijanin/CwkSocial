using AutoMapper;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public CreateUserCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
            request.Phone, request.DateOfBirth, request.CurrentCity);

        var userProfile = UserProfile.CreateUserProfile(Guid.NewGuid().ToString(), basicInfo);
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();

        var result = new OperationResult<UserProfile>();
        result.Payload = userProfile;
        return result;
    }
}