using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace async2;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            statusText.Text = "Creating database ...";
            await InitDatabaseAsync();
            authorComboBox.ItemsSource = await GetAuthorsAsync();
            searchButton.IsEnabled = true;
            statusText.Text = "Ready";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            statusText.Text = "Database error";
        }
    }

    private Task InitDatabaseAsync()
    {
        return Task.Run(() =>
        {
            using (LibraryContext context = new LibraryContext())
            {
                context.Database.EnsureCreated();
                if (context.Authors.Any())
                {
                    return;
                }

                Random rnd = new Random();
                string[] words = new string[]
                {
                    "Dark", "Silent", "Golden", "River", "Night", "Forest", "King", "Last", "Lost",
                    "Secret", "Winter", "Fire", "Shadow", "Garden", "Storm", "City", "Road", "Moon"
                };

                for (int i = 1; i <= 50; i++)
                {
                    Author author = new Author() { Name = "Author " + i };
                    for (int j = 1; j <= 1000; j++)
                    {
                        string title = words[rnd.Next(words.Length)] + " " + words[rnd.Next(words.Length)] + " " + rnd.Next(1, 10000);
                        author.Books.Add(new Book() { Title = title });
                    }
                    context.Authors.Add(author);
                }
                context.SaveChanges();
            }
        });
    }

    private async Task<List<Author>> GetAuthorsAsync()
    {
        using (LibraryContext context = new LibraryContext())
        {
            return await context.Authors.OrderBy(a => a.Id).ToListAsync();
        }
    }

    private async Task<List<Book>> GetBooksAsync(int authorId)
    {
        using (LibraryContext context = new LibraryContext())
        {
            return await context.Books
                .Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }
    }

    private async Task<List<Book>> SearchBooksAsync(string title)
    {
        using (LibraryContext context = new LibraryContext())
        {
            return await context.Books
                .Include(b => b.Author)
                .Where(b => b.Title.Contains(title))
                .ToListAsync();
        }
    }

    private async void AuthorComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Author? author = authorComboBox.SelectedItem as Author;
        if (author == null)
        {
            return;
        }

        statusText.Text = "Loading books ...";
        List<Book> books = await GetBooksAsync(author.Id);

        if (authorComboBox.SelectedItem != author)
        {
            return;
        }

        bookListBox.ItemsSource = books;
        statusText.Text = "Books :: " + books.Count;
    }

    private async void SearchButtonClick(object sender, RoutedEventArgs e)
    {
        string title = searchTextBox.Text.Trim();
        if (title.Length < 3)
        {
            MessageBox.Show("Enter at least 3 characters");
            return;
        }

        authorComboBox.SelectedItem = null;
        statusText.Text = "Searching ...";
        List<Book> books = await SearchBooksAsync(title);
        bookListBox.ItemsSource = books;
        statusText.Text = "Found :: " + books.Count;
    }
}