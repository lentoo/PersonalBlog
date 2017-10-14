using Domain.UnitOfWork;
using Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
  public class UnitOfWork : IUnitOfWork, Domain.DI.IScopedDependency
  {
    private IDbContext _dbContext;
    public UnitOfWork(IDbContext _dbContext)
    {
      this._dbContext = _dbContext;
    }
    public bool Commit()
    {
      return _dbContext.SaveChanges() > 0;
    }

    public void RegisterClean<TEntity>(TEntity entity) where TEntity : class
    {
      _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
    }

    public void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class
    {
      _dbContext.Set<TEntity>().Remove(entity);
    }

    public void RegisterDirty<TEntity>(TEntity entity) where TEntity : class
    {
      _dbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified ;
      
    }

    public void RegisterNew<TEntity>(TEntity entity) where TEntity : class
    {
      _dbContext.Set<TEntity>().Add(entity);
    }

    public void Rollback()
    {
      throw new NotImplementedException();
    }
  }
}
