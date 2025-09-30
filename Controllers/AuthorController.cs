using LibraryManagement.DTOs.Author;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Repositories;
using LibraryMangment.Model;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorRepository authorRepository, ILogger<AuthorsController> logger)
        {
            _authorRepository = authorRepository;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<ActionResult<PagedResult<AuthorDto>>> GetAllAuthors(int pageNumber = 1,int pageSize = 10,
            string sortBy = "Name",
            bool ascending = true)
        {
            try
            {
                var result = await _authorRepository.GetAllAsync(pageNumber, pageSize, sortBy, ascending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching authors");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.GetByIdAsync(id);

                if (author == null)
                {
                    return NotFound();
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorCreateDto authorCreateDto)
        {
            try
            {
                var result = await _authorRepository.AddAsync(authorCreateDto);
                return CreatedAtAction(nameof(GetAuthor), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating author");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, AuthorCreateDto authorUpdateDto)
        {
            try
            {
                var result = await _authorRepository.UpdateAsync(id, authorUpdateDto);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var authorDto = await _authorRepository.GetByIdAsync(id);

                if (authorDto == null)
                {
                    return NotFound();
                }
                var result = await _authorRepository.DeleteAsync(id);

                if (!result)
                {
                    return StatusCode(500, "Error deleting author");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
