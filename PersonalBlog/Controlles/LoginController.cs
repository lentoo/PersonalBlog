using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.AutoMapperConfig;
using Domain.IRepository;
using Domain.Model;
using Domain.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;
using Domain.UnitOfWork;

namespace PersonalBlog.Controlles
{
  public class LoginController : Controller
  {
    private readonly IUserRepository UserRepository;
    private readonly IUnitOfWork unitOfWork;
    public LoginController(IUserRepository UserRepository, IUnitOfWork unitOfWork)
    {
      this.UserRepository = UserRepository;
      this.unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
      if (HttpContext.User.Identity.IsAuthenticated)
      {
        return Redirect("/Blogs/Page");
      }
      return View();
    }
    [HttpGet]
    public IActionResult Reg()
    {
      return View();
    }

    public async Task<IActionResult> SignIn([FromServices]IConfiguration configuration, UserViewModel userView)
    {
      
      if (ModelState.IsValid)
      {
        var user = await UserRepository.GetEntity(u => u.UserName == userView.UserName && u.Pwd == userView.PassWord);
        if (user != null)
        {
          var claim = new List<Claim>();
          claim.Add(new Claim(ClaimTypes.Name, user.NickName));
          var indentity = new ClaimsIdentity(claim, "Login");
          var principal = new ClaimsPrincipal(indentity);
          
          await HttpContext.SignInAsync(configuration["LoginCookieName"], principal);
          if (principal.Identity.IsAuthenticated)
          {
            HttpContext.Session.Set("user", Encoding.UTF8.GetBytes(user.Id));
            HttpContext.Items.Add("User", user);
            //var url = Request.Query["ReturnUrl"];
            return Json(new { code = 0, Message = "" });
          }
        }
      }
      return Json(new { code = 1, Message = "用户名密码错误" });
    }
    [Authorize]
    public async Task<IActionResult> SignOut([FromServices]IConfiguration configuration)
    {
      await HttpContext.SignOutAsync(configuration["LoginCookieName"]);
      return Redirect("/Blogs/Page");
    }
    [HttpGet]
    public async Task<IActionResult> CheckUserNameIsRegister(string userName)
    {
      Users user = await UserRepository.GetEntity(u => u.UserName == userName);
      return user == null ? Content("ok") : Content("no");
    }

    public IActionResult RegisterUser(UserViewModel userView)
    {
      if (ModelState.IsValid)
      {
        try
        {
          
          Users user = AutoMapperContainer.MapTo<Users>(userView);
          user.Id = Guid.NewGuid().ToString();
          user.Identity = "普通用户";
          //await UserRepository.AddEntity(user);
          unitOfWork.RegisterNew(user);
          unitOfWork.Commit();
          return Redirect("/Blogs/Page");
        }
        catch (Exception exception)
        {
          return Json(new { code = 1, exception.Message });
        }
      }
      return Json(new { code = 1, Message = "字段验证失败" });
    }
  }
}