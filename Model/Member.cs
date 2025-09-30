using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangment.Model
{
    public class Member
    {
        
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; } 
        public ICollection<BorrowsHistory>? Borrows { get; set; }
    }
}
