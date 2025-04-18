using Microsoft.EntityFrameworkCore;
using MyWebAPI.Controllers;
using MyWebAPI.Models;

namespace MyWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSets (tables)
   


    }

}
