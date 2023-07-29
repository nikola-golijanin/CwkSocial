namespace CwkSocial.Application.Enums;

public enum ErrorCode
{
    NotFound = 404,
    InternalServerError = 500,
    ValidationError = 101,
    UnknownError = 999,
    IdentityUserAlredyExists = 201,
    IdewntityUserDoesNotExist = 202,
    IncorrectPassword = 203,
    IdentityCreationFailed = 204
}