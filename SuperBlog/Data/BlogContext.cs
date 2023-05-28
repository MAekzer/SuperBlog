using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperBlog.Models.Entities;
using System.Security.Cryptography.Xml;

namespace SuperBlog.Data
{
    public class BlogContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.HasMany(e => e.Roles)
                .WithMany(e => e.Users);
            });
        }
    }
}
