using LibraryManagement.DTOs.Borrow;
using LibraryManagement.Repositories;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowsHistoryController : ControllerBase
    {
        private readonly IBorrowsHistoryRepository _borrowsHistoryRepository;
        private readonly ILogger<BorrowsHistoryController> _logger;
        public BorrowsHistoryController(IBorrowsHistoryRepository borrowsHistoryRepository, ILogger<BorrowsHistoryController> logger)
        {
            _borrowsHistoryRepository = borrowsHistoryRepository;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<PagedResult<BorrowsHistoryDto>>> GetBorrowsHistory(
             int pageNumber = 1, int pageSize = 10,
             string sortBy = "BorrowDate",bool ascending = false)
        {
            try
            {
                var result = await _borrowsHistoryRepository.GetAllAsync(pageNumber, pageSize, sortBy, ascending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching borrows history");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowsHistoryDto>> GetBorrowHistory(int id)
        {
            try
            {
                var borrowHistory = await _borrowsHistoryRepository.GetByIdAsync(id);

                if (borrowHistory == null)
                {
                    return NotFound();
                }

                return Ok(borrowHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching borrow history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BorrowsHistoryDto>> CreateBorrowHistory(BorrowsHistoryCreateDto borrowsHistoryCreateDto)
        {
            try
            {
                var result = await _borrowsHistoryRepository.AddAsync(borrowsHistoryCreateDto);

                return CreatedAtAction(nameof(GetBorrowHistory), new { id = result.Id }, result);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating borrow history");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBorrowHistory(int id, BorrowsHistoryCreateDto borrowsHistoryCreateDto)
        {
            try
            {
                var result = await _borrowsHistoryRepository.UpdateAsync(id, borrowsHistoryCreateDto);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating borrow history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowHistory(int id)
        {
            try
            {
                var result = await _borrowsHistoryRepository.DeleteAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting borrow history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("byMember/{memberId}")]
        public async Task<ActionResult<PagedResult<BorrowsHistoryDto>>> GetBorrowsByMember(int memberId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var result = await _borrowsHistoryRepository.GetBorrowsByMemberAsync(memberId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error fetching borrows for member with ID {memberId}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("byBook/{bookId}")]
        public async Task<ActionResult<PagedResult<BorrowsHistoryDto>>> GetBorrowsByBook(int bookId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var result = await _borrowsHistoryRepository.GetBorrowsByBookAsync(bookId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching borrows for book with ID {bookId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("ifActive")]
        public async Task<ActionResult<PagedResult<BorrowsHistoryDto>>> GetActiveBorrows(
             int pageNumber = 1,
             int pageSize = 10)
        {
            try
            {
                var result = await _borrowsHistoryRepository.GetActiveBorrowsAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active borrows");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{bookId}/return")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            try
            {
                var result = await _borrowsHistoryRepository.ReturnBookAsync(bookId);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error returning book for borrow with ID {bookId}" );
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
