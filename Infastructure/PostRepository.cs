using Dapper;
using Npgsql;

namespace Infastructure;

public class PostRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }


    /*
     * creates a post with the given data and returns it
     */
    public Post createPost(Post post)
    {
        var insert = $@"insert into keepsocial.posts(author_id,text,img_url,created) values (@authorid,@text,@imgurl,@created)
            returning
            id;
            ";
        var select =
            $@"select posts.id as {nameof(Post.id)},
                      posts.author_id as {nameof(Post.authorId)},
                      posts.text as {nameof(Post.text)},
                      posts.img_url as {nameof(Post.imgUrl)},
                      posts.created as {nameof(Post.created)},
                      u.name as {nameof(Post.authorName)},
                      u.avatarUrl as {nameof(Post.avatarUrl)}
                      from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where posts.id = @id;";

        using (var conn = _dataSource.OpenConnection())
        {
            var id =  conn.ExecuteScalar<int>(insert, new { authorid = post.authorId, text = post.text, imgurl = post.imgUrl, created = DateTimeOffset.UtcNow });
            return conn.QueryFirst<Post>(select, new  {id});
        }
    }

    /*
     * returns posts with an offset so it knows where to start and a limit so it knows how many to return
     */
    public IEnumerable<Post> getposts(int limit, int offset)
    {
        var sql = $@"select posts.id as {nameof(Post.id)},
                      posts.author_id as {nameof(Post.authorId)},
                      posts.text as {nameof(Post.text)},
                      posts.img_url as {nameof(Post.imgUrl)},
                      posts.created as {nameof(Post.created)},
                      u.name as {nameof(Post.authorName)},
                      u.avatarUrl as {nameof(Post.avatarUrl)}
                      from keepsocial.posts join keepsocial.users u on u.id = posts.author_id order by created desc offset @offset limit @limit";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Post>(sql, new { offset = offset, limit });
        }
    }

    /*
     * returns the post with the given id
     */
    public Post getpost(int id)
    {
        var sql =
            $@"select posts.id as {nameof(Post.id)},
                      posts.author_id as {nameof(Post.authorId)},
                      posts.text as {nameof(Post.text)},
                      posts.img_url as {nameof(Post.imgUrl)},
                      posts.created as {nameof(Post.created)},
                      u.name as {nameof(Post.authorName)},
                      u.avatarUrl as {nameof(Post.avatarUrl)}
                      from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where posts.id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Post>(sql, new { id });
        }
    }

    /*
     * deletes the post with the given id and the comments to the post
     */
    public void deletePost(int id)
    {
        var deleteCommentsOnPost = $@"delete from keepsocial.comments where post_id = @id";
        var deletePost = $@"delete from keepsocial.posts where id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            conn.Execute(deleteCommentsOnPost, new { id });
            conn.Execute(deletePost, new { id });
        }
    }

    /*
     * updates the comment with the given data and returns it
     */
    public Post updatePost(int id, string text, string imgurl)
    {
        var update = $@"Update keepsocial.posts set text = @text, img_url = @imgurl, created = @created where id = @id;";
        var select = $@"select posts.id as {nameof(Post.id)},
                     posts.author_id as {nameof(Post.authorId)},
                     posts.text as {nameof(Post.text)},
                     posts.img_url as {nameof(Post.imgUrl)},
                     posts.created as {nameof(Post.created)},
                     u.name as {nameof(Post.authorName)},
                    u.avatarUrl as {nameof(Post.avatarUrl)}
                    from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where posts.id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            conn.Execute(update, new { id, text, imgurl, created = DateTimeOffset.UtcNow });
            return conn.QueryFirst<Post>(select, new {id});
        }
    }
    
    public IEnumerable<Post> getProfilePosts(int limit, int offset, int profileId)
    {
        var sql = $@"select posts.id as {nameof(Post.id)},
                      posts.author_id as {nameof(Post.authorId)},
                      posts.text as {nameof(Post.text)},
                      posts.img_url as {nameof(Post.imgUrl)},
                      posts.created as {nameof(Post.created)},
                      u.name as {nameof(Post.authorName)}
                      from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where author_id = @profileId order by created desc offset @offset limit @limit";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Post>(sql, new { offset = offset, limit, profileId });
        }
    }


    /*
     * gets post from the users that a user followers
     */
    public IEnumerable<Post> getFollowedPost(int id, int limit, int offset)
    {
        var sql =
            $@"select 
                posts.id as {nameof(Post.id)},
                posts.text as {nameof(Post.text)},
                posts.author_id as {nameof(Post.authorId)},
                posts.img_url as {nameof(Post.imgUrl)},
                posts.created as {nameof(Post.created)},
                u.name as {nameof(Post.authorName)},
                u.avatarurl as {nameof(Post.avatarUrl)} 
                from keepsocial.posts 
                    join keepsocial.followrelation f on followed_id = posts.author_id 
                    join keepsocial.users u on u.id = posts.author_id 
                    where f.follower_id = @id order by created desc offset @offset limit @limit;";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Post>(sql, new {id, limit, offset});
        }
    }
}