using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.IRepository;
using Domain.Model;
using Infrastructure.DbContext;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastructure.Repository
{
  public class BlogRepository : RepositoryBase<Blog>, IBlogRepository, Domain.DI.ITransientDependency
  {
    private readonly BlogDbContext dbContext;
    public BlogRepository(BlogDbContext dbContext) : base(dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<BlogView[]> GetPageEntitys(Expression<Func<Blog, bool>> wherExpression)
    {
      
      var blogViews = dbContext.Set<Blog>()
                 .GroupJoin
                 (dbContext.BlogComment,
                  b => b.Id, t => t.BlogId,
                  (b, bc) => new BlogView
                  {
                    Id = b.Id,
                    Author = b.Author,
                    Category = b.Category,
                    Content = b.Content,
                    Description = b.Description,
                    Keywords = b.Keywords,
                    PublishedTime = b.PublishedTime,
                    Title = b.Title,
                    UserId = b.UserId,
                    VisitsNumber = b.VisitsNumber,
                    CommentCount=bc.Count()
                  })
                  .Select(o => o).ToArray();
      var users =await dbContext.User.Where(user => blogViews.Any(l => user.Id == l.UserId)).Select(u => new { userId = u.Id, imgUrl = u.ImgUrl }).ToArrayAsync();
      for (int i = 0; i < blogViews.Count(); i++)
      {
        foreach (var user in users)
        {
          if (blogViews[i].UserId == user.userId)
          {
            blogViews[i].ImgUrl = user.imgUrl;
            break;
          }
        }        
      }
      //var list = from b in DbContext.Blogs
      //           join bc in DbContext.BlogComment on b.Id equals bc.BlogId
      //           join user in DbContext.User on b.UserId equals user.Id
      //           //into temp
      //           //from tt in temp.DefaultIfEmpty()
      //           select new BlogView
      //           {
      //             Id = b.Id,
      //             Author = b.Author,
      //             Category = b.Category,
      //             Content = b.Content,
      //             Description = b.Description,
      //             Keywords = b.Keywords,
      //             PublishedTime = b.PublishedTime,
      //             Title = b.Title,
      //             UserId = b.UserId,
      //             VisitsNumber = b.VisitsNumber,
      //             ImgUrl = user.ImgUrl,
      //             CommentCount = DbContext.BlogComment.Count(blog => blog.Id == bc.BlogId)
      //           };
      return blogViews;
    }
  }
}