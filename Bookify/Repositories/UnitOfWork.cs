using BLL.Repositories;
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
        private readonly DbContext _context;
        public IBookRepository Book { get; private set; }
        public IUserRepository User { get; private set; }


        public UnitOfWork(DbContext context)
        {
            _context = context;
            Book = new BookRepository(context as ApplicationDbContext);
            User = new UserRepository(context as UserDbContext);

            
        }
        //private string _lastname;
        //public int? LastName {
        //    get
        //    {
        //        return Convert.ToInt32( _lastname);
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _lastname = Convert.ToString(value);
        //        }

        //    }
        //}
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
