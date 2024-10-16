using Microsoft.EntityFrameworkCore;
using StudentMigrations.CRUD.Data;

namespace StudentMigrations.CRUD
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
			
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			app.UseStaticFiles();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Students}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
