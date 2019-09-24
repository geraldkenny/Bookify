using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;
using Bookify.Repositories.Interfaces;
using AutoMapper;
using Bookify.DTO;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(UserDbContext context, IUnitOfWork unitOfWork,
                                IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves available users
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Users retrieved</response>
        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetBookifyUsers()
        {
            var users = await _unitOfWork.User.GetUsersAsync();
            return _mapper.Map<List<UserDTO>>(users); 
        }
        /// <summary>
        /// Retrieves user with id
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">User retrieved</response>
        /// <response code="404">User Not found with a StatusCode and ResponseMessage</response>
        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var(isUserFound, user) = await _unitOfWork.User.GetUserByIdAsync(id);
            if (isUserFound)
            {
                return _mapper.Map<UserDTO>(user);
            }
            return BadRequest(new HttpResponseDTO { StatusCode = "03", ResponseMessage = "Unable to delete user" });

        }
        /// <summary>
        /// Retrieves users based on the search params
        /// Matching either first, last name or email
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Users retrieved</response>
        /// <response code="404">Users Not found with a StatusCode and ResponseMessage</response>
        // GET: api/Users/searchParam?="Gerald"
        [HttpGet("{searchParam}")]
        public async Task<ActionResult<List<UserDTO>>> SearchUsers(string searchParam)
        {
            Expression<Func<User, bool>> expression;
            expression = d => d.EmailAddress == searchParam || d.EmailAddress.Contains(searchParam) || d.FirstName == searchParam 
            || d.FirstName.Contains(searchParam) || d.LastName == searchParam || d.LastName.Contains(searchParam);

            var (foundUsers, users) = await _unitOfWork.User.SearchUserAsync(expression);
            if (foundUsers)
            {
                return _mapper.Map<List<UserDTO>>(users);
            }
            return BadRequest(new HttpResponseDTO { StatusCode = "03", ResponseMessage = "Unable to delete user" });

        }
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.BookifyUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        /// <summary>
        /// Deletes user
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">User retrieved</response>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var isUserDeleted = await _unitOfWork.User.DeleteUserAsync(id);

            if (isUserDeleted)
            {
                return Ok(new HttpResponseDTO { StatusCode = "00", ResponseMessage = "User deleted" });
            }

            return BadRequest(new HttpResponseDTO { StatusCode = "03", ResponseMessage = "Unable to delete user" });
        }

        private bool UserExists(int id)
        {
            return _context.BookifyUsers.Any(e => e.UserId == id);
        }
    }
}
