using Microsoft.AspNetCore.Identity;
namespace MiniReddit;
public interface IUserRepository
{
    public Task Add(UserEntity entity, string password);
    public Task<UserEntity?> GetById(string userId);
}

public class EFUserRepository : IUserRepository
{
    private readonly AppDbContext context;
    private readonly UserManager<UserEntity> userManager;

    public EFUserRepository(AppDbContext context, UserManager<UserEntity> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task Add(UserEntity entity, string password)
    {
        var result = await userManager.CreateAsync(entity);
        if (!result.Succeeded)
        {
            throw new IdentityCoreException(result.Errors);
        }

        result = await userManager.AddPasswordAsync(entity, password);
        if (!result.Succeeded)
        {
            await userManager.DeleteAsync(entity);
            throw new IdentityCoreException(result.Errors);
        }
    }

    public async Task<UserEntity?> GetById(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }
}
