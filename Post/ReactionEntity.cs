using System.ComponentModel.DataAnnotations;

public enum Reaction
{
    Like,
    Dislike,
    Heart,
}

public class ReactionEntity
{
    [Key]
    public Guid Id { get; set; }

    public PostEntity Post { get; set; }
    public UserEntity User { get; set; }
    public Reaction Type { get; set; }

    public ReactionEntity(PostEntity post, UserEntity user, Reaction type)
    {
        this.Id = Guid.NewGuid();
        this.Post = post;
        this.User = user;
        this.Type = type;
    }

    public ReactionEntity()
    {
        this.Post = null!;
        this.User = null!;
    }
}
