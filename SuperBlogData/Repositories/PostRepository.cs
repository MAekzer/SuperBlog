using Microsoft.EntityFrameworkCore;
using SuperBlogData.Models.Entities;

namespace SuperBlogData.Repositories
{
    public class PostRepository : IRepository<Post>
    {
        private readonly BlogContext _db;

        public PostRepository(BlogContext blogContext)
        {
            _db = blogContext;
        }

        public async Task AddAsync(Post entity)
        {
            await _db.Posts.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post entity)
        {
            _db.Posts.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Post> GetAll()
        {
            var posts = _db.Posts.AsQueryable();
            return posts;
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _db.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Post?> GetByNameAsync(string name)
        {
            return await _db.Posts.FirstOrDefaultAsync(p => p.Title == name);
        }

        public async Task UpdateAsync(Post entity)
        {
            _db.Posts.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
