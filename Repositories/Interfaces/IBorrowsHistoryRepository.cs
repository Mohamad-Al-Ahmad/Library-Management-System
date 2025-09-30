using LibraryManagement.DTOs.Borrow;

namespace LibraryManagement.Repositories.Interfaces
{
    public interface IBorrowsHistoryRepository
    {
        Task<PagedResult<BorrowsHistoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "BorrowDate", bool ascending = false);
        Task<BorrowsHistoryDto> GetByIdAsync(int id);
        Task<BorrowsHistoryDto> AddAsync(BorrowsHistoryCreateDto borrowsHistoryCreateDto);
        Task<BorrowsHistoryDto> UpdateAsync(int id, BorrowsHistoryCreateDto borrowsHistoryCreateDto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<BorrowsHistoryDto>> GetBorrowsByMemberAsync(int memberId, int pageNumber = 1, int pageSize = 10);
        Task<PagedResult<BorrowsHistoryDto>> GetBorrowsByBookAsync(int bookId, int pageNumber = 1, int pageSize = 10);
        Task<PagedResult<BorrowsHistoryDto>> GetActiveBorrowsAsync(int pageNumber = 1, int pageSize = 10);
        Task<bool> ReturnBookAsync(int bookId);
    }
}
