using LoginApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data
{
    public class DataContext: DbContext
    {

        public DataContext( DbContextOptions<DataContext> options) : base(options)  
        {

        }

        public DbSet<User> Users { get; set; }  
    }
}
