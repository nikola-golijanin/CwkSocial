using AutoMapper;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class UpdateUserProfileBasicInfoCommandHandler : IRequestHandler<UpdateUserProfileBasicInfoCommand>
{
    private readonly DataContext _context;

    public UpdateUserProfileBasicInfoCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateUserProfileBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(profile => profile.UserProfileId == request.UserProfileId);

        var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
            request.Phone, request.DateOfBirth, request.CurrentCity);

        userProfile.UpdateBasicInfo(basicInfo);

        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
        return new Unit();
    }
}