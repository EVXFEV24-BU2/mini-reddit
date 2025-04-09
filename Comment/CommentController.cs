/*

# Skapa kommentar
POST /comment

# Radera kommentar
DELETE /comment/{commentId}

*/

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentService commentService;
    private readonly ILogger<CommentController> logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        this.commentService = commentService;
        this.logger = logger;
    }

    [HttpPost("{postId}")]
    [Authorize("create_comment")]
    public async Task<IActionResult> CreateComment(Guid postId, [FromBody] string commentContent)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var comment = await commentService.CreateComment(postId, userId, commentContent);
            return CreatedAtAction(nameof(CreateComment), comment.Id);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error when commenting: {}", exception.Message);
            return StatusCode(500, "Unexpected error.");
        }
    }
}

public class CommentResponse
{
    public Guid Id { get; set; }
    public required string Content { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }

    public static CommentResponse FromModel(CommentEntity entity)
    {
        return new CommentResponse
        {
            Id = entity.Id,
            Content = entity.Content,
            UserId = entity.User.Id,
            UserName = entity.User.UserName ?? "undefined",
        };
    }
}
