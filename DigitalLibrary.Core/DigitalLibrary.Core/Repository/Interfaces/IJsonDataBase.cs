namespace DigitalLibrary.Core.Repository.Interfaces;
public interface IJsonDataBase<T> where T : class
{
    public List<T> GetAll();
    public void SaveChanges();
    public void Add(T entity);
    public T Get(int id);
    public void Delete(int id);
    public void Update(int id, T entity);
}