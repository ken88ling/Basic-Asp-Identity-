using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityV2
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "ken@ken.com";
            var password = "password";

            //var userStore = new UserStore<IdentityUser>(); can change to
            var userStore = new CustomUserStore(new CustomUserDbContext());
            var userManager = new UserManager<CustomUser, int>(userStore);

            var createResult = userManager.Create(new CustomUser { UserName = username }, password);
            Console.WriteLine("created : {0}", createResult.Succeeded);

            var user = userManager.FindByName(username);
            
            //var claimResult = userManager.AddClaim(user.Id, new Claim("give_type", "give_value"));
            //Console.WriteLine("Claim:{0}", claimResult.Succeeded);
            //Console.ReadLine();

            var isMatch = userManager.CheckPassword(user, password);
            Console.WriteLine("Password Match: {0}", isMatch);
            Console.ReadLine();
        }
    }

    public class CustomUser : IUser<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

    }

    public class CustomUserDbContext : DbContext
    {
        public CustomUserDbContext() : base("DefaultConnection") { }

        public DbSet<CustomUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<CustomUser>();
            user.ToTable("Users");
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            user.Property(x => x.UserName).IsRequired().HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            base.OnModelCreating(modelBuilder);
        }
    }

    public class CustomUserStore : IUserPasswordStore<CustomUser, int>
    {
        private readonly CustomUserDbContext _context;

        public CustomUserStore(CustomUserDbContext context)
        {
            this._context = context;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(CustomUser user)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(CustomUser user)
        {
            _context.Users.Attach(user);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(CustomUser user)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync();
        }

        public Task<CustomUser> FindByIdAsync(int userId)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<CustomUser> FindByNameAsync(string userName)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(CustomUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
