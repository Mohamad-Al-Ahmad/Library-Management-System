using LibraryManagement.DTOs.Book;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<BookDto>>> GetBooks(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "Title",
            bool ascending = true)
        {
            try
            {
                var result = await _bookRepository.GetAllAsync(pageNumber, pageSize, sortBy, ascending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching books");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching book with ID {id}" );
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook( CreateBookDto createBookDto)
        {
            try
            {
                var result = await _bookRepository.AddAsync(createBookDto);

                return CreatedAtAction(nameof(GetBook), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                var result = await _bookRepository.UpdateAsync(id, updateBookDto);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookRepository.DeleteAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error deleting book with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
