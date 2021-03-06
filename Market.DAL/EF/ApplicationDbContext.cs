﻿using Market.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartLine> CartLines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Characteristic>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Product>().Property(p => p.Character).HasColumnType("xml");

            builder.Entity<Brand>(brand =>
            {
                brand.HasMany(b => b.Products)
                    .WithOne(p => p.Brand).OnDelete(DeleteBehavior.SetNull);
                brand.HasIndex(c => c.Name).IsUnique();
            });

            builder.Entity<Category>(category =>
            {
                category.HasMany(c => c.Products)
                    .WithOne(p => p.Category).OnDelete(DeleteBehavior.SetNull);
                category.HasIndex(c => c.Name).IsUnique();
            });

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<CartLine>().HasKey(cl => new { cl.UserId, cl.ProductId });

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}