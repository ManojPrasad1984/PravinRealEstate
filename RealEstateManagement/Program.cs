using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using RealEstateManagement.Data;
using RealEstateManagement.Models;

namespace RealEstateManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure QuestPDF license (free community)
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);

            // Add MVC
            builder.Services.AddControllersWithViews();

            // Main application database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Secondary database (Estate)
            builder.Services.AddDbContext<EstateContext>(options =>
                options.UseSqlServer(
                    "server=(LocalDB)\\MSSQLLocalDB;database=EstateDBExam;trusted_connection=true;trust server certificate=true"));

            var app = builder.Build();

            // Configure middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}