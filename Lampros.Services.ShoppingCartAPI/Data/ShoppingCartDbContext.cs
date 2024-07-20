


using Lampros.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.ShoppingCartAPI.Data
{
    public class ShoppingCartDbContext: DbContext
    {
        public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }

        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<CartHeader> CartHeader { get; set; }   
    }
}
