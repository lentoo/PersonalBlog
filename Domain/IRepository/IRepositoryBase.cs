using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Model;
using Domain.DI;
namespace Domain.IRepository
{
  public interface IRepositoryBase<T> where T:class,new() 
  {
    Task<T> GetEntity(Expression<Func<T, bool>> wherExpression);

    List<T> GetPageEntitys<TS>(int page, int count, Expression<Func<T, bool>> wherelamda,
      Expression<Func<T, TS>> orderLamda, bool isDesc);
    Task<IQueryable<T>> GetEntitys(Expression<Func<T, bool>> wherExpression);
    //Task<int> UpdateEntity(T entity);
    //Task<int> DeleteEntity(T entity);
    //Task<int> AddEntity(T entity);
    //Task<int> AddEntitys(List<T> entity);
    //bool AddEntitysNotAsync(List<T> entity);
    Task<int> GetEntitysCount();
  }
}