using System.Security.Claims;
using BlogDataLibrary.Data;
using BlogDataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require a JWT unless marked [AllowAnonymous]
public class PostsController : ControllerBase
{
    private readonly SqlData _db;

    public PostsController(SqlData db)
    {
        _db = db;
    }

    // GET: /api/Posts
    [HttpGet]
    [AllowAnonymous] // Allow anyone to view all posts
    public ActionResult<List<ListPostModel>> List()
    {
        var posts = _db.ListPosts();
        return Ok(posts);
    }

    // GET: /api/Posts/{id}
    [HttpGet("{id:int}")]
    [AllowAnonymous] // Allow public viewing of details
    public ActionResult<PostModel?> Detail(int id)
    {
        var post = _db.ShowPostDetails(id);
        if (post == null)
            return NotFound(new { message = "Post not found." });

        return Ok(post);
    }

    // DTO (Data Transfer Object) for post creation
    public record CreatePostDto(string Title, string Body);

    // POST: /api/Posts
    [HttpPost]
    public IActionResult Create([FromBody] CreatePostDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Body))
        {
            return BadRequest(new { message = "Title and Body are required." });
        }

        // Extract the user ID from the JWT token
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
        {
            return Unauthorized(new { message = "Missing or invalid token." });
        }

        var post = new PostModel
        {
            Title = dto.Title,
            Body = dto.Body,
            DateCreated = DateTime.Now,
            UserId = int.Parse(userIdStr)
        };

        // Save the post to the database
        _db.AddPost(post);

        // Return a clear JSON response instead of “1”
        return Ok(new
        {
            message = "Post created successfully!",
            post = new
            {
                post.Title,
                post.Body,
                post.DateCreated,
                post.UserId
            }
        });
    }
}
