﻿using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class PostCreate
{
    [Required]
    [MaxLength(500)]
    public string TextContent { get; set; }
}