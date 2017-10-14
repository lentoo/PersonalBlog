using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.IRepository;
using Domain.Model;
using Infrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace PersonalBlog.Controlles
{
  public class NewsController : Controller
  {
    private readonly INewsRepository _newsRepository;
    public NewsController(INewsRepository newsRepository)
    {
      _newsRepository = newsRepository;
    }

    public async Task<IActionResult> Page(int page = 1, int count = 10)
    {
      List<News> list = _newsRepository.GetPageEntitys(page, count, u => true, u => u.ReleaseTime, true);
      ViewData["CurrentPage"] = page;
      ViewData["News"] = list;
      List<int> pages = PageUtils.GetPages(await _newsRepository.GetEntitysCount(), page).ToList();
      ViewData["Pages"] = pages;
      return await Task.FromResult(View("Index"));
    }

    public async Task<IActionResult> NewsDetail(string Id)
    {
      News news = await _newsRepository.GetEntity(u => u.Id + "" == Id);
      ViewData["News"] = news;
      return View();
    }
  }
}