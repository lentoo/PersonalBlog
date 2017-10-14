using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.IRepository;
using Domain.Model;
using Domain.UnitOfWork;

namespace Infrastructure.Common
{
  public class HtmlContentCrawling
  {
    private readonly INewsRepository _newsRepository;
    private readonly IUnitOfWork unitOfWork;

    public HtmlContentCrawling(INewsRepository newsRepository, IUnitOfWork unitOfWork)
    {
      _newsRepository = newsRepository;
      this.unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 获取博客园新闻内容
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="url"></param>
    public void GetCnbolgNews(string rootPath, string url)
    {
      try
      {
        var all = _newsRepository.GetEntitys(u => true);
        var allNewses = all.Result.ToList();
        HtmlWeb webClient = new HtmlWeb();
        HtmlDocument doc = webClient.LoadFromWebAsync(url).Result;
        var nodes = doc.DocumentNode.SelectNodes("//*[@class=\"news_block\"]/div[2]");
        if (nodes != null)
        {
          object obj = new object();
         Parallel.ForEach(nodes, node =>
         {
             News n = new News();
             var newTitle = node.SelectSingleNode("./*[@class=\"news_entry\"]/a").InnerHtml;
             var newHref = "https://news.cnblogs.com/" + node.SelectSingleNode("./*[@class=\"news_entry\"]/a")
                             .GetAttributeValue("href", null);
             var content = GetContent(newHref);
             var imgUrl = node.SelectSingleNode("./*[@class=\"entry_summary\"]/a/img").GetAttributeValue("src", null);
             var savePathResult = SaveImage(rootPath, imgUrl);
             string imgSavePath = savePathResult.Result;
             var summary = node.SelectSingleNode("./*[@class=\"entry_summary\"]").InnerText.TrimStart().TrimEnd();
             var author = node.SelectSingleNode("./*[@class=\"entry_footer\"]/a").InnerText;
             var pTime = node.SelectSingleNode("./*[@class=\"entry_footer\"]/span[@class=\"gray\"]").InnerText;
             var category = node.SelectSingleNode("./*[@class=\"entry_footer\"]/span[@class=\"tag\"]/a")?.InnerText;
             n.Title = newTitle;
             n.Content = content.Result;
             n.ImgUrl = "/" + imgSavePath;
             n.Introduction = summary;
             n.Author = author;
             n.Category = category;
             n.ReleaseTime = DateTime.Parse(pTime);
             lock (obj)
             {
               var item = allNewses.FirstOrDefault(u => u.Title.Contains(n.Title));
               if (item == null)
               {
                 unitOfWork.RegisterNew(n);
               }
             }
           });
          lock (obj)
          {
            unitOfWork.Commit();
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    /// <summary>
    /// 获去博客内容
    /// </summary>
    /// <param name="url">url</param>
    /// <returns></returns>
    public async Task<string> GetContent(string url)
    {
      HtmlWeb webClient = new HtmlWeb();
      var doc = await webClient.LoadFromWebAsync(url);
      var content = doc.DocumentNode.SelectSingleNode("//*[@id=\"news_body\"]").InnerHtml;
      return content;
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="rootPath">rootPath</param>
    /// <param name="imgUrl">图片Url</param>
    /// <returns></returns>
    public async Task<string> SaveImage(string rootPath, string imgUrl)
    {
      string fileName = Path.GetFileName(imgUrl);
      var savePath = PathUtils.GetSavePath(fileName, false);
      var imgSavePath = Path.Combine(rootPath, savePath);
      if (File.Exists(imgSavePath))
      {
        return savePath;
      }
      HttpWebResponse res = HttpUtils.CreateGetHttpResponse(imgUrl);
      var stream = res.GetResponseStream();
      try
      {
        using (FileStream fs = new FileStream(imgSavePath, FileMode.OpenOrCreate))
        {
          int offset = 0;
          byte[] buff = new byte[1024 * 100];
          int len = 0;
          while ((len = await stream.ReadAsync(buff, offset, buff.Length - offset)) != 0)
          {
            await fs.WriteAsync(buff, offset, len);
            offset += len;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }

      return savePath;
    }
  }
}