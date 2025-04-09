public interface ICommentService
{
    public Task<CommentEntity> CreateComment(Guid postId, string userId, string commentContent);
}

public class CommentService : ICommentService
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CommentService(
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IPostRepository postRepository
    )
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task<CommentEntity> CreateComment(
        Guid postId,
        string userId,
        string commentContent
    )
    {
        if (string.IsNullOrEmpty(commentContent))
        {
            throw new ArgumentException("Comments may not be null or empty");
        }

        var post =
            await postRepository.GetById(postId)
            ?? throw new ArgumentNullException("Post not found");

        var user =
            await userRepository.GetById(userId)
            ?? throw new ArgumentNullException("User not found");

        var comment = new CommentEntity(commentContent, user, post);
        await commentRepository.Add(comment);
        return comment;
    }
}
