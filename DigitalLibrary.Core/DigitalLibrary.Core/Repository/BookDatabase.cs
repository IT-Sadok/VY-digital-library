using DigitalLibrary.Core.Models;
using DigitalLibrary.Core.Repository.Interfaces;
using DigitalLibrary.Core.Services.Intefaces;
using System.Text.Json;
using System.Xml.Linq;

namespace DigitalLibrary.Core.Repository
{
    public class BookDatabase : IJsonDataBase<Book>, IBookDatabase
    {
        private readonly string _filePath;
        private readonly List<Book> _books;
        private int _nextId;

        public BookDatabase(IDirectoryProvider provider)
        {
            var directory = provider.GetDataDirectory();
            _filePath = Path.Combine(directory, "books.json");
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
                _books = new List<Book>();
                _nextId = 1;
            }
            else
            {
                _books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_filePath));
                _nextId = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
            }
        }

        public List<Book> GetAll()
        {
            return _books;
        }

        public void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }


        public void Add(Book book)
        {
            book.Id = _nextId++;
            _books.Add(book);
        }

        public Book Get(int id)
        {
            var book = _books.SingleOrDefault(b => b.Id == id);
            if (book == null) throw new Exception($"Book with id {id} wasn't found");
            return book;
        }

        public void Delete(int id)
        {
            var book = _books.SingleOrDefault(b => b.Id == id);
            if (book == null) throw new Exception($"Book with id {id} wasn't found");
            _books.Remove(book);
        }

        public void Update(int id, Book entity)
        {
            var book = _books.SingleOrDefault(b => b.Id == id);
            if (book == null)
                throw new Exception($"Book with id {id} wasn't found");

            book.Name = entity.Name;
            book.Author = entity.Author;
            book.Year = entity.Year;
        }

        public List<Book> GetBookByName(string name)
        {
            var books = _books.Where(b => b.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (books == null) throw new Exception($"Book with author {name} wasn't found");
            return books;
        }

        public List<Book> GetBookByAuthor(string author)
        {
            var books = _books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
            if (books == null) throw new Exception($"Book with author {author} wasn't found");
            return books;
        }

        public void CheckoutBook(int id)
        {
            var book = _books.SingleOrDefault(b => b.Id == id);
            if (book == null) throw new Exception($"Book with id {id} wasn't found");
            if (book.Status == BookStatus.Reserved) throw new Exception($"This books is already reserved, please try later");
            book.Status = BookStatus.Reserved;
        }

        public void ReturnBook(int id)
        {
            var book = _books.SingleOrDefault(b => b.Id == id);
            if (book == null) throw new Exception($"Book with id {id} wasn't found");
            book.Status = BookStatus.Available;
        }
    }
}
