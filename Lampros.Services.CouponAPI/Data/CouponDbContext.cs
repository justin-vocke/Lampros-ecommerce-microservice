
using Lampros.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.CouponAPI.Data
{
    public class CouponDbContext: DbContext
    {
        public CouponDbContext(DbContextOptions<CouponDbContext> options) : base(options)
        {
            
        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
