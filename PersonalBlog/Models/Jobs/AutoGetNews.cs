using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Domain.IRepository;
using Hangfire;
using Infrastructure.Common;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.UnitOfWork;

namespace PersonalBlog.Models.Jobs
{
  public class AutoGetNews
  {
    private readonly INewsRepository _newsRepository;
    private readonly IHostingEnvironment dev;
    private readonly IUnitOfWork unitOfWork;
    public AutoGetNews(INewsRepository newsRepository,IUnitOfWork unitOfWork, IHostingEnvironment dev)
    {
      _newsRepository = newsRepository;
      this.unitOfWork = unitOfWork;
      this.dev = dev;
    }
    public void GetNews()
    {
      HtmlContentCrawling htmlContentCrawling=new HtmlContentCrawling(_newsRepository, unitOfWork);
      htmlContentCrawling.GetCnbolgNews(dev.WebRootPath, "https://news.cnblogs.com/n/page/1");
    }

    public static void Enqueue()
    {
      BackgroundJob.Enqueue<AutoGetNews>(i => i.GetNews());
    }
    
  }
}