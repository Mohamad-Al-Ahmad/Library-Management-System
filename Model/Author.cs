using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangment.Model
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Country { get; set; }
        [Required]
        [MaxLength(50)]
        public string? City { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
