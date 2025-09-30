using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Author;
using LibraryManagement.DTOs.Member;
using LibraryManagement.Repositories.Interfaces;
using LibraryMangment.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberRepository> _logger;
        public MemberRepository(AppDbContext context, IMapper mapper, ILogger<MemberRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<MemberDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", bool ascending = true)
        {
            try
            {
                pageNumber = pageNumber < 1 ? 1 : pageNumber;
                pageSize = pageSize < 1 ? 10 : pageSize;
                int RecordToPass = pageSize * pageNumber - pageSize;

                var query = _context.Members
                    .Include(m => m.Borrows)
                    .ThenInclude(b => b.Book)
                    .AsQueryable();

                query = sortBy.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(m => m.Name) : query.OrderByDescending(m => m.Name),
                    "email" => ascending ? query.OrderBy(m => m.Email) : query.OrderByDescending(m => m.Email),
                    _ => query.OrderBy(m => m.Name)
                };

                var totalItems = await query.CountAsync();

                var result = await query
                    .Skip(RecordToPass)
                    .Take(pageSize)
                    .ToListAsync();

                var AuthorsDto = _mapper.Map<List<MemberDto>>(result);

                return new PagedResult<MemberDto>
                {
                    Data = AuthorsDto,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting members");
                throw new Exception("Error Getting members", ex);
            }
        }
        public async Task<MemberDto> GetByIdAsync(int id)
        {
            try
            {
                var member = await _context.Members
                    .Include(m => m.Borrows)
                    .ThenInclude(b => b.Book)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (member == null)
                {
                    _logger.LogWarning($"Member with ID {id} not found");
                    return null;
                }

                return _mapper.Map<MemberDto>(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving member with ID {id}");
                throw new Exception($"Error retrieving member with ID {id}", ex);
            }
        }
        public async Task<MemberDto> AddAsync(MemberCreateDto memberCreateDto)
        {
            try
            {
                if (await EmailExistsAsync(memberCreateDto.Email))
                {
                    throw new Exception("Email already exists");
                }

                var member = _mapper.Map<Member>(memberCreateDto);

                await _context.Members.AddAsync(member);
                await _context.SaveChangesAsync();

                return _mapper.Map<MemberDto>(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member");
                throw new Exception("Error creating member", ex);
            }
        }
        public async Task<MemberDto> UpdateAsync(int id, MemberCreateDto memberCreateDto)
        {
            try
            {
                var member = await _context.Members
                .Include(m => m.Borrows)
                    .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

                if (member == null)
                {
                    _logger.LogWarning($"Member with ID {id} was not found for update");
                    return null;
                }

                if (await EmailExistsAsync(memberCreateDto.Email, id))
                {
                    throw new Exception("Email already exists");
                }

                _mapper.Map(memberCreateDto, member);
                _context.Members.Update(member);
                await _context.SaveChangesAsync();

                return _mapper.Map<MemberDto>(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating member with ID {id}");
                throw new ApplicationException($"Error updating member with ID {id}", ex);
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                if (member == null)
                {
                    _logger.LogWarning($"Member with ID {id} was not found for delete");
                    return false;
                }

                var hasActiveBorrows = await _context.Borrows
                    .AnyAsync(b => b.MemberId == id && b.ReturnDate == null);

                if (hasActiveBorrows)
                {
                    throw new Exception("Cannot delete a member with active borrows");
                }

                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting member with ID {id}");
                throw new Exception($"Error deleting member with ID {id}", ex);
            }
        }
        public async Task<bool> EmailExistsAsync(string email, int? MemberId = null)
        {
            if (MemberId.HasValue)
            {
                return await _context.Members
                    .AnyAsync(m => m.Email == email && m.Id != MemberId.Value);
            }

            return await _context.Members
                .AnyAsync(m => m.Email == email);
        }

    }
}
