using Microsoft.EntityFrameworkCore;
using SuperBlog.Models.Entities;

namespace SuperBlog.Data.Repositories
{
    public class TagRepository : IRepository<Tag>
    {
        private readonly BlogContext _db;

        public TagRepository(BlogContext blogContext)
        {
            _db = blogContext;
        }

        public async Task AddAsync(Tag entity)
        {
            await _db.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tag entity)
        {
            _db.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Tag> GetAll()
        {
            return _db.Tags.AsQueryable();
        }

        public async Task<Tag?> GetByIdAsync(string id)
        {
            return await _db.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _db.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task UpdateAsync(Tag entity)
        {
            _db.Tags.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
