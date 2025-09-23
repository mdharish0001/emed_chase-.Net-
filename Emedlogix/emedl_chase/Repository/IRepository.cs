
namespace emedl_chase.Repository
{
    public partial interface IRepository<T> where T : class
    {

        T GetById(object id);

        Task<T> GetByIdAsync(object id);


        T Insert(T entity);

        Task<T> InsertAsync(T entity);

        IEnumerable<T> Insert(IEnumerable<T> entities);

        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities);

        void Update(T entity);

        bool Delete(T entity);

        Task<bool> DeleteAsync(T entity);

        bool Delete(IEnumerable<T> entities);

        Task<bool> DeleteAsync(IEnumerable<T> entities);

        void Save();

        Task<bool> SaveAsync();

        IQueryable<T> Table { get; }

        IQueryable<T> TableNoTracking { get; }
        Tuple<int, int> ExecuteCommand(string commandText);

        IQueryable<T> SelectQuery(string query);

        T GetByIdWithDetach(object id);
    }
}
