namespace MiniReddit;

public class CommentEntity
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public UserEntity User { get; set; }
    public PostEntity Post { get; set; }
    public DateTime CreatedDateTime { get; set; }

    public CommentEntity(string content, UserEntity user, PostEntity post)
    {
        this.Id = Guid.NewGuid();
        this.CreatedDateTime = DateTime.UtcNow;
        this.Content = content;
        this.User = user;
        this.Post = post;
    }

    public CommentEntity()
    {
        this.Content = string.Empty;
        this.User = null!;
        this.Post = null!;
    }
}
