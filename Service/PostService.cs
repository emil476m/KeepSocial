using Infastructure;
using Microsoft.Extensions.Logging;

namespace Service;

public class PostService
{
    private readonly PostRepository _postRepository;
    private readonly ILogger<PostService> _logger;

    public PostService(PostRepository postRepository, ILogger<PostService> logger)
    {
        _logger = logger;
        _postRepository = postRepository;
    }


    public Post createPost(Post post)
    {
        return _postRepository.createPost(post);
    }
}