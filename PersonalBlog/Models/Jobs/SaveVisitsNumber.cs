using Domain.IRepository;
using Domain.Model;
using Domain.UnitOfWork;
using Hangfire;
using Infrastructure.Common;
using Infrastructure.Common.Cached;
using Infrastructure.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBlog.Models.Jobs
{
  public class SaveVisitsNumber
  {
    private readonly IBlogRepository _blogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheClient _cacheClient;
    private readonly IElasticsearchClient _elasticsearchClient;
    public SaveVisitsNumber(IBlogRepository blogRepository, ICacheClient cacheClient, IUnitOfWork unitOfWork, IElasticsearchClient elasticsearchClient)
    {
      _blogRepository = blogRepository;
      _cacheClient = cacheClient;
      _unitOfWork = unitOfWork;
      _elasticsearchClient = elasticsearchClient;
    }
    public static void StartSave()
    {
      BackgroundJob.Enqueue<SaveVisitsNumber>(obj => obj.Save());
    }
    public async Task Save()
    {
      //获取阅读量改变的blog列表
      HashSet<string> changeBlogVisits = _cacheClient.GetCache<HashSet<string>>(MyDictionary.ChangeBlogVisits);
      if (changeBlogVisits != null)
      {
        var blogs = await _blogRepository.GetEntitys(o => changeBlogVisits.Contains(o.Id));
        var blogList = blogs.ToList();
        foreach (var item in blogList)
        {
          int visitsNum = _cacheClient.GetCache<int>(item.Id);
          item.VisitsNumber = visitsNum;
          _unitOfWork.RegisterDirty(item);
        }        
        //持久化数据
        _unitOfWork.Commit();
        //更新es的数据
        await _elasticsearchClient.BulkUpdateDocumentPartial<Blog,object>(blogList);
        //删除修改完成的blog列表缓存数据
        _cacheClient.DeleteCache(MyDictionary.ChangeBlogVisits);
      }
    }
  }
}
