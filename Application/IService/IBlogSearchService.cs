using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
  public interface IBlogSearchService:Domain.DI.ITransientDependency
  {
    Task<(List<BlogView>, int)> SearchBlogs(string keyword);
  }
}
