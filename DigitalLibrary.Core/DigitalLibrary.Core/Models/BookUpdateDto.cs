namespace DigitalLibrary.Core.Models;

class BookUpdateDto
{
    public string Name { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public BookStatus Status { get; set; }
}