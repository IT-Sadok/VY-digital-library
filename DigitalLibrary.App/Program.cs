using DigitalLibrary.Core;

var directoryProvider = new DefaultDirectoryProvider();
var db = new BookDatabase(directoryProvider);

var books = db.GetAll();
//PrintBookTable(books);

//db.Add(new Book { Name = "Book1", Author = "Author1", Year = 2001 });
//db.Add(new Book { Name = "Book2", Author = "Author2", Year = 2002 });
//db.SaveChanges();

//db.Add(new Book { Name = "Book2Delete", Author = "Author2", Year = 2003 });
//db.Add(new Book { Name = "Book2Update", Author = "Author2", Year = 2004 });
//db.SaveChanges();
//db.Delete(3);
//db.SaveChanges();
//var updatedBook = new Book { Name = "Updated Book", Author = "Updated Author", Year = 2023 };
//db.Update(4, updatedBook);
//db.SaveChanges();

void PrintBookTable(List<Book> books)
{
    Console.WriteLine();
    Console.WriteLine($"{"ID",4} | {"Name",-30} | {"Author",-20} | {"Year",4}");
    Console.WriteLine(new string('-', 67));

    foreach (var book in books)
    {
        Console.WriteLine($"{book.Id,4} | {book.Name,-30} | {book.Author,-20} | {book.Year,4}");
    }

    Console.WriteLine();
}
