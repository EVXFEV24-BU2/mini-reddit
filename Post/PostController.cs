/*

# Skapa inlägg
POST /post

# Hämta inlägg
GET /post/{page}

# Söka inlägg
GET /post/search

# Radera inlägg
DELETE /post/{postId}

# Redigera inlägg
PUT /post/{postId}

# Gilla inlägg
PUT /post/{postId}

# Ogilla inlägg
PUT /post/{postId}

*/

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/post")]
public class PostController : ControllerBase
{
    private readonly IPostService postService;
    private readonly ILogger<PostController> logger;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        this.postService = postService;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize("create_post")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = await postService.CreatePost(request, userId);
            return CreatedAtAction(nameof(CreatePost), PostResponse.FromModel(post));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error when creating post: {}", exception.Message);
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpGet("{page}")]
    public async Task<IEnumerable<PostResponse>> GetPosts(int page)
    {
        var posts = await postService.GetPage(page);
        return posts.Select(PostResponse.FromModel);
    }
}

public class CreatePostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class PostResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }

    //public ICollection<CommentEntity> Comments { get; set; }
    //public ICollection<ReactionEntity> Reactions { get; set; }

    public static PostResponse FromModel(PostEntity entity)
    {
        return new PostResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            CreatedDateTime = entity.CreatedDateTime,
            UserId = entity.User.Id,
            UserName = entity.User.UserName ?? "undefined",
        };
    }
}
