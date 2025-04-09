using Microsoft.EntityFrameworkCore;

public interface IPostRepository
{
    public Task Add(PostEntity entity);
    public Task<IEnumerable<PostEntity>> GetPage(int page);
    public Task UpdateOrAddReaction(Guid postId, string userId, Reaction reactionType);
    public Task<int> Delete(Guid postId, string userId);
    public Task<PostEntity?> GetById(Guid postId);
}

public class EFPostRepository : IPostRepository
{
    private readonly AppDbContext context;

    public EFPostRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task Add(PostEntity entity)
    {
        await context.Posts.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<int> Delete(Guid postId, string userId)
    {
        int deleted = await context
            .Posts.Where(post => post.Id.Equals(postId) && post.User.Id.Equals(userId))
            .ExecuteDeleteAsync();

        return deleted;
    }

    public async Task<PostEntity?> GetById(Guid postId)
    {
        return await context.Posts.FindAsync(postId);
    }

    public async Task<IEnumerable<PostEntity>> GetPage(int page)
    {
        // TODO: Include comment and reactions
        return await context
            .Posts.Include(model => model.User)
            .Include(model => model.Reactions)
            .Include(model => model.Comments)
            .ThenInclude(comment => comment.User)
            .Skip(page * 5)
            .Take(5)
            .ToListAsync();
    }

    public async Task UpdateOrAddReaction(Guid postId, string userId, Reaction reactionType)
    {
        var reaction = await context
            .Reactions.Where(reaction =>
                reaction.Post.Id.Equals(postId) && reaction.User.Id.Equals(userId)
            )
            .FirstOrDefaultAsync();

        if (reaction == null)
        {
            var post =
                await context.Posts.FindAsync(postId)
                ?? throw new ArgumentNullException("Post not found.");

            var user =
                await context.Users.FindAsync(userId)
                ?? throw new ArgumentNullException("User not found.");

            await context.Reactions.AddAsync(new ReactionEntity(post, user, reactionType));
            await context.SaveChangesAsync();
        }
        else
        {
            reaction.Type = reactionType;
            await context.SaveChangesAsync();
        }
    }
}
