using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataBookApi.Authentification;
using DataBookApi.Models;

namespace DataBookApi.DataContext
{
    public class DataBookContext : IdentityDbContext<User>
    {
        public DbSet<DataBook> DataBook { get; set; }
        public DataBookContext(DbContextOptions options) : base(options) { }
    }
}
