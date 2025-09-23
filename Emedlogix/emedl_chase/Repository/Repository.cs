
using emedl_chase;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;


namespace emedl_chase.Repository
{
    public partial class Repository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly ChaseDbContext _context;
        private DbSet<T> _entities;

        #endregion

        #region Ctor

        public Repository(ChaseDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }


        public virtual T GetById(object id)
        {
            return this.GetByIdAsync(id).Result;

        }

        public async Task<T> GetByIdAsync(object id)
        {

            return await this.Entities.FindAsync(id);
        }

        public virtual T Insert(T entity)
        {
            this.Entities.Add(entity);
            this._context.SaveChanges();

            return entity;
        }

        public virtual async Task<T> InsertAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                await this.Entities.AddAsync(entity);
                await this._context.SaveChangesAsync();

                return entity;

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public virtual IEnumerable<T> Insert(IEnumerable<T> entities)
        {
            return this.InsertAsync(entities).Result;
        }

        public virtual async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            try
            {

                var validationResults = new List<ValidationResult>();
                foreach (var entity in entities)
                {
                    if (!Validator.TryValidateObject(entity, new ValidationContext(entity), validationResults))
                    {
                        // throw new ValidationException() or do whatever you want
                    }
                }

                await this.Entities.AddRangeAsync(entities);
                await this._context.SaveChangesAsync();
                return entities;
            }
            catch (DbUpdateException ex)
            {
                throw ex;
            }

        }

        public virtual void Update(T entity)
        {
            try
            {
                this.Entities.Update(entity);
                this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public virtual bool Delete(T entity)
        {

            if (entity == null)
                throw new ArgumentNullException("entity");

            return this.DeleteAsync(entity).Result;

        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {

            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Remove(entity);
            await this._context.SaveChangesAsync();
            return true;
        }

        public virtual bool Delete(IEnumerable<T> entities)
        {

            return this.DeleteAsync(entities).Result;
        }

        public virtual async Task<bool> DeleteAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            this.Entities.RemoveRange(entities);
            await this._context.SaveChangesAsync();
            return true;

        }


        public virtual void Save()
        {
            this._context.SaveChanges();
        }

        public virtual async Task<bool> SaveAsync()
        {
            try
            {
                await this._context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }




        #endregion



        public virtual Tuple<int, int> ExecuteCommand(string commandText)
        {
            var parameterReturn = new NpgsqlParameter
            {
                ParameterName = "jid",
                NpgsqlDbType = NpgsqlDbType.Integer,
                Direction = System.Data.ParameterDirection.Output,
            };
            int afftectRows = this._context.Database.ExecuteSqlRaw(commandText, parameterReturn);
            int returnId = 0;
            if (parameterReturn != null && parameterReturn.Value != null && !string.IsNullOrEmpty(parameterReturn.Value.ToString()) && !string.IsNullOrWhiteSpace(parameterReturn.Value.ToString()))
            {
                returnId = Convert.ToInt32(parameterReturn.Value);
            }
            return Tuple.Create(afftectRows, returnId);
        }


        public virtual IQueryable<T> SelectQuery(string query)
        {
            return this._context.Set<T>().FromSqlRaw(query).AsQueryable();
        }

        public T GetByIdWithDetach(object id)
        {

            var model = this.Entities.Find(id);
            this._context.Entry(model).State = EntityState.Detached;
            return model;
        }

    }
}
