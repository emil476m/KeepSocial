﻿using System.ComponentModel.DataAnnotations;

namespace Infastructure;

public class Post
{
    public int id { get; set; }
    public int authorId { get; set; }
    [MinLength(3)]
    [MaxLength(500)]
    public string text { get; set; }
    public string? imgUrl { get; set; }
    public DateTimeOffset created { get; set; }
    public string authorName { get; set; }
    public string avatarUrl { get; set; }
}