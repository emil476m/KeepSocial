using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Infastructure;

public class Comment
{
    public int id { get; set; }
    public int postId { get; set; }
    public int authorId { get; set; }
    [MinLength(3)]
    [MaxLength(500)]
    public string text { get; set; }
    public string? imgUrl { get; set; }
    public DateTimeOffset created { get; set; }
    public string authorName { get; set; }
    public string avatarUrl { get; set; }
}