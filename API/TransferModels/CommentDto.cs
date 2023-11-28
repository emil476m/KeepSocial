using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class CommentDto
{
    public class PostDto
    {
        public int postId { get; set; }
        public int authorId { get; set; }
        [MinLength(3)]
        [MaxLength(500)]
        public string text { get; set; }
        public string? imgurl { get; set; }
        public DateTimeOffset? created { get; set; }
    }
}