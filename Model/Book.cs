using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangment.Model
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<BorrowsHistory>? Borrows { get; set; }
    }
}
