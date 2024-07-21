
using Lampros.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.OrderAPI.Data
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            
        }
        

        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }   
    }
}
