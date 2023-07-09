using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlogApi.Services;
using SuperBlogData.Extentions;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperBlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly ResponseBuilder responseBuilder;

        public UserController(UserManager<User> userManager, IMapper mapper, ResponseBuilder responseBuilder)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.responseBuilder = responseBuilder;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await userManager.Users.ToListAsync();
                var responses = new List<UserResponse>();
                foreach (var user in users)
                {
                    var response = await responseBuilder.BuildUserResponse(user);
                    responses.Add(response);
                }
                
                return StatusCode(200, responses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null) return StatusCode(404, new { errorMessage = $"User with id {id} was not found." });
                var response = await responseBuilder.BuildUserResponse(user);
                return StatusCode(200, response);
            }
            catch (FormatException)
            {
                return StatusCode(404, new { errorMessage = $"User with id {id} was not found." });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserPostRequest request)
        {
            try
            {
                var user = mapper.Map<User>(request);

                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null) 
                    return StatusCode(409, new { errorMessage = $"User with email {request.Email} already exists" });

                existingUser = await userManager.FindByNameAsync(request.UserName);
                if (existingUser != null) 
                    return StatusCode(409, new { errorMessage = $"User with login {request.UserName} already exists" });

                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    var response = await responseBuilder.BuildUserResponse(user);
                    return StatusCode(200, response);
                }
                return StatusCode(500, result.Errors);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UserPutRequest request)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null) return StatusCode(404, $"User with id {id} was not found");

                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && user.Id != existingUser.Id)
                    return StatusCode(409, new { errorMessage = $"User with email {request.Email} already exists" });

                existingUser = await userManager.FindByNameAsync(request.UserName);
                if (existingUser != null && user.Id != existingUser.Id)
                    return StatusCode(409, new { errorMessage = $"User with login {request.UserName} already exists" });

                user.Update(request);
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var response = await responseBuilder.BuildUserResponse(user);
                    return StatusCode(200, response);
                }
                return StatusCode(500, result.Errors);
            }
            catch (FormatException)
            {
                return StatusCode(404, new { errorMessage = $"User with id {id} was not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null) return StatusCode(404, $"User with id {id} was not found");
                await userManager.DeleteAsync(user);
                return StatusCode(204);
            }
            catch (FormatException)
            {
                return StatusCode(404, new { errorMessage = $"User with id {id} was not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
