using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Language.Database;
using Language.Model;

namespace Language.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        public readonly MainDbContext _dbContext;

        public MainController(MainDbContext DbContext) 
        {
            _dbContext = DbContext;
        }

        [HttpGet("Test")]
        public async Task<List<User>> test()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();
            /*await _dbContext.Products.AddAsync(new Product() {Name = "Banana", Description = "Yellow banana", Price = 100 });
            await _dbContext.SaveChangesAsync();

            var products = await _dbContext.Products.ToListAsync();
            return products;*/
            return null!;
        }

        [HttpGet("Test1")]
        public async Task<string> test1()
        {
            return "Hello World";
        }

    }
}
