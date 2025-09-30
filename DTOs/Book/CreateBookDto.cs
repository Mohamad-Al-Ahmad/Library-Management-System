namespace LibraryManagement.DTOs.Book
{
    public class CreateBookDto
    {
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public int AuthorId { get; set; }
    }
}
