﻿using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class PostCommentUpdate
{
    [Required]
    public string Text { get; set; }
    [Required]
    public string UserProfileId { get; set; }
}