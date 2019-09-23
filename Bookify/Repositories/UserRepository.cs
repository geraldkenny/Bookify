using Bookify.Repositories.Interfaces;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Repositories
{
    public class UserRepository : IUserRepository
    {
        // private field for the data context
        private readonly UserDbContext _context;


        // Constructor that accept data context
        public UserRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task CreateUserAsync(User user)
        {
            _context.BookifyUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var (isUserFound, user) = await GetUserByIdAsync(userId);
            if (isUserFound)
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;
                _context.Entry(user).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExistsAsync(userId))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<(bool, User)> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.BookifyUsers.FindAsync(userId);
                if (user == null)
                {
                    return (false, null);
                }
                return (true, user);
            }
            catch (Exception)
            {

                return (false, null);
            }
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.BookifyUsers.AnyAsync(e => e.UserId == userId);
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.BookifyUsers.ToListAsync();
        }
        public async Task<(bool, List<User>)> SearchUserAsync(Expression<Func<User, bool>> expression)
        {
            try
            {
                var users = await _context.BookifyUsers.Where(expression).ToListAsync();
                if (users == null || users.Count < 0)
                {
                    return (false, null);
                }
                return (true, users);
            }
            catch (Exception)
            {

                return (false, null);
            }
        }
    }
}
