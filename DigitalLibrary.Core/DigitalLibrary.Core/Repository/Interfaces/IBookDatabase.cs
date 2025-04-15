using DigitalLibrary.Core.Models;

namespace DigitalLibrary.Core.Repository.Interfaces
{
    public interface IBookDatabase
    {
        public List<Book> GetBookByName(string name);
        public List<Book> GetBookByAuthor(string author);
        public void CheckoutBook(int id);
        public void ReturnBook(int id);
    }
}
