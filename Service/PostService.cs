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

    /*
     * sends a post to the postRepository and returns that post once created
     */
    public Post createPost(Post post)
    {
        return _postRepository.createPost(post);
    }
    /*
     * gets posts depending on the variables offset and limit
     */
    public IEnumerable<Post> getposts(int limit, int offset)
    {
        return _postRepository.getposts(limit, offset);
    }

    /*
     * gets a post based on it's id
     */
    public Post getpost(int id)
    {
        return _postRepository.getpost(id);
    }

    /*
     * sends a id to a post to the postRepository
     */
    public void deletePost(int id)
    {
        _postRepository.deletePost(id);
    }

    /*
     * sends updated data to the post repository and returns the post but with the new data
     */
    public Post UpdatePost(int id, string dtoText, string? dtoImgurl)
    {
        return _postRepository.updatePost(id, dtoText, dtoImgurl);
    }
    
    public IEnumerable<Post> getProfileposts(int limit, int offset, int profileId)
    {
        return _postRepository.getProfilePosts(limit, offset, profileId);
    }
}