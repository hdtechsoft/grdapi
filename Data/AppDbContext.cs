
using grdApi.Models;
using Microsoft.EntityFrameworkCore;


namespace grdApi.Data
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}