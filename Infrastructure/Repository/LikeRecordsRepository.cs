using Domain.IRepository;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.DbContext;

namespace Infrastructure.Repository
{
  public class LikeRecordsRepository : RepositoryBase<LikeRecords>, ILikeRecordsRepository, Domain.DI.ITransientDependency
  {
    public LikeRecordsRepository(BlogDbContext dbContext) : base(dbContext)
    {
    }
  }
}
