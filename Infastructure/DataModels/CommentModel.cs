using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Infastructure;

public class Comment
{
    public BigInteger id { get; set; }
    public int post_id { get; set; }
    public int author_id { get; set; }
    [MinLength(3)]
    [MaxLength(500)]
    public string text { get; set; }
    public string? img_url { get; set; }
    public DateTimeOffset created { get; set; }
    public string name { get; set; }
}