using LibraryManagement.DTOs.Member;

namespace LibraryManagement.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        Task<PagedResult<MemberDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", bool ascending = true);
        Task<MemberDto> GetByIdAsync(int id);
        Task<MemberDto> AddAsync(MemberCreateDto memberCreateDto);
        Task<MemberDto> UpdateAsync(int id, MemberCreateDto memberCreateDto);
        Task<bool> DeleteAsync(int id);
         Task<bool> EmailExistsAsync(string email, int? MemberId=null);
    }
}
