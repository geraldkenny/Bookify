using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entity
{
    public class UserDbContext : IdentityDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);
            //// Customize the ASP.NET Identity model and override the defaults if needed.
            //// For example, you can rename the ASP.NET Identity table names and more.
            //// Add your customizations after calling base.OnModelCreating(builder);


            //builder.Entity<User>().HasData(
            //    new User { Username = "Gerald", CreatedAt = DateTime.Now, FirstName = "Gerald", LastName = "Kenny", UserId = 1 },
            //    new User { Username = "Elon", CreatedAt = DateTime.Now, FirstName = "Elon", LastName = "Musk", UserId = 2 },
            //    new User { Username = "Jeff", CreatedAt = DateTime.Now, FirstName = "Jeff", LastName = "Bezos", UserId = 3 }
            //);
            base.OnModelCreating(builder);
           
        }

         public DbSet<User> BookifyUsers { get; set; }


    }

}
