using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.IRepository;
using Domain.Model;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
  public class UserRepository:RepositoryBase<Users>,IUserRepository, Domain.DI.ITransientDependency
  {
    public UserRepository(BlogDbContext dbContext) : base(dbContext)
    {
    }
  }
}
