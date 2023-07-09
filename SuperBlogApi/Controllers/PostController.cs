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
    public class PostController : ControllerBase
    {
        private readonly IRepository<Post> postRepo;
        private readonly IRepository<Tag> tagRepo;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly ResponseBuilder responseBuilder;

        public PostController(
            IRepository<Post> postRepo,
            IMapper mapper,
            UserManager<User> userManager,
            ResponseBuilder responseBuilder,
            IRepository<Tag> tagRepo)
        {
            this.postRepo = postRepo;
            this.mapper = mapper;
            this.userManager = userManager;
            this.responseBuilder = responseBuilder;
            this.tagRepo = tagRepo;
        }

        // GET: api/<PostController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var posts = await postRepo.GetAll().ToListAsync();
                var responses = new List<PostResponse>();
                foreach (var post in posts)
                {
                    var response = await responseBuilder.BuildPostResponse(post);
                    responses.Add(response);
                }
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var postId);
                if (!validGuid)
                    return StatusCode(404, new { errorMessage = $"Post with id {id} was not found" });

                var post = await postRepo.GetAll().FirstOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                    return StatusCode(404, new { errorMessage = $"Post with id {id} was not found" });

                var response = await responseBuilder.BuildPostResponse(post);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostPostRequest request)
        {
            try
            {
                var validUserGuid = Guid.TryParse(request.UserId, out var userGuid);
                if (!validUserGuid)
                    return StatusCode(404, new { errorMessage = $"User with id {request.UserId} was not found" });

                var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userGuid);
                if (user == null)
                    return StatusCode(404, new { errorMessage = $"User with id {request.UserId} was not found" });

                var post = mapper.Map<Post>(request);

                foreach (var tagId in request.Tags)
                {
                    var validGuid = Guid.TryParse(tagId, out var tagGuid);
                    if (!validGuid)
                        return StatusCode(404, new { errorMessage = $"Tag with id {tagId} was not found" });

                    var tag = await tagRepo.GetByIdAsync(tagGuid);
                    if (tag == null)
                        return StatusCode(404, new { errorMessage = $"Tag with id {tagId} was not found" });

                    post.Tags.Add(tag);
                }

                await postRepo.AddAsync(post);
                var response = await responseBuilder.BuildPostResponse(post);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] PostPutRequest request)
        {
            try
            {
                var post = await postRepo.GetByIdAsync(id);
                if (post == null)
                    return StatusCode(404, new { errorMessage = $"Post with id {id} was not found" });

                post.Tags.Clear();
                foreach (var tagId in request.Tags)
                {
                    var validGuid = Guid.TryParse(tagId, out var tagGuid);
                    if (!validGuid)
                        return StatusCode(404, new { errorMessage = $"Tag with id {tagId} was not found" });

                    var tag = await tagRepo.GetByIdAsync(tagGuid);
                    if (tag == null)
                        return StatusCode(404, new { errorMessage = $"Tag with id {tagId} was not found" });

                    post.Tags.Add(tag);
                }

                post.Update(request);
                await postRepo.UpdateAsync(post);
                var response = await responseBuilder.BuildPostResponse(post);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var post = await postRepo.GetByIdAsync(id);
                if (post == null)
                    return StatusCode(404, new { errorMessage = $"Post with id {id} was not found" });
                await postRepo.DeleteAsync(post);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
