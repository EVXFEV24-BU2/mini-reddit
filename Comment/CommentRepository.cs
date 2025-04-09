public interface ICommentRepository
{
    public Task Add(CommentEntity entity);
}

public class EFCommentRepository : ICommentRepository
{
    private readonly AppDbContext context;

    public EFCommentRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task Add(CommentEntity entity)
    {
        await context.Comments.AddAsync(entity);
        await context.SaveChangesAsync();
    }
}
