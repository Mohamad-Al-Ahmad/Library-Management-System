using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Author;
using LibraryManagement.DTOs.Book;
using LibraryManagement.Repositories.Interfaces;
using LibraryMangment.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class BooksRepository : IBookRepository

    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksRepository> _logger;
        public BooksRepository(AppDbContext context, IMapper mapper, ILogger<BooksRepository> logger)
        {
            _context = context;
            _mapper = mapper; _logger = logger;
        }

        public async Task<PagedResult<BookDto>> GetAllAsync(
            int pageNumber = 1, int pageSize = 10,
            string sortBy = "title", bool ascending = true)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;
                int RecordToPass = pageSize * pageNumber - pageSize;

                var query = _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Borrows).AsQueryable();
                query = sortBy.ToLower() switch
                {
                    "title" => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title),
                    "publisheddate" => ascending ? query.OrderBy(b => b.PublishedDate) : query.OrderByDescending(b => b.PublishedDate),
                    "author" => ascending ? query.OrderBy(b => b.Author.Name) : query.OrderByDescending(b => b.Author.Name),
                    "isavailable" => ascending ? query.OrderBy(b => b.IsAvailable) : query.OrderByDescending(b => b.IsAvailable),
                    _ => query.OrderBy(b => b.Title)
                };
                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip(RecordToPass)
                    .Take(pageSize)
                    .ToListAsync();

                var bookDto = _mapper.Map<List<BookDto>>(items);
                return new PagedResult<BookDto>
                {
                    Data = bookDto,
                    TotalItems = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Books");
                throw;
            }
        }
        public async Task<BookDto> GetByIdAsync(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Borrows)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _logger.LogWarning($"Book with ID {id} was not found");
                    return null;
                }

                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching Book with ID {id}");
                throw;
            }
        }
        public async Task<BookDto> AddAsync(CreateBookDto createBookDto)
        {
            try
            {
                if (createBookDto == null)
                {
                    throw new Exception("Book data cannot be null");
                }
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == createBookDto.AuthorId);
                if (!authorExists)
                {
                    throw new Exception("Book does not exist");
                }

                var book = _mapper.Map<Book>(createBookDto);
                book.IsAvailable = true;
                var exists = await _context.Books
                    .AnyAsync(a => a.Title == book.Title && a.AuthorId == book.AuthorId);
                if (exists)
                {
                    throw new Exception("An Book with the same Title and Author already exists");
                }
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding Book");
                throw;
            }
        }
        public async Task<BookDto> UpdateAsync(
            int id, UpdateBookDto updateBookDto)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Borrows)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _logger.LogWarning($"Book with ID {id} was not found for update");
                    return null;
                }
                if (book.AuthorId != updateBookDto.AuthorId)
                {
                    var authorExists = await _context.Authors.AnyAsync(a => a.Id == updateBookDto.AuthorId);
                    if (!authorExists)
                    {
                        throw new Exception("Author does not exist");
                    }
                }

                _mapper.Map(updateBookDto, book);
                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return _mapper.Map<BookDto>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while updating book with ID {id}");
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID {id} was not found for delete");
                    return false;
                }
                if (!book.IsAvailable)
                {
                    throw new Exception("Can not delete a book that is currently borrowed");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while deleting Book with ID {id}");
                throw;
            }
        }
    

    }
}
