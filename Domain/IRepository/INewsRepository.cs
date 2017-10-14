using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.IRepository
{
  public interface INewsRepository : IRepositoryBase<News>, DI.ITransientDependency
  {
  }
}
