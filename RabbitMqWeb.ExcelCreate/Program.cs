using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqWeb.ExcelCreate.Models;
using RabbitMqWeb.ExcelCreate.Services;

namespace RabbitMqWeb.ExcelCreate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
            builder.Services.AddSingleton<RabbitMqClientService>();
            builder.Services.AddSingleton<RabbitMQPublisher>();


            // Now we build the app
            var app = builder.Build();
            var userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            // Perform database migrations
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();

                if (!dbContext.Users.Any())
                {
                    userManager.CreateAsync(new IdentityUser
                    {
                        UserName = "deneme",
                        Email = "deneme@outlook.com"
                    }, "deneme12*").Wait();
                    userManager.CreateAsync(new IdentityUser
                    {
                        UserName = "deneme",
                        Email = "deneme@outlook.com"
                    }, "deneme12*").Wait();


                }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
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
}
