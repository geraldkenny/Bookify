﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entity
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);
            //// Customize the ASP.NET Identity model and override the defaults if needed.
            //// For example, you can rename the ASP.NET Identity table names and more.
            //// Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper()});
            base.OnModelCreating(builder);
            // any guid
            //const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            //// any guid, but nothing is against to use the same one
            //const string ROLE_ID = ADMIN_ID;
            //builder.Entity<IdentityRole>().HasData(new IdentityRole
            //{
            //    Id = ROLE_ID,
            //    Name = "admin",
            //    NormalizedName = "admin"
            //});

            //var hasher = new PasswordHasher<IdentityUser>();
            //builder.Entity<IdentityUser>().HasData(new IdentityUser
            //{
            //    Id = ADMIN_ID,
            //    UserName = "admin",
            //    NormalizedUserName = "admin",
            //    Email = "admin@bookify.com",
            //    NormalizedEmail = "admin@bookify.com",
            //    EmailConfirmed = true,
            //    PasswordHash = hasher.HashPassword(null, "Admin1234$"),
            //    SecurityStamp = string.Empty
            //});

            //builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            //{
            //    RoleId = ROLE_ID,
            //    UserId = ADMIN_ID
            //});
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


    }

}
