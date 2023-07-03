using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand>
{
    private readonly DataContext _context;

    public DeleteUserProfileCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(profile => profile.UserProfileId == request.UserProfileId);

        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();
        
        return new Unit();
    }
}