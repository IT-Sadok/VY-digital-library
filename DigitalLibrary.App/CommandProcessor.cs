using DigitalLibrary.Core.Models;
using DigitalLibrary.Core.Repository;

namespace DigitalLibrary.App
{
    public class CommandProcessor
    {
        private readonly BookDatabase _db;

        public CommandProcessor(BookDatabase db)
        {
            _db = db;
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Available commands: list, add <name> <author> <year>, delete <id>");
                return;
            }

            var command = args[0].ToLower();

            try
            {
                switch (command)
                {
                    case "help":
                        Console.WriteLine(
                            "Available commands:\n" +
                            "list                               - List all books\n" +
                            "add <name> <author> <year>         - Add a new book\n" +
                            "update <id> <name> <author> <year> - Add a new book\n" +
                            "delete <id>                        - Delete book by ID\n" +
                            "checkout <id>                      - Checkout book\n" +
                            "return <id>                        - Return book\n" +
                            "find -n <book name>                - Find books by name\n" +
                            "find -a <author name>              - Find books by author\n" +
                            "exit                               - Exit the application"
                        );
                        break;

                    case "list":
                        var books = _db.GetAll();
                        PrintBookTable(books);
                        break;

                    case "add":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Usage: add <name> <author> <year>");
                            break;
                        }

                        var name = args[1];
                        var author = args[2];
                        if (!int.TryParse(args[3], out int year))
                        {
                            Console.WriteLine("Year must be an integer.");
                            break;
                        }

                        _db.Add(new Book { Name = name, Author = author, Year = year });
                        _db.SaveChanges();
                        Console.WriteLine("Book added.");
                        break;

                    case "update":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Usage: update <id> <name> <author> <year>");
                            break;
                        }

                        if (!int.TryParse(args[1], out int id2update))
                        {
                            Console.WriteLine("Id must be an integer.");
                            break;
                        }
                        var name2update = args[2];
                        var author2update = args[3];
                        if (!int.TryParse(args[4], out int year2update))
                        {
                            Console.WriteLine("Year must be an integer.");
                            break;
                        }

                        _db.Update(id2update, new Book { Name = name2update, Author = author2update, Year = year2update });
                        _db.SaveChanges();
                        Console.WriteLine("Book added.");
                        break;

                    case "delete":
                        if (args.Length < 2 || !int.TryParse(args[1], out int id))
                        {
                            Console.WriteLine("Usage: delete <id>");
                            break;
                        }

                        _db.Delete(id);
                        _db.SaveChanges();
                        Console.WriteLine($"Book with id {id} deleted.");
                        break;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;

                    case "find":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("Usage: find -n <book name> OR find -a <author>");
                            break;
                        }

                        var flag = args[1];
                        //var value = string.Join(' ', args.Skip(2)).ToLower();
                        var value = args[2];
                        var results = new List<Book>();

                        if (flag == "-n")
                        {
                            results = _db.GetBookByName(value);
                        }
                        else if (flag == "-a")
                        {
                            results = _db.GetBookByAuthor(value);
                        }
                        else
                        {
                            Console.WriteLine("Invalid flag. Use -n for name or -a for author.");
                            break;
                        }

                        if (results.Count == 0)
                            Console.WriteLine("No books found.");
                        else
                            PrintBookTable(results);

                        break;

                    case "checkout":
                        if (args.Length < 2 || !int.TryParse(args[1], out var checkoutId))
                        {
                            Console.WriteLine("Usage: checkout <id>");
                            break;
                        }

                        try
                        {
                            _db.CheckoutBook(checkoutId); 
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "return":
                        if (args.Length < 2 || !int.TryParse(args[1], out var returnId))
                        {
                            Console.WriteLine("Usage: return <id>");
                            break;
                        }

                        try
                        {
                            _db.ReturnBook(returnId);
                            _db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void PrintBookTable(List<Book> books)
        {
            Console.WriteLine();
            Console.WriteLine($"{"ID",4} | {"Name",-30} | {"Author",-20} | {"Year",4} | {"Status",-10}");
            Console.WriteLine(new string('-', 80));

            foreach (var book in books)
            {
                Console.WriteLine($"{book.Id,4} | {book.Name,-30} | {book.Author,-20} | {book.Year,4} | {book.Status.ToString(),-10}");
            }

            Console.WriteLine();
        }
    }
}
