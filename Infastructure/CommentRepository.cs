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

    /*
     * creates a comment with the given data and returns it
     */
    public Comment createComment(Comment comment)
    {
        var insert = $@"insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (@postid,@authorid,@text,@imgurl,@created)
            returning
            id;
            ";
        var select =
            $@"select comments.id as {nameof(Comment.id)},
                    comments.post_id as {nameof(Comment.postId)},
                    comments.author_id as {nameof(Comment.authorId)},
                    comments.text as {nameof(Comment.text)},
                    comments.img_url as {nameof(Comment.imgUrl)},
                    comments.created as {nameof(Comment.created)},
                    u.name as {nameof(Comment.authorName)},
                    u.avatarUrl as {nameof(Comment.avatarUrl)}
                    from keepsocial.comments join keepsocial.users u on u.id = comments.author_id where comments.id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            var id =  conn.ExecuteScalar<int>(insert, new {postid = comment.postId, authorid = comment.authorId, text = comment.text, imgurl = comment.imgUrl, created = DateTimeOffset.UtcNow });
           
            return  conn.QueryFirst<Comment>(select, new  {id});;
        }
    }

    /*
     * returns comments that have the same postId and uses the ofset to know what has been loaded and limit to not load them all
     */
    public IEnumerable<Comment> getComents(int limit, int offset, int postId)
    {
        var sql = $@"select comments.id as {nameof(Comment.id)},
                     comments.post_id as {nameof(Comment.postId)},
                     comments.author_id as {nameof(Comment.authorId)},
                     comments.text as {nameof(Comment.text)},
                     comments.img_url as {nameof(Comment.imgUrl)},
                     comments.created as {nameof(Comment.created)},
                     u.name as {nameof(Comment.authorName)},
                     u.avatarUrl as {nameof(Comment.avatarUrl)}
                    from keepsocial.comments join keepsocial.users u on u.id = comments.author_id where comments.post_id = @postId 
                    order by created desc offset @offset limit @limit";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Comment>(sql, new { offset, limit, postId});
        }
    }

    /*
     * delets the comment that has the given id
     */
    public void deleteComment(int id)
    {
        var sql = $@"delete from keepsocial.comments where id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            conn.Execute(sql, new { id });
        }
    }

    /*
     * updates the comment with the given data and returns it 
     */
    public Comment updateComment(int id, string text, string imgurl)
    {
        var update = $@"Update keepsocial.comments set text = @text, img_url = @imgurl, created = @created where id = @id;";
        var select = $@"select comments.id as {nameof(Comment.id)},
                     comments.post_id as {nameof(Comment.postId)},
                     comments.author_id as {nameof(Comment.authorId)},
                     comments.text as {nameof(Comment.text)},
                     comments.img_url as {nameof(Comment.imgUrl)},
                     comments.created as {nameof(Comment.created)},
                     u.name as {nameof(Comment.authorName)},
                     u.avatarUrl as {nameof(Comment.avatarUrl)}
                    from keepsocial.comments join keepsocial.users u on u.id = comments.author_id where comments.id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            conn.Execute(update, new { id, text, imgurl, created = DateTimeOffset.UtcNow });
            return conn.QueryFirst<Comment>(select, new {id});
        }
    }
}