namespace LibraryManagement.DTOs.Book
{
    public class UpdateBookDto
    {
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
    }
}
