using EpinHell.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EpinHell.Data
{
    // This class represents the application's database context and integrates Identity for user authentication and authorization
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        // Constructor to configure the database context with specified options
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet for managing the 'Products' table
        public DbSet<Product> Products { get; set; }

        // DbSet for managing the 'Carts' table, which holds shopping cart information for users
        public DbSet<Cart> Carts { get; set; }

        // DbSet for managing the 'CartItems' table, which contains individual items in user carts
        public DbSet<CartItem> CartItems { get; set; }
    }
}
