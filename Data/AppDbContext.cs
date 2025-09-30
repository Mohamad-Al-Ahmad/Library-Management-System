using LibraryMangment.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BorrowsHistory> Borrows { get; set; }

    }
}
