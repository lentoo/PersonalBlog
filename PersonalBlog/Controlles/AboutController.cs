using System.Text;
using System.Threading.Tasks;
using Domain.AutoMapperConfig;
using Domain.IRepository;
using Domain.ViewModel;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace PersonalBlog.Controlles
{
  public class AboutController : Controller
  {
    private readonly IUserRepository _userRepository;
    private readonly IBlogRepository _blogRepository;
    public AboutController(IUserRepository userRepository, IBlogRepository blogRepository)
    {
      _userRepository = userRepository;
      _blogRepository = blogRepository;

    }
    [Authorize]
    public async Task<IActionResult> Index([FromServices]IConfiguration configuration, int page = 1, int count = 10)
    {
      HttpContext.Session.TryGetValue(MyDictionary.LoginKey, out byte[] buff);
      
      if (buff == null)
      {
        string returnUrl = Request.Path.Value;
        await HttpContext.SignOutAsync(configuration["LoginCookieName"]);
        return Redirect("/Login/Index?returnUrl=" + returnUrl);
      }
      string userId = Encoding.UTF8.GetString(buff);
      var user = await _userRepository.GetEntity(u => u.Id == userId);
      var userViewModel = AutoMapperContainer.MapTo<UserViewModel>(user);
      var bs= await _blogRepository.GetPageEntitys(b => b.UserId == userId);
      //var blogs = _blogRepository.GetPageEntitys(page, count, u => u.UserId == userId, o => o.PublishedTime, false);
      //var blogViews=AutoMapperContainer.MapTo<List<BlogView>>(blogs);
      ViewData["blogs"] = bs;
      var pages = PageUtils.GetPages(await _blogRepository.GetEntitysCount()).ToList();
      ViewBag.pages = pages;
      ViewData["CurrentPage"] = page;
      return View(userViewModel);
    }
  }
}