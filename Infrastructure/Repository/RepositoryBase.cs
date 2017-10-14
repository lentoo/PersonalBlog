using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.IRepository;
using Domain.Model;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
  public class RepositoryBase<T> : IRepositoryBase<T> where T : class, new()
  {
    
    public BlogDbContext DbContext { get; set; }
    public RepositoryBase(BlogDbContext dbContext)
    {
      DbContext = dbContext;
    }

    public virtual Task<T> GetEntity(Expression<Func<T, bool>> wherExpression)
    {
      return DbContext.Set<T>().Where(wherExpression).FirstOrDefaultAsync();
    }

    public virtual List<T> GetPageEntitys<TS>(int page, int count, Expression<Func<T, bool>> wherelamda, Expression<Func<T, TS>> orderLamda, bool isDesc)
    {
      var list = DbContext.Set<T>().Where(wherelamda);
      list = isDesc ? list.OrderByDescending(orderLamda) : list.OrderBy(orderLamda);
      list = list.Skip((page - 1) * count).Take(count);
      return list.ToList();
    }

    public virtual Task<IQueryable<T>> GetEntitys(Expression<Func<T, bool>> wherExpression)
    {
      return Task.FromResult(DbContext.Set<T>().Where(wherExpression).AsNoTracking());
    }

    //public Task<int> UpdateEntity(T entity)
    //{
    //  DbContext.Entry(entity).State = EntityState.Modified;

    //  return DbContext.SaveChangesAsync();
    //}

    //public Task<int> DeleteEntity(T entity)
    //{
    //  DbContext.Entry(entity).State = EntityState.Deleted;

    //  return DbContext.SaveChangesAsync();
    //}

    //public Task<int> AddEntity(T entity)
    //{
    //  DbContext.Set<T>().AddAsync(entity);
    //  return DbContext.SaveChangesAsync();
    //}
    //public Task<int> AddEntitys(List<T> entity)
    //{
    //  DbContext.Set<T>().AddRangeAsync(entity);
    //  return DbContext.SaveChangesAsync();
    //}
    //public bool AddEntitysNotAsync(List<T> entity)
    //{
    //  DbContext.Set<T>().AddRange(entity);
    //  return DbContext.SaveChanges() > 0;
    //}
    public virtual Task<int> GetEntitysCount()
    {
      return DbContext.Set<T>().CountAsync();
    }
  }
}