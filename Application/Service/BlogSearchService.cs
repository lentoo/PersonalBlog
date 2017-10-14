using Application.IService;
using Domain.AutoMapperConfig;
using Domain.IRepository;
using Domain.Model;
using Domain.ViewModel;
using Infrastructure.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
  /// <summary>
  /// bolg search
  /// </summary>
  public class BlogSearchService:IBlogSearchService,Domain.DI.ITransientDependency
  {
    private readonly IElasticsearchClient _elasticsearchClient;
    private readonly IUserRepository _userRepository;
    public BlogSearchService(IElasticsearchClient elasticsearchClient,IUserRepository userRepository)
    {
      _elasticsearchClient = elasticsearchClient;
      _userRepository = userRepository;
    }
    /// <summary>
    /// SearchBlogs
    /// </summary>
    /// <param name="keyword">关键字</param>
    /// <returns>查询到的文档和总数</returns>
    public async Task<(List<BlogView>,int)> SearchBlogs(string keyword)
    {
      var searchResponse = await _elasticsearchClient.HighQuery<Blog>(
        new Func<Nest.QueryContainerDescriptor<Blog>, Nest.QueryContainer>[]
        {
          query => query.Match(m => m.Field(f => f.Title).Query(keyword)),
          query => query.Match(m => m.Field(f => f.Content).Query(keyword)),
        },
        highFieldFunc: new Func<Nest.HighlightFieldDescriptor<Blog>, Nest.IHighlightField>[]{
                       h => h.Field(f => f.Title).HighlightQuery(q => q.Match(m => m.Field(f => f.Title).Query(keyword))),
                       h => h.Field(f => f.Content).HighlightQuery(q => q.Match(m => m.Field(f => f.Content).Query(keyword)))
       }
       );
      int count = searchResponse.Documents.Count;
      var blogs = new List<Blog>();
      var hits = searchResponse.Hits;
      foreach (var item in hits)
      {
        foreach (var key in item.Highlights)
        {
          foreach (var value in key.Value.Highlights)
          {
            if (key.Key == "title")
            {
              item.Source.Title = value;
            }
            else if (key.Key == "content")
            {
              item.Source.Content = value;
            }
          }
        }
        blogs.Add(item.Source);
      }
      var users = (await _userRepository.GetEntitys(u => blogs.Any(b => b.UserId == u.Id))).ToList();
      List<BlogView> blogviews = new List<BlogView>();
      foreach (var item in blogs)
      {
        BlogView blogView = AutoMapperContainer.MapTo<BlogView>(item);
        users.ForEach(user =>
        {
          if (user.Id == item.UserId)
          {
            blogView.ImgUrl = user.ImgUrl;
          }
        });
        blogviews.Add(blogView);
      }
      return (blogviews,count);
    }
  }
}
