using Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);
        Task<(Book, string)> BorrowBookAsync(int bookId, User user);
        Task<bool> AddBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> UserBorrowedBookAsync(int bookId, int userId);
        Task<(bool, List<Book>)> SearchUserAsync(Expression<Func<Book, bool>> expression);

    }
}
