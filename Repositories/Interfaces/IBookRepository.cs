using LibraryManagement.DTOs.Author;
using LibraryManagement.DTOs.Book;

namespace LibraryManagement.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<PagedResult<BookDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "title", bool ascending = true);
        Task<BookDto> GetByIdAsync(int id);
        Task<BookDto> AddAsync(CreateBookDto createBookDto);
        Task<BookDto> UpdateAsync(int id, UpdateBookDto updateBookDto);
        Task<bool> DeleteAsync(int id);
    }
}
