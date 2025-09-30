using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Borrow
{
    public class BorrowsHistoryCreateDto
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public int MemberId { get; set; }
        [Required]
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
