using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Repository.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> AsQueryable();
        EntityEntry<TEntity> Add(TEntity entity, bool IsCommit = false);
        EntityEntry<TEntity> Update(TEntity entity, bool IsCommit = false);
        int Delete(TEntity entity, bool IsCommit = false);
        IQueryable<TEntity> FromSql(string sql);
        IQueryable<TEntity> Database();
        DbContext Entity();

    }
}
