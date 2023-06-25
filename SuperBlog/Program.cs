using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using SuperBlog.Data;
using SuperBlog.Data.Repositories;
using SuperBlog.Exceptions;
using SuperBlog.Models.Entities;
using SuperBlog.Services;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SuperBlog
{
    public class Program
    { 
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddNLog();

            string connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<BlogContext>(opt => opt.UseSqlite(connectionString));

            var mappingAssembly = Assembly.GetAssembly(typeof(MappingProfile));
            builder.Services.AddAutoMapper(mappingAssembly);

            builder.Services.AddScoped<IRepository<Post>, PostRepository>();
            builder.Services.AddScoped<IRepository<Comment>, CommentRepository>();
            builder.Services.AddScoped<IRepository<Tag>, TagRepository>();
            builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
            builder.Services.AddScoped<UserHandler>();
            builder.Services.AddScoped<TagHandler>();
            builder.Services.AddScoped<RoleHandler>();
            builder.Services.AddScoped<PostHandler>();
            builder.Services.AddScoped<CommentHandler>();
            builder.Services.AddScoped<ErrorHandler>();

            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequiredLength = 5;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<BlogContext>();

            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.AccessDeniedPath = "/AccessDenied";
                opt.LoginPath = "/";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            { 
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}