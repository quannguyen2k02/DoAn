using API.Models;
using Api1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api1.Data
{
    public class BanHangContext:IdentityDbContext<ApplicationUser>
    {
        public BanHangContext(DbContextOptions<BanHangContext> opt):base(opt) {
            
        }
        public DbSet<Adv> Advs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Subcribe> Subcribes  { get; set; }
        public DbSet<SaleInfor> SaleInfors { get; set; }
        public DbSet<Response> Responses { get; set; }
    }
}


