using DigitalLibrary.Core.Models;
using DigitalLibrary.Core.Repository;
using DigitalLibrary.Core.Services.Intefaces;
using NUnit.Framework;

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
        }

        [TearDown]
        public void TearDown()
        {
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

        [Test]
        public void ConcurrentAdd_ShouldProduceUniqueIds()
        {
            // Arrange
            int threadCount = 10;
            int booksPerThread = 100;
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < booksPerThread; j++)
                    {
                        _db.Add(new Book
                        {
                            Name = $"Book_{j}",
                            Author = "Author",
                            Year = 2020
                        });
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var books = _db.GetAll();

            // Assert
            var uniqueIds = books.Select(b => b.Id).Distinct().Count();
            Assert.AreEqual(threadCount * booksPerThread, books.Count, "Some books were lost");
            Assert.AreEqual(books.Count, uniqueIds, "Duplicate book IDs detected");
        }

        [Test]
        public void AddMultipleBooksConcurrently_ShouldAddAllCorrectlyAndSequentially()
        {
            // Arrange
            var booksToAdd = Enumerable.Range(1, 100)
                .Select(i => new Book
                {
                    Name = $"Book_{i}",
                    Author = $"Author_{i}",
                    Year = 2000 + i
                }).ToList();

            var tasks = booksToAdd
                .Select(book => Task.Run(() => _db.Add(book)))
                .ToArray();

            // Act
            Task.WaitAll(tasks);
            _db.SaveChanges();

            // Assert
            var allBooks = _db.GetAll();
            Assert.AreEqual(100, allBooks.Count, "Not all books were added.");

            var distinctIds = allBooks.Select(b => b.Id).Distinct().Count();
            Assert.AreEqual(100, distinctIds, "Duplicate IDs detected.");

            var isSorted = allBooks.Select(b => b.Id).SequenceEqual(allBooks.OrderBy(b => b.Id).Select(b => b.Id));
            Assert.IsTrue(isSorted, "Books are not sorted by ID.");
        }

        [Test]
        public void UpdateMultipleBooksConcurrently_ShouldUpdateAllCorrectly()
        {
            // Arrange 
            var booksToAdd = Enumerable.Range(1, 100)
                .Select(i => new Book
                {
                    Name = $"Original_{i}",
                    Author = $"Author_{i}",
                    Year = 2000 + i
                }).ToList();

            foreach (var book in booksToAdd)
                _db.Add(book);
            _db.SaveChanges();

            var allBooks = _db.GetAll();

            // Act: 
            var updateTasks = allBooks.Select(book =>
                Task.Run(() =>
                {
                    var updated = new Book
                    {
                        Name = $"Updated_{book.Id}",
                        Author = $"UpdatedAuthor_{book.Id}",
                        Year = 3000 + book.Id
                    };
                    _db.Update(book.Id, updated);
                })
            ).ToArray();

            Task.WhenAll(updateTasks).Wait();
            _db.SaveChanges();

            // Assert: 
            var updatedBooks = _db.GetAll();

            foreach (var book in updatedBooks)
            {
                Assert.That(book.Name, Is.EqualTo($"Updated_{book.Id}"));
                Assert.That(book.Author, Is.EqualTo($"UpdatedAuthor_{book.Id}"));
                Assert.That(book.Year, Is.EqualTo(3000 + book.Id));
            }
        }

        [Test]
        public void ConcurrentUpdateSameBook_ResultShouldBeValid()
        {
            // Arrange
            var originalBook = new Book
            {
                Name = "Initial",
                Author = "Initial Author",
                Year = 2000
            };
            _db.Add(originalBook);
            _db.SaveChanges();

            var bookId = originalBook.Id;

            int updateCount = 100;
            var tasks = new List<Task>();

            for (int i = 0; i < updateCount; i++)
            {
                int index = i; 
                tasks.Add(Task.Run(() =>
                {
                    var updated = new Book
                    {
                        Name = $"Book_{index}",
                        Author = $"Author_{index}",
                        Year = 1900 + index
                    };

                    _db.Update(bookId, updated);
                }));
            }

            // Act
            Task.WhenAll(tasks).Wait();
            _db.SaveChanges();

            // Assert
            var final = _db.Get(bookId);
            Assert.That(final.Name, Does.StartWith("Book_"));
            Assert.That(final.Author, Does.StartWith("Author_"));
            Assert.That(final.Year, Is.GreaterThanOrEqualTo(1900).And.LessThan(2000 + updateCount));
        }

    }
}
