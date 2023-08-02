namespace CwkSocial.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserProfileId(this HttpContext httpContext) =>
        GetClaimByKey("UserProfileId",httpContext);
    
    public static Guid GetIdentityId(this HttpContext httpContext) =>
        GetClaimByKey("IdentityId",httpContext);
    
    private static Guid GetClaimByKey(string key, HttpContext context)
    {
        var claim = context.User.Claims
            .FirstOrDefault(c => c.Type == key)?.Value;
        
        return Guid.Parse(claim);
    }
}