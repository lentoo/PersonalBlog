using Domain.DI;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.IRepository
{
  public interface IBlogCommentRepository : IRepositoryBase<BlogComment>,ITransientDependency
  {
  }
}
