using Domain.IRepository;
using Domain.Model;
using Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository
{
  public class BlogCommentRepository : RepositoryBase<BlogComment>, IBlogCommentRepository, Domain.DI.ITransientDependency
  {
    public BlogCommentRepository(BlogDbContext dbContext) : base(dbContext){ }
  }
}
