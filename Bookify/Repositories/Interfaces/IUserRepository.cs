using Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Repositories.Interfaces
{
    public interface IUserRepository
    {
         Task CreateUserAsync(User user);

         Task<IEnumerable<User>> GetUsersAsync();
         Task<bool> DeleteUserAsync(int userId);
         Task<bool> UserExistsAsync(int userId);
         Task<(bool, User)> GetUserByIdAsync(int userId);
         Task<(bool, User)> GetUserByUsernameAsync(string username);
         Task<(bool, List<User>)> SearchUserAsync(Expression<Func<User, bool>> expression);
    }
}
