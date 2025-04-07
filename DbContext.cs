using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<PostEntity> Posts => Set<PostEntity>();
    public DbSet<CommentEntity> Comments => Set<CommentEntity>();
    public DbSet<ReactionEntity> Reactions => Set<ReactionEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PostEntity>().HasMany(model => model.Reactions);

        builder.Entity<UserEntity>().HasMany(model => model.Reactions);
    }
}
