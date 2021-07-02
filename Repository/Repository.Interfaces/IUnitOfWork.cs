using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;


namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable 
    {
        DbContext DataContext { get; }
        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();

        void RollBack(object entity);

        void RollBack();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }


}
