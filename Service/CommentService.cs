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
}