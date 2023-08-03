namespace CwkSocial.Application.Enums;

public enum ErrorCode
{
    NotFound = 404,
    InternalServerError = 500,

    //Validation errors
    ValidationError = 101,

    
    //Infrastructure errors
    IdentityUserAlreadyExists = 201,
    IdentityUserDoesNotExist = 202,
    IncorrectPassword = 203,
    IdentityCreationFailed = 204,

    //Application errors
    PostUpdateNotPossible = 300,
    PostDeleteNotPossible = 301,

    UnknownError = 999,
}