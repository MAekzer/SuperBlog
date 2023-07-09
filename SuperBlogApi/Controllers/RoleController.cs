using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlogData.Models.Entities;
using SuperBlogData.Models.Requests;
using SuperBlogData.Models.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperBlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public RoleController(RoleManager<Role> roleManager, IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        // GET: api/<RoleController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var roles = await roleManager.Roles.ToListAsync();
                var response = roles.Select(mapper.Map<RoleResponse>);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound(new { errorMessage = $"Role with id {id} was not found" });
                var response = mapper.Map<RoleResponse>(role);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // POST api/<RoleController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolePostRequest request)
        {
            try
            {
                var role = mapper.Map<Role>(request);
                var existingRole = await roleManager.FindByNameAsync(request.Name);
                if (existingRole != null)
                    return StatusCode(409, new { errorMessage = $"Role with name {request.Name} already exists" });

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    var response = mapper.Map<RoleResponse>(role);
                    return Ok(response);
                }
                return StatusCode(500, result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] RolePutRequest request)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out var roleGuid);
                if (!validGuid) return StatusCode(404, new { errorMessage = $"Role with id {id} was not found" });

                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                    return StatusCode(404, new { errorMessage = $"Role with id {id} was not found" });

                var existingRole = await roleManager.FindByNameAsync(request.Name);
                if (existingRole != null && existingRole.Id != roleGuid)
                    return StatusCode(409, new { errorMessage = $"Role with name {request.Name} already exists" });

                role.Update(request);
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    var response = mapper.Map<RoleResponse>(role);
                    return Ok(response);
                }
                return StatusCode(500, result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var validGuid = Guid.TryParse(id, out _);
                if (!validGuid) return NotFound(new { errorMessage = $"Role with id {id} was not found" });

                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                    return NotFound(new { errorMessage = $"Role with id {id} was not found" });

                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return StatusCode(204);
                return StatusCode(500, result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }
    }
}
