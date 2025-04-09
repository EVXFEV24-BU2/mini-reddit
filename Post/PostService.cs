public interface IPostService
{
    public Task<PostEntity> CreatePost(CreatePostRequest request, string userId);
    public Task<IEnumerable<PostEntity>> GetPage(int page);
    public Task ReactPost(Guid postId, string userId, Reaction reactionType);
    public Task DeletePost(Guid postId, string userId);
}

public class PostService : IPostService
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public PostService(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task<PostEntity> CreatePost(CreatePostRequest request, string userId)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            throw new ArgumentException("Title may not be null or empty");
        }

        if (request.Title.Length < 5)
        {
            throw new ArgumentException("Title must be above 5 characters");
        }

        if (request.Content == null)
        {
            throw new ArgumentException("Content may not be null");
        }

        var user =
            await userRepository.GetById(userId)
            ?? throw new ApplicationException("Unexpected user error.");

        var post = new PostEntity(request.Title, request.Content, user);
        await postRepository.Add(post);

        return post;
    }

    public async Task DeletePost(Guid postId, string userId)
    {
        int deleted = await postRepository.Delete(postId, userId);
        if (deleted == 0)
        {
            throw new ArgumentNullException("Post not found");
        }
    }

    public async Task<IEnumerable<PostEntity>> GetPage(int page)
    {
        return await postRepository.GetPage(page);
    }

    public async Task ReactPost(Guid postId, string userId, Reaction reactionType)
    {
        await postRepository.UpdateOrAddReaction(postId, userId, reactionType);
    }
}
