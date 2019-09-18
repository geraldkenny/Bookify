using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Repositories.Interfaces
{
    public interface IUserRepository
    {
         Task CreateUser(IdentityUser identityUser);
    }
}
