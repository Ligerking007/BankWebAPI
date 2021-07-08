using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public abstract class GenericRepository<TContext, TEntity> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext//, new()
    {
        // private readonly MCSurveyContext _dbContext;
        protected TContext Entities;
        protected DbSet<TEntity> DataSet;

        private bool ShareContext = false;

        public GenericRepository(TContext context)
        {
            Entities = context;
            DataSet = Entities.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return DataSet.AsQueryable();
        }

        public virtual IQueryable<TEntity> FromSql(string sql)
        {
            return DataSet.FromSqlRaw(sql);
        }
        public virtual IQueryable<TEntity> FromSql(string sql,object[] param)
        {
            return DataSet.FromSqlRaw(sql, param);
        }
        public virtual IQueryable<TEntity> GetAll()
        {
            return DataSet;
        }

        public virtual IQueryable<TEntity> Database()
        {
            return DataSet;
        }

        public virtual DbContext Entity()
        {
            return Entities;
        }
        public virtual int ExecuteSql(string sql, object[] param)
        {

            var result = Entities.Database.ExecuteSqlRaw(sql, param);

            return result;
        }
        public virtual EntityEntry<TEntity> Add(TEntity entity, bool IsCommit = false)
        {
            EntityEntry<TEntity> newEntry = null;
            try {
                newEntry = DataSet.Add(entity);
                Entities.Entry(entity).State = EntityState.Added;
                if(IsCommit){
                    Entities.SaveChanges();
                }
                return newEntry;
            }catch(Exception ex){
                throw ex;
            }
            return newEntry;
        }
        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, bool IsCommit = false)
        {
            EntityEntry<TEntity> newEntry = null;
            try
            {
                newEntry = await DataSet.AddAsync(entity);
                Entities.Entry(entity).State = EntityState.Added;
                if (IsCommit)
                {
                    await Entities.SaveChangesAsync();
                }
                return newEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newEntry;
        }
        public virtual EntityEntry<TEntity> Update(TEntity entity, bool IsCommit = false)
        { 
            EntityEntry<TEntity> updateEntry = null;
            try
            {
                updateEntry = DataSet.Update(entity);
                Entities.Entry(entity).State = EntityState.Modified;
                if (IsCommit)
                {
                    Entities.SaveChanges();
                }
                return updateEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return updateEntry;
        }
        public virtual async Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity, bool IsCommit = false)
        {
            EntityEntry<TEntity> updateEntry = null;
            try
            {
                updateEntry = await Task.Run(() => DataSet.Update(entity));
                Entities.Entry(entity).State = EntityState.Modified;
                if (IsCommit)
                {
                    await Entities.SaveChangesAsync();
                }
                return updateEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return updateEntry;
        }
        public virtual void UpdateRange(IEnumerable<TEntity> entity)
        {

            try
            {
                DataSet.UpdateRange(entity);
                Entities.Entry(entity).State = EntityState.Modified;
                if (!ShareContext)
                {
                    Entities.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
 
        }

        public virtual int Delete(TEntity entity, bool IsCommit = false)
        {
            try
            {
                DataSet.Remove(entity);
                if (IsCommit)
                {
                    Entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
            return 1;
        }

        public void AddRangeCommit(IEnumerable<TEntity> entitys)
        {
            try
            {
                DataSet.AddRange(entitys);
                if (!ShareContext)
                {
                    Entities.SaveChanges();
                }

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public virtual EntityEntry<TEntity> AddCommit(TEntity entity)
        {
            try
            {
                var newEntry = DataSet.Add(entity);
                Entities.Entry(entity).State = EntityState.Added;
                if (!ShareContext)
                {
                    Entities.SaveChanges();
                }

                return newEntry;
            }
            catch (Exception e)
            {
               
                throw e;
            }

        }

        public virtual TEntity UpdateCommit(TEntity entity)
        {
            try
            {
                Entities.Entry(entity).State = EntityState.Modified;
                if (!ShareContext)
                {
                    Entities.SaveChanges();
                }
            }
            catch (Exception e)
            {
              
                throw e;
            }
            return entity;
        }
        public virtual void Delete(TEntity entity)
        {
            DataSet.Remove(entity);
        }

        public virtual int DeleteCommit(TEntity entity)
        {
            try
            {
                DataSet.Remove(entity);
                if (!ShareContext)
                {
                    Entities.SaveChanges();
                }
            }
            catch (Exception e)
            {
               
                throw e;
            }
            return 1;
        }

       
        public virtual TEntity Update(TEntity entity)
        {
            Entities.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null) return DataSet.Any();
            else return DataSet.Any(predicate);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Entities.Database.BeginTransaction();
        }

        public IDbContextTransaction BeginTransaction(System.Data.IsolationLevel IsolationLevel)
        {
            return Entities.Database.BeginTransaction(IsolationLevel);
        }

        public virtual int Count
        {
            get
            {
                return DataSet.Count();
            }
        }

        

        public int GetMaxID(Expression<Func<TEntity, int?>> expression = null)
        {
            return Entities.Set<TEntity>().Max(expression) ?? 0;
        }



        public void AddRange(IQueryable<TEntity> entitys)
        {
            DataSet.AddRange(entitys);
        }

        public void AddRange(IEnumerable<TEntity> entitys)
        {
            DataSet.AddRange(entitys);
        }

        public void AddRange(List<TEntity> entitys)
        {
            DataSet.AddRange(entitys);
        }

        public virtual IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            var query = DataSet.Where(predicate).AsQueryable<TEntity>();
            return query;
        }

        public virtual IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = null;
            foreach (var include in includes)
            {
                if (query == null)
                {
                    query = DataSet.Include(include);
                }
                else
                {
                    query = query.Include(include);
                }

            }
            query = query.Where(predicate).AsQueryable<TEntity>();
            return query;
        }

        public virtual IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> filter, out int total, int index = 0, int size = 50)
        {
            total = 0;
            int skipCount = index * size;

            var _resetSet = filter != null ? DataSet.Where(filter).AsQueryable() : DataSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        private List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var query = DataSet.FirstOrDefault(predicate);
            return query;
        }

        public int DirectExecute(string sql)
        {
            return Entities.Database.ExecuteSqlRaw(sql);
        }

        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var query = DataSet.LastOrDefault(predicate);
            return query;
        }
    }
    class MyContext : DbContext
    {
        public override int SaveChanges()
        {
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext);
            }

            return base.SaveChanges();
        }
    }
}
