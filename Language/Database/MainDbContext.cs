using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Language.Model;

namespace Language.Database
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        //    Database.EnsureDeleted();
        //    Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BaseWord> BaseWords { get; set; }
        public DbSet<ExtentedWord> ExtentedWords { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<WordProperties>  WordProperties { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Assembly assemblyWithConfigurations = GetType().Assembly; //get whatever assembly you want
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);


            Users.AddAsync(new User() { Id = new Guid(), Name = "Admin", Email = "test@mail.ru", Password = "1234" });
        }

        public void ClearAll()
        {
            Users.RemoveRange(this.Users);
            BaseWords.RemoveRange(this.BaseWords);
            Texts.RemoveRange(this.Texts);
            SaveChanges();
        }
    }
}
