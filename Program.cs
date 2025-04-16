using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MiniReddit;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, EFUserRepository>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IPostRepository, EFPostRepository>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                "Host=localhost;Port=5432;Database=minireddit;Username=postgres;Password=password"
            )
        );

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "create_post",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );

            options.AddPolicy(
                "create_comment",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
        });

        builder
            .Services.AddIdentityCore<UserEntity>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapIdentityApi<UserEntity>();

        app.Run();
    }
}

// Post
// Comment
// User
// Utility
// Models
// Exceptions
