using Bookify.Repositories.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bookify.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IBookRepository Book { get; private set; }
        public IUserRepository User { get; private set; }


        public UnitOfWork(UserDbContext userDbContext, ApplicationDbContext applicationDbContext)
        {
            Book = new BookRepository(applicationDbContext);
            User = new UserRepository(userDbContext);

            
        }
      
    }
}
