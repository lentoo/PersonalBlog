using Infrastructure.Common;
using Infrastructure.Common.Cached;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using PersonalBlog.Models.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBlog.Models.filters
{
  public class VisitsRecordAttribute : Attribute, IAsyncActionFilter
  {
    private readonly ICacheClient _cacheClient;
    public VisitsRecordAttribute(ICacheClient cacheClient)
    {
      _cacheClient = cacheClient;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      //before
      await next();
      //after
      string blogId = context.RouteData.Values["Id"].ToString(); //获取blogId
      string userIpAddr = context.HttpContext.GetUserIp();      //获取客户端IP地址
      List<string> visitsRecords = _cacheClient.GetCache<List<string>>(userIpAddr); //获取客户端访问记录
      if (visitsRecords == null)
      {
        visitsRecords = new List<string>();
        visitsRecords.Add(blogId);
        _cacheClient.AddCache<List<string>>(userIpAddr, visitsRecords);
      }
      else if (!visitsRecords.Contains(blogId)) //未访问过该页面
      {
        visitsRecords.Add(blogId);  //添加一条访问记录
        _cacheClient.SetCache<List<string>>(userIpAddr, visitsRecords); //写入要redis缓存中
      }
      else
      {
        return;
      }

      UpdateBlogVisits(blogId);
      RecordChangeBlogVisits(blogId);
    }

    /// <summary>
    /// 修改博客訪問次數
    /// </summary>
    /// <param name="blogId"></param>
    public void UpdateBlogVisits(string blogId)
    {
      int visits = _cacheClient.GetCache<int>(blogId); //获取该blog访问次数
      if (visits == 0)
      {
        _cacheClient.AddCache<int>(blogId, 1);
      }
      else
      {
        visits += 1;    //访问次数加1
        _cacheClient.SetCache<int>(blogId, visits);
      }
    }
    /// <summary>
    /// 记录博客次数改变的博客ID
    /// </summary>
    /// <param name="blogId"></param>
    public void RecordChangeBlogVisits(string blogId)
    {
      //该list集合用来保存blog访问次数改变的blogId
      //用户后台定时保存到数据库
      HashSet<string> changeBlogVisits = _cacheClient.GetCache<HashSet<string>>(MyDictionary.ChangeBlogVisits);
      if (changeBlogVisits == null)
      {
        changeBlogVisits = new HashSet<string>();
      }
      changeBlogVisits.Add(blogId);
      _cacheClient.SetCache(MyDictionary.ChangeBlogVisits, changeBlogVisits);
    }
  }
  public static class HttpContextExtension
  {
    public static string GetUserIp(this HttpContext context)
    {
      var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
      if (string.IsNullOrEmpty(ip))
      {
        ip = context.Connection.RemoteIpAddress.ToString();
      }
      return ip;
    }
  }
}
