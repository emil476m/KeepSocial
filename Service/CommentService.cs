using Infastructure;
using Microsoft.Extensions.Logging;

namespace Service;

public class CommentService
{
    private readonly CommentRepository _commentRepository;
    private readonly ILogger<CommentService> _logger;

    public CommentService(CommentRepository commentRepository, ILogger<CommentService> logger)
    {
        _logger = logger;
        _commentRepository = commentRepository;
    }
    
    /*
     * sends a comment to the commentRepository and returns that comment once created
     */
    public Comment createComment(Comment comment)
    {
        return _commentRepository.createComment(comment);
    }

    /*
     * gets comments from the comment repository depending on the variables limit, offset and postId
     */
    public IEnumerable<Comment> getComments(int limit, int offset, int postId)
    {
        return _commentRepository.getComents(limit, offset, postId);
    }

    /*
     * sends an id to the comment repository
     */
    public void deleteComment(int id)
    {
        _commentRepository.deleteComment(id);
    }

    /*
     * sends updated data to the comment repository and returns the comment but with the new data
     */
    public Comment UpdateComment(int id, string text, string imgUrl)
    {
        return _commentRepository.updateComment(id, text, imgUrl);
    }
}