namespace MiniReddit;

public class PostEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public UserEntity User { get; set; }
    public ICollection<CommentEntity> Comments { get; set; }
    public ICollection<ReactionEntity> Reactions { get; set; }

    public PostEntity(string title, string content, UserEntity user)
    {
        this.Id = Guid.NewGuid();
        this.CreatedDateTime = DateTime.UtcNow;
        this.Comments = new List<CommentEntity>();
        this.Reactions = new List<ReactionEntity>();
        this.Title = title;
        this.Content = content;
        this.User = user;
    }

    public PostEntity()
    {
        this.Title = string.Empty;
        this.Content = string.Empty;
        this.Comments = new List<CommentEntity>();
        this.Reactions = new List<ReactionEntity>();
        this.User = null!;
    }
}
