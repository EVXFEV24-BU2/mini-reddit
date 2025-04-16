using Microsoft.AspNetCore.Identity;
namespace MiniReddit;
public class UserEntity : IdentityUser
{
    public ICollection<PostEntity> Posts { get; set; }
    public ICollection<CommentEntity> Comments { get; set; }
    public ICollection<ReactionEntity> Reactions { get; set; }

    public UserEntity(string username, string email)
        : base(username)
    {
        this.Email = email;
        this.Posts = new List<PostEntity>();
        this.Comments = new List<CommentEntity>();
        this.Reactions = new List<ReactionEntity>();
    }

    public UserEntity()
    {
        this.Posts = new List<PostEntity>();
        this.Comments = new List<CommentEntity>();
        this.Reactions = new List<ReactionEntity>();
    }
}
