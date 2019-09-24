using System;
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
            //// Customize the ASP.NET Identity model and override the defaults if needed.
            //// For example, you can rename the ASP.NET Identity table names and more.
            //// Add your customizations after calling base.OnModelCreating(builder);

            base.OnModelCreating(builder);

            //builder.Entity<User>().HasData(
            //    new Book { Name = "Art of not giving a fuck", CreatedAt = DateTime.Now, BookId = 1, Description = "Name is self explanory", BookCount = 5  },
            //    new Book { Name = "48 laws of power", CreatedAt = DateTime.Now, BookId = 2, Description = "Name is self explanory", BookCount = 5  },
            //    new Book { Name = "Think and grow rich", CreatedAt = DateTime.Now, BookId = 3, Description = "Name is self explanory", BookCount = 5  }
            //);

            //builder.Entity<Transaction>().HasData(
            //    new Transaction { UserId = 1, BookId = 1, TransactionId = 1, Status = BookStatus.Borrowed, BorrowedAt = DateTime.Now.AddDays(-3) },
            //    new Transaction { UserId = 1, BookId = 1, TransactionId = 2, Status = BookStatus.Borrowed, BorrowedAt = DateTime.Now.AddDays(-1) },
            //    new Transaction { UserId = 1, BookId = 1, TransactionId = 3, Status = BookStatus.Returned, BorrowedAt = DateTime.Now.AddDays(-2) }
            //);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


    }

}
