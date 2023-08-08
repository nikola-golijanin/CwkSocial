namespace CwkSocial.Application.Enums;

public enum ErrorCode
{
    NotFound = 404,
    InternalServerError = 500,

    //Validation errors
    ValidationError = 101,

    
    //Infrastructure errors
    IdentityCreationFailed = 204,

    //Application errors
    PostUpdateNotPossible = 300,
    PostDeleteNotPossible = 301,
    InteractionRemovalNotAuthorized = 302,
    IdentityUserAlreadyExists = 303,
    IdentityUserDoesNotExist = 304,
    IncorrectPassword = 305,
    UnauthorizedAccountRemoval = 306,

    UnknownError = 999,
}