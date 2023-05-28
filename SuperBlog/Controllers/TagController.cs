using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Data.Repositories;
using SuperBlog.Models.Entities;
using SuperBlog.Models.ViewModels;

namespace SuperBlog.Controllers
{
    public class TagController : Controller
    {
        public IRepository<Tag> tagRepo { get; set; }

        public TagController(IRepository<Tag> tagRepository)
        {
            tagRepo = tagRepository;
        }

        [HttpGet]
        [Route("AllTags")]
        public async Task<IActionResult> AllTags()
        {
            var tags = await tagRepo.GetAll().ToListAsync();
            return View("/Views/Tags/AllTags.cshtml", tags);
        }

        [HttpGet]
        [Route("Tag")]
        public async Task<IActionResult> Tag(string id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            return View("/Views/Tags/Tag.cshtml", tag);
        }

        [HttpGet]
        [Route("CreateTag")]
        public async Task<IActionResult> Create()
        {
            return View("/Views/Tags/CreateTag.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(WriteTagViewModel model)
        {
            var tag = new Tag();
            tag.Name = model.Name;
            await tagRepo.AddAsync(tag);
            return View("/Views/Tags/Tag.cshtml", tag);
        }

        [HttpGet]
        [Route("UpdateTag")]
        public async Task<IActionResult> Update(string id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            return View("/Views/Tags/UpdateTag.cshtml", tag);
        }

        [HttpPost]
        [Route("UpdateTag")]
        public async Task<IActionResult> Update(WriteTagViewModel model, string id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            if (tag == null) return View("/Views/Tags/TagNotFound.cshtml");

            tag.Name = model.Name;
            await tagRepo.UpdateAsync(tag);

            return View("/Views/Tags/Tag.cshtml", tag);
        }

        [HttpPost]
        [Route("DeleteTag")]
        public async Task<IActionResult> Delete(string id)
        {
            var tag = await tagRepo.GetByIdAsync(id);
            if (tag != null) await tagRepo.DeleteAsync(tag);

            var tags = await tagRepo.GetAll().ToListAsync();
            return View("/Views/Tags/AllTags.cshtml", tags);
        }
    }
}
