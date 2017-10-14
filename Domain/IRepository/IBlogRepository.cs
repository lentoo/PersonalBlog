using Domain.Model;
using Domain.ViewModel;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.IRepository
{
  public interface IBlogRepository : IRepositoryBase<Blog>, DI.ITransientDependency
  {
    Task<BlogView[]> GetPageEntitys(Expression<Func<Blog, bool>> wherExpression);
  }
}