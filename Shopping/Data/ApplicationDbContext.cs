using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.Customer;
using System;

namespace Shopping.Data

{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        #region Admin
        public virtual DbSet<Brade> Brades { get; set; }
        public virtual DbSet<Category> Categorys { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Item> Items { get; set; }

        //customer
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        #endregion Admin

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(
                b => {
                    b.Property(p => p.LastName).IsRequired().HasMaxLength(ValidConst.MaxLenghtNames);
                });
            builder.Entity<Brade>(
                b => {
                    b.HasKey(x => x.id);
                    b.Property(x => x.Name).HasMaxLength(ValidConst.MaxLenghtNames);
                    b.HasMany(x => x.Categorys).WithOne(x=>x.Brade).HasForeignKey(x=>x.BradeId).OnDelete(DeleteBehavior.Cascade);
                });
            builder.Entity<Category>(
                b => {
                    b.HasKey(x => x.id);
                    b.Property(x => x. Name).HasMaxLength(ValidConst.MaxLenghtNames);
                    b.HasMany(x => x.Products).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
                });
           builder.Entity<Product>(
                b => {
                    b.HasKey(x => x.id);
                    
                    b.Property(x => x. Name).HasMaxLength(ValidConst.MaxLenghtNames);
                    b.Property(x => x.TagName).HasMaxLength(ValidConst.MaxLenghtNames);
                    
                    b.HasMany(x => x.Items).WithOne(x => x.product).HasForeignKey(x => x.productId).OnDelete(DeleteBehavior.Cascade);
                });
            builder.Entity<Item>(
                 b => {
                     b.HasKey(x => x.id);
                     b.Property(x => x.Name).HasMaxLength(ValidConst.MaxLenghtNames);
                     b.Property(x => x.color).HasMaxLength(ValidConst.MaxLenghtNames);
                     b.HasMany<OrderItem>(x => x.OrderItem).WithOne(x => x.Item).HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);
                 });

            builder.Entity<Order>(
                 b => {
                     b.HasKey(x => x.id);
                     b.Property(x => x.Note).HasMaxLength(ValidConst.MaxLenghtNames);
                     b.Property(x => x.numbers).HasMaxLength(ValidConst.MaxLenghtNames);
                     b.HasMany<OrderItem>(x => x.OrderItem).WithOne(x => x.order).HasForeignKey(x => x.orderId).OnDelete(DeleteBehavior.Cascade);
                 });

            base.OnModelCreating(builder);

            var UserId = Guid.NewGuid().ToString();
            this.SeedUsers(builder, UserId);

            var RoleId = Guid.NewGuid().ToString();
            this.SeedRoles(builder, RoleId);

            this.SeedUserRoles(builder, RoleId, UserId);

        }

        private void SeedUsers(ModelBuilder builder, string UserId)
        {
            var user = new ApplicationUser()
            {
                Id = UserId,
                UserName = "Admin",
                LastName = "Admin",
                Email = "admin@gmail.com",
                LockoutEnabled = false,
                Address = "",
                PhoneNumber = ""
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            passwordHasher.HashPassword(user, "Admin*123");

            builder.Entity<ApplicationUser>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder, string RoleId)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = RoleId, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "user", ConcurrencyStamp = "2", NormalizedName = "Human Resource" }
                );
        }

        private void SeedUserRoles(ModelBuilder builder, String roleId, string userId)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = roleId, UserId = userId }
                );
        }

    }
}
