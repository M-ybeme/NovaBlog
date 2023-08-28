using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaBlog.Models;
using Npgsql;

namespace NovaBlog.Data
{
    public class DataUtility
    {
        private const string _adminEmail = "marlomayberry.90@gmail.com";
        private const string _modEmail = "moderatorNova@mailinator.com";
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";

        public static DateTime GetPostgresDate (DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return String.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(":");
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task SeedDataAsync(IServiceProvider svcProvider)
        {
            //Service: An Instance of rolemanager
            var dbContextScv = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Migration: This is the programatic equivelent to Update=Database
            await dbContextScv.Database.MigrateAsync();
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
            //Service: An instance of roleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service: An inastance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            //Seed Roles
            await SeedRolesAsync(roleManagerSvc);
            //Seed Users
            await SeedUsersAsync(dbContextScv, configurationSvc ,userManagerSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            //Add Admin
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Marlo",
                    LastName = "Mayberry",
                    PhoneNumber = "850-356-5150",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, configuration["AdminPwd"] ?? Environment.GetEnvironmentVariable("AdminPwd"));
                await userManager.AddToRoleAsync(adminUser, _adminRole);

            }

            //Add Mod
            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "CF",
                    LastName = "Mod",
                    PhoneNumber = "850-356-5151",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(modUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd"));
                await userManager.AddToRoleAsync(modUser, _modRole);
            }

        }
    }
}
