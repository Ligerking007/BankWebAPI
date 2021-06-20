using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Repository.Entities
{
    public abstract class GenericRepository<TContext, TEntity> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext//, new()
    {
        // private readonly MCSurveyContext _dbContext;
        protected TContext Entities;
        protected DbSet<TEntity> DataSet;

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

    }
}
