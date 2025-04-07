public interface IUserService
{
    public Task<UserEntity> CreateUser(CreateUserRequest request);
}

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserEntity> CreateUser(CreateUserRequest request)
    {
        var user = new UserEntity(request.Username, request.Email);

        await userRepository.Add(user, request.Password);

        return user;
    }
}
