using LibraryManagement.DTOs.Member;
using LibraryManagement.Repositories;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ILogger<MembersController> _logger;
        public MembersController(IMemberRepository memberRepository, ILogger<MembersController> logger)
        {
            _memberRepository = memberRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<MemberDto>>> GetMembers(
             int pageNumber = 1,int pageSize = 10,
             string sortBy = "Name",bool ascending = true)
        {
            try
            {
                var result = await _memberRepository.GetAllAsync(pageNumber, pageSize, sortBy, ascending);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching members");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetMember(int id)
        {
            try
            {
                var member = await _memberRepository.GetByIdAsync(id);

                if (member == null)
                {
                    return NotFound();
                }

                return Ok(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching member with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MemberDto>> CreateMember(MemberCreateDto memberCreateDto)
        {
            try
            {
              
                var result = await _memberRepository.AddAsync(memberCreateDto);

                return CreatedAtAction(nameof(GetMember), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, MemberCreateDto memberCreateDto)
        {
            try
            {
                var result = await _memberRepository.UpdateAsync(id, memberCreateDto);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating member with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                var result = await _memberRepository.DeleteAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting member with ID {MemberId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
