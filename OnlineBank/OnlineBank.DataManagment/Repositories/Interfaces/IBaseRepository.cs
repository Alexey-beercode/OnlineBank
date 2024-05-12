namespace OnlineBank.DataManagment.Repositories.Interfaces;

public interface IBaseRepository<T>
{
    Task<T> GetById(Guid id);
    Task Update(T entity);
    Task Delete(T entity);
    Task<List<T>> GetAll();
    Task Create(T entity);
}