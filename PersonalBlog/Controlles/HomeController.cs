using System.Text;
using System.Threading.Tasks;
using Domain.AutoMapperConfig;
using Domain.IRepository;
using Domain.Model;
using Domain.UnitOfWork;
using Domain.ViewModel;
using Domain.ViewModel.Result;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalBlog.Controlles
{
  public class HomeController : Controller
  {
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUserRepository userRepository,IUnitOfWork unitOfWork)
    {
      _userRepository = userRepository;
      _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
      Users user = await _userRepository.GetEntity(u => true);
      return View(user);
      //return await Task.FromResult(Content("Home View"));
    }
    /// <summary>
    /// 修改用户信息
    /// </summary>
    /// <param name="userViewModel"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> UpdateUser(UserViewModel userViewModel)
    {
      HttpContext.Session.TryGetValue(MyDictionary.LoginKey, out byte[] buff);
      string userId = Encoding.UTF8.GetString(buff);

      Users dbUser = await _userRepository.GetEntity(u => u.Id == userId);
      dbUser.ImgUrl = userViewModel.ImgUrl;
      dbUser.NickName = userViewModel.NickName;
      dbUser.Sex = userViewModel.Sex;
      dbUser.Career = userViewModel.Career;
      dbUser.Description = userViewModel.Description;

      //Users user = AutoMapperContainer.MapTo(userViewModel,dbUser);
      _unitOfWork.RegisterDirty(dbUser);
      bool isOk = _unitOfWork.Commit();
      var result = new ResultModel();
      if (isOk)
      {
        result.ResultCode = ResultCode.Ok;
        result.Message = "保存成功";
      }
      else
      {
        result.ResultCode = ResultCode.No;
        result.Message = "保存失败，请重试";
      }
      return Json(result);
    }
  }
}