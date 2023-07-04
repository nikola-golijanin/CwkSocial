using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.UserProfile.Requests;

public record UserProfileCreateOrUpdate
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required] [EmailAddress] public string EmailAddress { get; set; }

    public string Phone { get; set; }

    [Required] public DateTime DateOfBirth { get; set; }

    public string CurrentCity { get; set; }
}