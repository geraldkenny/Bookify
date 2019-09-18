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
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;


        // Constructor that accept data context
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
 
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    book.IsDeleted = true;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
           return await _context.Books.ToListAsync();
        }



        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
