using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.UnitOfWork
{
  public interface IUnitOfWork:DI.IScopedDependency
  {
    /// <summary>
    /// 添加
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    void RegisterNew<TEntity>(TEntity entity) where TEntity : class;
    /// <summary>
    /// 修改
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    void RegisterDirty<TEntity>(TEntity entity) where TEntity : class;
    /// <summary>
    /// 取消修改
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    void RegisterClean<TEntity>(TEntity entity) where TEntity : class;
    /// <summary>
    /// 删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class;
    /// <summary>
    /// 提交操作
    /// </summary>
    /// <returns></returns>
    bool Commit();
    void Rollback();
  }
}
