using Dapper;
using Npgsql;

namespace Infastructure;

public class CommentRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CommentRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public Comment createComment(Comment comment)
    {
        var insert = $@"insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (@postid,@authorid,@text,@imgurl,@created)
            returning
            id;
            ";
        var select =
            $@"select comments.id, comments.post_id,comments.author_id,comments.text,comments.img_url,comments.created,u.name from keepsocial.comments join keepsocial.users u on u.id = comments.author_id where comments.id = @id;";

        using (var conn = _dataSource.OpenConnection())
        {
            var id =  conn.ExecuteScalar<int>(insert, new {postid = comment.post_id, authorid = comment.author_id, text = comment.text, imgurl = comment.img_url, created = DateTimeOffset.UtcNow });
           
            return  conn.QueryFirst<Comment>(select, new  {id = id});;
        }
    }

    public IEnumerable<Comment> getComents(int limit, int offset, int postId)
    {
        var sql = $@"select comments.post_id,comments.author_id,comments.text,comments.img_url,comments.created,u.name from keepsocial.comments join keepsocial.users u on u.id = comments.author_id where comments.post_id = @postId order by created desc offset @offset limit @limit";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Comment>(sql, new { offset, limit, postId});
        }
    }
}