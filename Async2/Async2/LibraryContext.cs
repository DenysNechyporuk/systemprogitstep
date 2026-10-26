using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace async2;

public class LibraryContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    public LibraryContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source = localhost\SQLEXPRESS01;
                                      Initial Catalog = LibraryDb;
                                      Integrated Security = True;
                                      Trust Server Certificate = True");
    }
}

public class Author
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public ICollection<Book> Books { get; set; }

    public Author()
    {
        Books = new HashSet<Book>();
    }
}

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    public int AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public Author Author { get; set; }

    public override string ToString()
    {
        return $"{Title} by {Author.Name}";
    }
}