
using Microsoft.EntityFrameworkCore;
using SuperBlogApi.Services;
using SuperBlogData;
using SuperBlogData.Models.Entities;
using SuperBlogData.Repositories;
using System.Reflection;

namespace SuperBlogApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionstring = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<BlogContext>(opt => opt.UseSqlServer(connectionstring));
            builder.Services.AddScoped<IRepository<Post>, PostRepository>();
            builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();
            builder.Services.AddScoped<IRepository<Tag>, TagRepository>();
            builder.Services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequiredLength = 5;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<BlogContext>();
            builder.Services.AddScoped<ResponseBuilder>();

            var mappingAssembly = Assembly.GetAssembly(typeof(MappingProfile));
            builder.Services.AddAutoMapper(mappingAssembly);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}