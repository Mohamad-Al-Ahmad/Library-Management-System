using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.Member;
using LibraryManagement.Repositories.Interfaces;
using LibraryMangment.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Repositories
{
    public class BorrowsHistoryRepository : IBorrowsHistoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BorrowsHistoryRepository> _logger;
        public BorrowsHistoryRepository(AppDbContext context, IMapper mapper, ILogger<BorrowsHistoryRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<BorrowsHistoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "BorrowDate", bool ascending = false)
        {
            try
            {
                pageNumber = pageNumber < 1 ? 1 : pageNumber;
                pageSize = pageSize < 1 ? 10 : pageSize;
                int RecordToPass = pageSize * pageNumber - pageSize;

                var query = _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .AsQueryable();

                query = sortBy.ToLower() switch
                {
                    "borrowdate" => ascending ? query.OrderBy(b => b.BorrowDate) : query.OrderByDescending(b => b.BorrowDate),
                    "returndate" => ascending ? query.OrderBy(b => b.ReturnDate) : query.OrderByDescending(b => b.ReturnDate),
                    "book" => ascending ? query.OrderBy(b => b.Book.Title) : query.OrderByDescending(b => b.Book.Title),
                    "member" => ascending ? query.OrderBy(b => b.Member.Name) : query.OrderByDescending(b => b.Member.Name),
                    _ => query.OrderByDescending(b => b.BorrowDate)
                };

                var totalItems = await query.CountAsync();

                var result = await query
                    .Skip(RecordToPass)
                    .Take(pageSize)
                    .ToListAsync();
                var borrowDto = _mapper.Map<List<BorrowsHistoryDto>>(result);
                return new PagedResult<BorrowsHistoryDto>
                {
                    Data = borrowDto,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving borrows history");
                throw new Exception("Error retrieving borrows history", ex);
            }
        }

        public async Task<BorrowsHistoryDto> GetByIdAsync(int id)
        {
            try
            {
                var borrow = await _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (borrow == null)
                {
                    _logger.LogWarning($"Borrow with ID {id} not found");
                    return null;
                }

                return _mapper.Map<BorrowsHistoryDto>(borrow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving borrow history with ID {id}");
                throw new Exception($"Error retrieving borrow history with ID {id}", ex);
            }
        }

        public async Task<BorrowsHistoryDto> AddAsync(BorrowsHistoryCreateDto borrowsHistoryCreateDto)
        {
            try
            {
                var bookExists = await _context.Books.AnyAsync(b => b.Id == borrowsHistoryCreateDto.BookId);
                var memberExists = await _context.Members.AnyAsync(m => m.Id == borrowsHistoryCreateDto.MemberId);

                if (!bookExists || !memberExists)
                {
                    throw new Exception("Book or Member does not exist");
                }
                var isBookBorrowed = await _context.Borrows
                    .AnyAsync(b => b.BookId == borrowsHistoryCreateDto.BookId && b.ReturnDate == null);

                if (isBookBorrowed)
                {
                    throw new Exception("Book is currently borrowed");
                }

                var borrow = _mapper.Map<BorrowsHistory>(borrowsHistoryCreateDto);

                await _context.Borrows.AddAsync(borrow);

                var book = await _context.Books.FindAsync(borrowsHistoryCreateDto.BookId);
                book.IsAvailable = false;

                _context.Books.Update(book);

                await _context.SaveChangesAsync();

                return _mapper.Map<BorrowsHistoryDto>(borrow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating borrow history");
                throw new Exception("Error creating borrow history", ex);
            }
        }

        public async Task<BorrowsHistoryDto> UpdateAsync(int id, BorrowsHistoryCreateDto borrowsHistoryCreateDto)
        {
            try
            {
                var borrow = await _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (borrow == null)
                {
                    _logger.LogWarning($"Borrow with ID {id} not found");
                    return null;
                }

                if (borrow.BookId != borrowsHistoryCreateDto.BookId)
                {
                    var bookExists = await _context.Books.AnyAsync(b => b.Id == borrowsHistoryCreateDto.BookId);
                    if (!bookExists)
                    {
                        throw new Exception("Book does not exist");
                    }
                }

                if (borrow.MemberId != borrowsHistoryCreateDto.MemberId)
                {
                    var memberExists = await _context.Members.AnyAsync(m => m.Id == borrowsHistoryCreateDto.MemberId);
                    if (!memberExists)
                    {
                        throw new Exception("Member does not exist");
                    }
                }

                _mapper.Map(borrowsHistoryCreateDto, borrow);

                _context.Borrows.Update(borrow);

                await _context.SaveChangesAsync();

                return _mapper.Map<BorrowsHistoryDto>(borrow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating borrow history with ID {id}");
                throw new Exception($"Error updating borrow history with ID {id}", ex);
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var borrow = await _context.Borrows.FindAsync(id);
                if (borrow == null)
                {
                    _logger.LogWarning($"Borrow with ID {id} not found to delete it!");
                    return false;
                }

                _context.Borrows.Remove(borrow);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting borrow history with ID {id}");
                throw new Exception($"Error deleting borrow history with ID {id}", ex);
            }
        }

        public async Task<PagedResult<BorrowsHistoryDto>> GetBorrowsByMemberAsync(int memberId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var memberExists = await _context.Members.AnyAsync(m => m.Id == memberId);
                if (!memberExists)
                {
                    throw new Exception("Member does not exist");
                }

                var query = _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .Where(b => b.MemberId == memberId)
                    .OrderByDescending(b => b.BorrowDate)
                    .AsQueryable();

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var result = _mapper.Map<List<BorrowsHistoryDto>>(items);

                return new PagedResult<BorrowsHistoryDto>
                {
                    Data = result,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving borrows for member with ID {memberId}");
                throw new Exception($"Error retrieving borrows for member with ID {memberId}", ex);
            }
        }

        public async Task<PagedResult<BorrowsHistoryDto>> GetBorrowsByBookAsync(int bookId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
                if (!bookExists)
                {
                    throw new Exception("Book does not exist");
                }

                var query = _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .Where(b => b.BookId == bookId)
                    .OrderByDescending(b => b.BorrowDate)
                    .AsQueryable();

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var result = _mapper.Map<List<BorrowsHistoryDto>>(items);
                return new PagedResult<BorrowsHistoryDto>
                {
                    Data = result,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving borrows for book with ID {bookId}");
                throw new Exception($"Error retrieving borrows for book with ID {bookId}", ex);
            }
        }

        public async Task<PagedResult<BorrowsHistoryDto>> GetActiveBorrowsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Borrows
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .Where(b => b.ReturnDate == null)
                    .OrderByDescending(b => b.BorrowDate)
                    .AsQueryable();

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var result = _mapper.Map<List<BorrowsHistoryDto>>(items);
                return new PagedResult<BorrowsHistoryDto>
                {
                    Data = result,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active borrows");
                throw new Exception("Error retrieving active borrows", ex);
            }

        }

        public async Task<bool> ReturnBookAsync(int bookId)
        {
            try
            {
                var borrow = await _context.Borrows
                    .Include(b => b.Book)
                    .FirstOrDefaultAsync(b => b.BookId == bookId && b.ReturnDate == null);

                if (borrow == null)
                {
                    _logger.LogWarning("No Active borrow for this book");
                    return false;
                }

                if (borrow.ReturnDate!=null)
                {
                    throw new Exception("Book is already returned");
                }

                borrow.ReturnDate = DateTime.UtcNow;
                _context.Borrows.Update(borrow);

                borrow.Book.IsAvailable = true;
                _context.Books.Update(borrow.Book);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error returning book with ID {bookId}");
                throw new Exception($"Error returning book with ID {bookId}", ex);
            }

        }
    }
}
