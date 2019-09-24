using Bookify.Repositories.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                book.CreatedAt = DateTime.Now;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
 
        }

        public async Task<(Book, string)> BorrowBookAsync(int bookId, User user)
        {
            var book = await GetBookByIdAsync(bookId);
            var taskBook = UserBorrowedBookAsync(bookId, user.UserId);
            if (book?.BookCount > 0)
            {
                if (!await taskBook)
                {
                    var borrowBook = new Transaction
                    {
                        BookId = book.BookId,
                        BorrowedAt = DateTime.Now,
                        Status = BookStatus.Borrowed,
                        UserId = user.UserId
                    };
                    _context.Entry(book).State = EntityState.Modified;
                    _context.Transactions.Add(borrowBook);
                    await _context.SaveChangesAsync();
                    return (book, string.Empty);

                }
                return (null, "User already borrowed book");

            }
            return (null, "Book not avaible");
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    book.IsDeleted = true;
                    _context.Entry(book).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }
        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            return await _context.Books.FindAsync(bookId);
        }

        public async Task<List<Book>> GetBooksAsync()
        {
           return await _context.Books.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<(bool, List<Book>)> SearchUserAsync(Expression<Func<Book, bool>> expression)
        {
            return (true, await _context.Books.Where(expression).ToListAsync());
        }

        public async Task<bool> UserBorrowedBookAsync(int bookId, int userId)
        {
            return await _context.Transactions.AnyAsync(x => x.BookId == bookId && x.UserId == userId && x.Status == BookStatus.Borrowed);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
