using SuperBlog.Models.Entities;

namespace SuperBlog.Data.Repositories
{
    public class CommentRepository : IRepository<Comment>
    {
        private readonly BlogContext _db;

        public CommentRepository(BlogContext blogContext)
        {
            _db = blogContext;
        }

        public async Task AddAsync(Comment entity)
        {
            await _db.Comments.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment entity)
        {
            _db.Comments.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Comment> GetAll()
        {
            return _db.Comments.AsQueryable();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _db.Comments.FindAsync(id);
        }

        public Task<Comment?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Comment entity)
        {
            _db.Comments.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
