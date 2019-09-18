using Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync();
        Task<bool> AddBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
    }
}
