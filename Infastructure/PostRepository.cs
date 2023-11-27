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


    public Post createPost(Post post)
    {
        var sql = $@"insert into keepsocial.posts(author_id,text,img_url,created) values (@authorid,@text,@imgurl,@created)
            returning
            id as {nameof(post.id)},
            author_id as {nameof(post.authorId)},
            text as {nameof(post.text)},
            img_url as {nameof(post.imgurl)},
            created as {nameof(post.created)}";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Post>(sql, new { authorid = post.authorId, text = post.text, imgurl = post.imgurl, created = post.created });
        }
    }
}