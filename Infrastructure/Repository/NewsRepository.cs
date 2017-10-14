using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.IRepository;
using Infrastructure.DbContext;

namespace Infrastructure.Repository
{
  public class NewsRepository : RepositoryBase<News>,INewsRepository, Domain.DI.ITransientDependency
  {
    public NewsRepository(BlogDbContext dbContext) : base(dbContext)
    {
    }
  }
}
