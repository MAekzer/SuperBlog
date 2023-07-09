using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlogApi.Services;
using SuperBlogData.Extentions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.Responses;
using SuperBlogData.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperBlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IRepository<Comment> commentRepo;
        private readonly IMapper mapper;
        private readonly ResponseBuilder responseBuilder;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Post> postRepo;

        public CommentController
            (IRepository<Comment> commentRepo,
            IMapper mapper,
            ResponseBuilder responseBuilder,
            UserManager<User> userManager,
            IRepository<Post> postRepo)
        {
            this.commentRepo = commentRepo;
            this.mapper = mapper;
            this.responseBuilder = responseBuilder;
            this.userManager = userManager;
            this.postRepo = postRepo;
        }

        // GET: api/<CommentController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var comments = await commentRepo.GetAll().ToListAsync();
                var response = comments.Select(mapper.Map<CommentResponse>);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                var comment = await commentRepo.GetByIdAsync(guid);
                if (comment == null)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                var response = mapper.Map<CommentResponse>(comment);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // POST api/<CommentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentPostRequest request)
        {
            try
            {
                var validUserGuid = Guid.TryParse(request.UserId, out var userGuid);
                if (!validUserGuid)
                    return NotFound(new { errorMessage = $"User with id {request.UserId} was not found" });

                var user = await userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return NotFound(new { errorMessage = $"User with id {request.UserId} was not found" });

                var validPostGuid = Guid.TryParse(request.PostId, out var postGuid);
                if (!validPostGuid)
                    return NotFound(new { errorMessage = $"Post with id {request.UserId} was not found" });

                var post = await postRepo.GetByIdAsync(postGuid);
                if (post == null)
                    return NotFound(new { errorMessage = $"Post with id {request.UserId} was not found" });

                var comment = mapper.Map<Comment>(request);
                await commentRepo.AddAsync(comment);
                var response = responseBuilder.BuildCommentResponse(comment);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // PUT api/<CommentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] CommentPutRequest request)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                var comment = await commentRepo.GetByIdAsync(guid);
                if (comment == null)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                comment.Update(request);
                await commentRepo.UpdateAsync(comment);
                var response = responseBuilder.BuildCommentResponse(comment);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {errorMessage = ex.Message});
            }
        }

        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                var comment = await commentRepo.GetByIdAsync(guid);
                if (comment == null)
                    return NotFound(new { errorMessage = $"Comment with id {id} was not found" });

                await commentRepo.DeleteAsync(comment);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }
    }
}
