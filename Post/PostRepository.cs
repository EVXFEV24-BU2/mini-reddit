using Microsoft.EntityFrameworkCore;

public interface IPostRepository
{
    public Task Add(PostEntity entity);
    public Task<IEnumerable<PostEntity>> GetPage(int page);
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

    public async Task<IEnumerable<PostEntity>> GetPage(int page)
    {
        // TODO: Include comment and reactions
        return await context
            .Posts.Include(model => model.User)
            .Skip(page * 5)
            .Take(5)
            .ToListAsync();
    }
}
