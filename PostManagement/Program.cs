using Microsoft.EntityFrameworkCore;
using PostManagement.Models;
using PostManagement.Services;

namespace PostManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<PostManagementDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRazorPages();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddSignalR();
            builder.Services.AddControllers().AddJsonOptions(option =>
                option.JsonSerializerOptions.PropertyNamingPolicy = null
            );
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Ensure the database is created and migrate any pending migrations
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<PostManagementDbContext>();
                dbContext.Database.EnsureCreated(); // Ensures the database is created
                dbContext.Database.Migrate(); // Applies any pending migrations
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                // pattern: "{controller=Home}/{action=Index}/{id?}");
                pattern: "{controller=Posts}/{action=Index}/{id?}");

            app.MapHub<SignalRServer>("/signalRServer");

            app.MapRazorPages();

            app.Run();
        }
    }
}
