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
        var insert = $@"insert into keepsocial.posts(author_id,text,img_url,created) values (@authorid,@text,@imgurl,@created)
            returning
            id;
            ";
        var select =
            $@"select posts.id,posts.author_id,posts.text,posts.img_url,posts.created,u.name from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where posts.id = @id;";

        using (var conn = _dataSource.OpenConnection())
        {
            var id =  conn.ExecuteScalar<int>(insert, new { authorid = post.author_id, text = post.text, imgurl = post.img_url, created = DateTimeOffset.UtcNow });
            return conn.QueryFirst<Post>(select, new  {id = id});
        }
    }

    public IEnumerable<Post> getposts(int limit, int offset)
    {
        var sql = $@"select posts.id,posts.author_id,posts.text,posts.img_url,posts.created,u.name from keepsocial.posts join keepsocial.users u on u.id = posts.author_id order by created desc offset @offset limit @limit";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Post>(sql, new { offset = offset, limit });
        }
    }

    public Post getpost(int id)
    {
        var sql =
            $@"select posts.id,posts.author_id,posts.text,posts.img_url,posts.created,u.name from keepsocial.posts join keepsocial.users u on u.id = posts.author_id where posts.id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Post>(sql, new { id });
        }
    }
}