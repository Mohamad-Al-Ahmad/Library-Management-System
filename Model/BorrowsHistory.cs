using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryMangment.Model
{
    public class BorrowsHistory
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required]
        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set;}
    }
}
