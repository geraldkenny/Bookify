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
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public BooksController(ApplicationDbContext context, IUnitOfWork unitOfWork,
                                IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves available books
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Books retrieved</response>
        // GET: api/Books
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _unitOfWork.Book.GetBooksAsync();
            return _mapper.Map<List<BookDTO>>(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Produces("application/json")]

        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        /// <summary>
        /// Allows a user to borrow a book
        /// </summary>
        /// <remarks>Authorized for admin and normal users!</remarks>
        /// <response code="200">Book borrowed</response>
        /// <response code="400">already borrowed book</response>
        // GET: api/Books/Borrow/5
        [HttpGet]
        [Route("Borrow")]
        [Authorize]

        public async Task<ActionResult<BookDTO>> BorrowBook(int bookId)
        {
            var user = await _unitOfWork.User.GetUserByUsernameAsync(User.Identity.Name);
            var (book, respMessage) = await _unitOfWork.Book.BorrowBookAsync(bookId, user.Item2);
            if (book != null)
            {
                return _mapper.Map<BookDTO>(book);
            }
            return BadRequest(new { status_code = 03, response_message = respMessage });
        }


        /// <summary>
        /// Update existing book
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Book updated</response>
        /// <response code="400">Book has missing/invalid values</response>
        /// <response code="500">Oops! Can't update your book right now</response>
        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Book created</response>
        /// <response code="400">Book has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] AddBookDTO book)
        {
            var model = _mapper.Map<Book>(book);
            //model.CreatedBy = User.Identity.Name;
            if (await _unitOfWork.Book.AddBookAsync(model))
            {

                return CreatedAtAction("GetBook", new { id = model.BookId }, model);
            }
            return BadRequest();

        }

        /// <summary>
        /// Deletes a new book
        /// </summary>
        /// <remarks>Only authorized for admin users!</remarks>
        /// <response code="200">Book deleted</response>
        /// <response code="400">Book has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your delete right now</response>
        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var status = await _unitOfWork.Book.DeleteBookAsync(id);
            if (status)
            {
                return Ok(new { status_code = 00, response_message = "Book deleted!" });

            }
            return BadRequest();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
