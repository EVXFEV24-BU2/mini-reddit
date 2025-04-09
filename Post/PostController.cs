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

# Reagera inlägg
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

    [HttpPut("{postId}")]
    public async Task<IActionResult> ReactPost(Guid postId, [FromQuery] Reaction reactionType)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await postService.ReactPost(postId, userId, reactionType);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error when reacting to post: {}", exception.Message);
            return StatusCode(500, "Unexpected error.");
        }
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await postService.DeletePost(postId, userId);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error when deleting post: {}", exception.Message);
            return StatusCode(500, "Unexpected error.");
        }
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
    public required Dictionary<Reaction, int> Reactions { get; set; }

    public ICollection<CommentResponse> Comments { get; set; }

    public static PostResponse FromModel(PostEntity entity)
    {
        var reactions = new Dictionary<Reaction, int>();
        // Kan göras med reflection
        reactions[Reaction.Like] = 0;
        reactions[Reaction.Dislike] = 0;
        reactions[Reaction.Heart] = 0;

        foreach (var reaction in entity.Reactions)
        {
            int currentCount = reactions.GetValueOrDefault(reaction.Type, 0);
            reactions[reaction.Type] = currentCount + 1;
        }

        return new PostResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            CreatedDateTime = entity.CreatedDateTime,
            UserId = entity.User.Id,
            UserName = entity.User.UserName ?? "undefined",
            Reactions = reactions,
            Comments = entity.Comments.Select(CommentResponse.FromModel).ToList(),
        };
    }
}
