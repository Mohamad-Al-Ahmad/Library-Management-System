
using LibraryManagement.DTOs.Book;

namespace LibraryManagement.DTOs.Author
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public List<BookDto> Books { get; set; } = new List<BookDto>();

    }
}
