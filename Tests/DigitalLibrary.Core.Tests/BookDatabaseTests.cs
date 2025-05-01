using DigitalLibrary.Core.Models;
using DigitalLibrary.Core.Repository;
using DigitalLibrary.Core.Services.Intefaces;
using NUnit.Framework;
//using NUnit.Framework;

namespace DigitalLibrary.Core.Tests
{
    [TestFixture]
    public class Tests
    {
        private IDirectoryProvider _directoryProvider;
        private BookDatabase _db;
        private const string _testFileName = "books-test.json";

        [SetUp]
        public void Setup()
        {
            _directoryProvider = new TestDirectoryProvider();
            _db = new BookDatabase(_directoryProvider, _testFileName);

            var filePath = Path.Combine(_directoryProvider.GetDataDirectory(), _testFileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Test]
        public void Add_ShouldAddBookToDatabase()
        {
            var book = new Book { Name = "Test Book", Author = "Tester", Year = 2020 };
            _db.Add(book);
            _db.SaveChanges();

            var addedBook = _db.GetAll().First();
            Assert.AreEqual("Test Book" , addedBook.Name);
            Assert.AreEqual("Tester", addedBook.Author);
            Assert.AreEqual(2020, addedBook.Year);
            Assert.AreEqual(1, addedBook.Id); 
        }

        [Test]
        public void Get_ShouldReturnBookById()
        {
            var book = new Book { Name = "Test Book", Author = "Tester", Year = 2020 };
            _db.Add(book);
            _db.SaveChanges();

            var result = _db.Get(book.Id);
            Assert.AreEqual(book.Name, result.Name);
            Assert.AreEqual(book.Author, result.Author);
            Assert.AreEqual(book.Year, result.Year);
        }

        [Test]
        public void Get_ShouldThrowException_WhenBookNotFound()
        {
            Assert.Throws<Exception>(() => _db.Get(999)); 
        }

        [Test]
        public void Delete_ShouldRemoveBookFromDatabase()
        {
            var book = new Book { Name = "Test Book", Author = "Tester", Year = 2020 };
            _db.Add(book);
            _db.SaveChanges();

            _db.Delete(book.Id);
            _db.SaveChanges();

            Assert.Throws<Exception>(() => _db.Get(book.Id)); 
        }

        [Test]
        public void Delete_ShouldThrowException_WhenBookNotFound()
        {
            Assert.Throws<Exception>(() => _db.Delete(999)); 
        }

        [Test]
        public void Update_ShouldModifyBookDetails()
        {
            var book = new Book { Name = "Test Book", Author = "Tester", Year = 2020 };
            _db.Add(book);
            _db.SaveChanges();

            var updatedBook = new Book { Name = "Updated Book", Author = "Updated Author", Year = 2022 };
            _db.Update(book.Id, updatedBook);
            _db.SaveChanges();

            var result = _db.Get(book.Id);
            Assert.AreEqual("Updated Book", result.Name);
            Assert.AreEqual("Updated Author", result.Author);
            Assert.AreEqual(2022, result.Year);
        }

        [Test]
        public void Update_ShouldThrowException_WhenBookNotFound()
        {
            var updatedBook = new Book { Name = "Updated Book", Author = "Updated Author", Year = 2022 };
            Assert.Throws<Exception>(() => _db.Update(999, updatedBook)); 
        }

        [Test]
        public void GetBookByName_ShouldReturnMatchingBooks()
        {
            var book = new Book { Name = "Book by name", Author = "Author", Year = 2000 };
            _db.Add(book);
            _db.SaveChanges();

            var result = _db.GetBookByName("by name");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Book by name"));
        }

        [Test]
        public void GetBookByAuthor_ShouldReturnMatchingBooks()
        {
            var book = new Book { Name = "Book by author", Author = "Author123", Year = 2000 };
            _db.Add(book);
            _db.SaveChanges();

            var result = _db.GetBookByAuthor("Author123");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Author, Is.EqualTo("Author123"));
        }

        [Test]
        public void CheckoutBook_ShouldChangeStatusToReserved()
        {
            var book = new Book { Name = "Domain-Driven Design", Author = "Eric Evans", Year = 2003 };
            _db.Add(book);
            _db.SaveChanges();

            _db.CheckoutBook(book.Id);
            var updated = _db.Get(book.Id);
            Assert.That(updated.Status, Is.EqualTo(BookStatus.Reserved));
        }

        [Test]
        public void ReturnBook_ShouldChangeStatusToAvailable()
        {
            var book = new Book { Name = "Refactoring", Author = "Martin Fowler", Year = 1999, Status = BookStatus.Reserved };
            _db.Add(book);
            _db.SaveChanges();

            _db.ReturnBook(book.Id);
            var updated = _db.Get(book.Id);
            Assert.That(updated.Status, Is.EqualTo(BookStatus.Available));
        }


    }
}
