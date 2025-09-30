using AutoMapper;

using LibraryManagement.Data;
using LibraryManagement.DTOs.Author;
using LibraryManagement.Repositories.Interfaces;
using LibraryMangment.Model;

using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorRepository> _logger;
        public AuthorRepository(AppDbContext context, IMapper mapper, ILogger<AuthorRepository> logger)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<AuthorDto>> GetAllAsync(
            int pageNumber = 1, int pageSize = 10,
            string sortBy = "Name", bool ascending = true)
        {
            try
            {
                pageNumber = pageNumber < 1 ? 1 : pageNumber;
                pageSize = pageSize < 1 ? 10 : pageSize;
                int RecordToPass = pageSize * pageNumber - pageSize;

                var query = _context.Authors.Include(a => a.Books).AsQueryable();

                query = sortBy.ToLower() switch
                {
                    "Name" => ascending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name),
                    "Id" => ascending ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id),
                    "City" => ascending ? query.OrderBy(a => a.City) : query.OrderByDescending(a => a.City),
                    "Country" => ascending ? query.OrderBy(a => a.Country) : query.OrderByDescending(a => a.Country),
                    _ => ascending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name)
                };

                var totalCount = await query.CountAsync();

                var result = await query
                    .Skip(RecordToPass)
                    .Take(pageSize)
                    .ToListAsync();

                var AuthorsDto = _mapper.Map<List<AuthorDto>>(result);
                return new PagedResult<AuthorDto>
                {
                    Data = AuthorsDto,
                    TotalItems = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Authors");
                throw new Exception("Error fetching Authors", ex);
            }
        }
        public async Task<AuthorDto> GetByIdAsync(int id)
        {
            try
            {
                var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
                if (author is null)
                {
                    _logger.LogWarning($"Author with ID {id} not found");
                    return null;
                }
                return _mapper.Map<AuthorDto>(author);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching Author with ID {id}");
                throw;
            }
        }
        public async Task<AuthorDto> AddAsync(AuthorCreateDto authorCreateDto)
        {
            try
            {
                if (authorCreateDto == null)
                {
                    throw new Exception( "Author data cannot be null");
                }

                var author = _mapper.Map<Author>(authorCreateDto);
                var exists = await _context.Authors
                    .AnyAsync(a => a.Name == author.Name && a.City == author.City);

                if (exists)
                {
                    throw new Exception("An author with the same name and city already exists");
                }

                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
               
                return _mapper.Map<AuthorDto>(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Unexpected error occurred while adding author");
                throw;
            }
        }
        public async Task<AuthorDto> UpdateAsync(int id, AuthorCreateDto authorCreateDto)
        {
            try
            {
                if (authorCreateDto == null)
                {
                    throw new Exception("Author update data cannot be null");
                }

                var existingAuthor = await _context.Authors.FindAsync(id);

                if (existingAuthor == null)
                {
                    _logger.LogWarning($"Author with ID {id} was not found for update");
                    return null;
                }

                var duplicateExists = await _context.Authors
                    .AnyAsync(a => a.Id != id &&
                                  a.Name == authorCreateDto.Name &&
                                  a.City == authorCreateDto.City);

                if (duplicateExists)
                {
                    throw new Exception("Another author with the same name and city already exists");
                }

                _mapper.Map(authorCreateDto, existingAuthor);
                _context.Authors.Update(existingAuthor);
                await _context.SaveChangesAsync();
                return _mapper.Map<AuthorDto>(existingAuthor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while updating author with ID {id}");
                throw;
            }
        }
        //الحذف لا يمكن في حال وجود كتب مرتبطة 
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingAuthor = await _context.Authors.FindAsync(id);

                if (existingAuthor == null)
                {
                    _logger.LogWarning($"Author with ID {id} was not found for delete");
                    return false;
                }

                var hasRelatedBooks = await _context.Books
                    .AnyAsync(b => b.AuthorId == existingAuthor.Id);

                if (hasRelatedBooks)
                {
                    throw new Exception("Cannot delete author with existing books. Delete the books first.");
                }

                _context.Authors.Remove(existingAuthor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred while deleting author with ID {id}");
                throw;
            }
        }
        
    }
}
