using Bookify.Repositories.Interfaces;
using Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositories
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
        public async Task CreateUser(IdentityUser identityUser)
        {
            
           // await _context.SaveChangesAsync();
        }
    }
}
