namespace SuperBlog.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetAll();
        public Task<T?> GetByIdAsync(string id);
        public Task<T?> GetByNameAsync(string name);
        public Task AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
    }
}
