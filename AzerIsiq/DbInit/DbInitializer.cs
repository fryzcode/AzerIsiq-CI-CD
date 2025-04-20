using AzerIsiq.Data;
using AzerIsiq.Models;
using Microsoft.EntityFrameworkCore;

namespace AzerIsiq.DbInit
{
    public class DbInitializer : IDbInitializer
    {

        private readonly AppDbContext _context;

        public DbInitializer(AppDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            if (!_context.Roles.Any())
            {
                var roles = new List<Role>
        {
            new Role { RoleName = "Admin" },
            new Role { RoleName = "User" },
            new Role { RoleName = "Manager" },
            new Role { RoleName = "Operator" }
        };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }

            if (!_context.Users.Any(u => u.Email == "feryazhajimuradov18@gmail.com"))
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "feryazhajimuradov18@gmail.com",
                    PhoneNumber = "994559159999",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345Test@@@@"),
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = "127.0.0.1"
                };
                _context.Users.Add(admin);
                _context.SaveChanges();

                var adminRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                if (adminRole != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
                    _context.SaveChanges();
                }
            }

            // Region və District əlavə et
            if (!_context.Regions.Any())
            {
                var regions = new List<Region>
        {
            new Region { Name = "Bakı" },
            new Region { Name = "Gəncə" },
            new Region { Name = "Sumqayıt" },
            new Region { Name = "Şəki" },
            new Region { Name = "Naxçıvan" }
        };

                _context.Regions.AddRange(regions);
                _context.SaveChanges();
            }

            if (!_context.Districts.Any())
            {
                var regionDict = _context.Regions.ToDictionary(r => r.Name, r => r.Id);

                var districts = new List<District>
        {
            new District { Name = "Binəqədi", RegionId = regionDict["Bakı"] },
            new District { Name = "Nizami", RegionId = regionDict["Bakı"] },
            new District { Name = "Yasamal", RegionId = regionDict["Bakı"] },

            new District { Name = "Kəpəz", RegionId = regionDict["Gəncə"] },
            new District { Name = "Nizami (Gəncə)", RegionId = regionDict["Gəncə"] },

            new District { Name = "Sumqayıt şəhəri", RegionId = regionDict["Sumqayıt"] },

            new District { Name = "Şəki şəhəri", RegionId = regionDict["Şəki"] },

            new District { Name = "Naxçıvan şəhəri", RegionId = regionDict["Naxçıvan"] }
        };

                _context.Districts.AddRange(districts);
                _context.SaveChanges();
            }
        }


    }
}
