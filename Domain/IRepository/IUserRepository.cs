using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Model;

namespace Domain.IRepository
{
  public interface IUserRepository:IRepositoryBase<Users>, DI.ITransientDependency
  {
    
  }
}