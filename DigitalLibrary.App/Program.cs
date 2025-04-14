using DigitalLibrary.Core;

Console.WriteLine("app is working");
var book = new Book()
{
    Id = 1,
    Name = "C# in Depth",
    Author = "Jon Skeet",
    Year = 2019
};

Console.WriteLine($"Book ID: {book.Id}");
Console.WriteLine($"Book Name: {book.Name}");

Console.ReadLine();