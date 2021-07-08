using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repository.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> AsQueryable();
        EntityEntry<TEntity> Add(TEntity entity, bool IsCommit = false);
		Task<EntityEntry<TEntity>> AddAsync(TEntity entity, bool IsCommit = false);
		EntityEntry<TEntity> Update(TEntity entity, bool IsCommit = false);
		Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity, bool IsCommit = false);

		int Delete(TEntity entity, bool IsCommit = false);
        IQueryable<TEntity> FromSql(string sql);
        IQueryable<TEntity> FromSql(string sql, object[] param);
        int ExecuteSql(string sql, object[] param);
        IQueryable<TEntity> Database();
        DbContext Entity();



		void Delete(TEntity entity);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="predicate"></param>
		void AddRangeCommit(IEnumerable<TEntity> entitys);
		EntityEntry<TEntity> AddCommit(TEntity entity);
		TEntity UpdateCommit(TEntity entity);
		int DeleteCommit(TEntity entity);
		void UpdateRange(IEnumerable<TEntity> entity);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IDbContextTransaction BeginTransaction();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="IsolationLevel"></param>
		/// <returns></returns>
		IDbContextTransaction BeginTransaction(System.Data.IsolationLevel IsolationLevel);
		/// <summary>
		/// Get the total objects count.
		/// </summary>
		int Count { get; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		int GetMaxID(Expression<Func<TEntity, int?>> predicate = null);

		void AddRange(IQueryable<TEntity> entitys);

		void AddRange(IEnumerable<TEntity> entitys);

		void AddRange(List<TEntity> entitys);

		IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate);

		IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

		IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> filter, out int total, int index = 0, int size = 50);

	

		TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

		TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate);
	}
}
