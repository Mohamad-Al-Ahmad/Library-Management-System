using LibraryManagement.DTOs.Author;
using LibraryMangment.Model;

namespace LibraryManagement.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<PagedResult<AuthorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", bool ascending = true);
        Task<AuthorDto> GetByIdAsync(int id);
        Task<AuthorDto> AddAsync(AuthorCreateDto authorCreateDto);
        Task<AuthorDto> UpdateAsync(int id, AuthorCreateDto author);
        //الحذف لا يمكن في حال وجود كتب مرتبطة   
        Task<bool> DeleteAsync(int id);
    }
}
