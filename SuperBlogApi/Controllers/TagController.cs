using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlogApi.Services;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.Responses;
using SuperBlogData.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperBlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IRepository<Tag> tagRepo;
        private readonly IMapper mapper;
        private readonly ResponseBuilder responseBuilder;

        public TagController(IRepository<Tag> tagRepo, IMapper mapper, ResponseBuilder responseBuilder)
        {
            this.tagRepo = tagRepo;
            this.mapper = mapper;
            this.responseBuilder = responseBuilder;
        }

        // GET: api/<TagController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tags = await tagRepo.GetAll().ToListAsync();
                var response = new List<TagResponse>();

                foreach (var tag in tags)
                {
                    var tagResponse = await responseBuilder.BuildTagResponse(tag);
                    response.Add(tagResponse);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // GET api/<TagController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                var tag = await tagRepo.GetByIdAsync(guid);
                if (tag == null)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                var response = await responseBuilder.BuildTagResponse(tag);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMesage = ex.Message });
            }
        }

        // POST api/<TagController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TagRequest request)
        {
            try
            {
                var existingTag = await tagRepo.GetByNameAsync(request.Name);
                if (existingTag != null)
                    return StatusCode(409, new { errorMessage = $"Tag with name {request.Name} already exists" });

                var tag = mapper.Map<Tag>(request);
                await tagRepo.AddAsync(tag);
                var response = await responseBuilder.BuildTagResponse(tag);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage =  ex.Message });
            }
        }

        // PUT api/<TagController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] TagRequest request)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                var tag = await tagRepo.GetByIdAsync(guid);
                if (tag == null)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                tag.Update(request);
                await tagRepo.UpdateAsync(tag);
                var response = await responseBuilder.BuildTagResponse(tag);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {errorMessage = ex.Message});
            }
        }

        // DELETE api/<TagController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var guid);
                if (!validGuid)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                var tag = await tagRepo.GetByIdAsync(guid);
                if (tag == null)
                    return NotFound(new { errorMessage = $"Tag with id {id} was not found" });

                await tagRepo.DeleteAsync(tag);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }
    }
}
