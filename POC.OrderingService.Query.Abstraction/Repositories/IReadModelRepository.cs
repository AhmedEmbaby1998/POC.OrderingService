public interface IReadModelRepository<T> where T : class
    {
        Task<T?> GetAsync(Guid id);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
    }
