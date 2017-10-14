using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.AutoMapperConfig;
using Domain.IRepository;
using Domain.Model;
using Domain.ViewModel;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Application.IService;
using Domain.ViewModel.Result;
using System.Text;
using Domain.UnitOfWork;
using Infrastructure.ES;
using Microsoft.AspNetCore.Http;
using PersonalBlog.Models.filters;

namespace PersonalBlog.Controlles
{
  public class BlogsController : Controller
  {
    private readonly IUserRepository _userRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly IBlogPostCommentsService _blogPostCommentsService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IElasticsearchClient _elasticsearchClient;
    private readonly IBlogSearchService _blogSearchService;
    public BlogsController(IUserRepository userRepository,
      IBlogRepository blogRepository,
      IBlogPostCommentsService blogPostCommentsService,
      IUnitOfWork unitOfWork,
      IElasticsearchClient elasticsearchClient,
      IBlogSearchService blogSearchService
      )
    {
      _userRepository = userRepository;
      _blogRepository = blogRepository;
      _blogPostCommentsService = blogPostCommentsService;
      _unitOfWork = unitOfWork;
      _elasticsearchClient = elasticsearchClient;
      _blogSearchService = blogSearchService;
    }

    public async Task<IActionResult> Page(int page = 1, int count = 10)
    {
      //Users user = await _userRepository.GetEntity(u => u.UserName == "lentoo");
      //ViewData["User"] = user;
      var bs = await _blogRepository.GetPageEntitys(null);
      BlogView[] blogViews = bs;
      //var bs = _blogRepository.GetPageEntitys(page, count, u => true, u => u.PublishedTime, true);
      //Blog[] blogs = bs.ToArray();
      ViewData["Blogs"] = blogViews;
      List<int> pages = PageUtils.GetPages(await _blogRepository.GetEntitysCount(), page).ToList();
      ViewData["Pages"] = pages;
      ViewData["CurrentPage"] = page;

      return View("Index");
    }

    /// <summary>
    /// 博客详情
    /// </summary>
    /// <param name="Id">博客id</param>
    /// <returns></returns>
    [ServiceFilter(typeof(VisitsRecordAttribute))]
    public async Task<IActionResult> BlogDetail(string Id)
    {

      var blog = await _blogRepository.GetEntity(u => u.Id == Id);
      var nextBlog = await _blogRepository.GetEntity(b => b.UserId == blog.UserId && b.PublishedTime > blog.PublishedTime);
      var upBlog = await _blogRepository.GetEntity(b => b.UserId == blog.UserId && b.PublishedTime < blog.PublishedTime);
      var comments = await _blogPostCommentsService.GetBlogComments(Id);
      ViewData["Blog"] = blog;
      ViewData["Comments"] = comments;
      ViewData["NextBlog"] = nextBlog;
      ViewData["UpBlog"] = upBlog;
      if (!User.Identity.IsAuthenticated) return View();

      HttpContext.Session.TryGetValue(MyDictionary.LoginKey, out byte[] buff);
      if (buff == null)
      {
        string returnUrl = Request.Path;
        return Redirect("/Login/Index?returnUrl=" + returnUrl);
      }
      string userId = Encoding.UTF8.GetString(buff);
      ViewData["User"] = await _userRepository.GetEntity(u => u.Id == userId);
      return View();
    }
    /// <summary>
    /// 获取博客评论
    /// </summary>
    /// <param name="blogId">博客id</param>
    /// <param name="page"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<IActionResult> GetBlogComment(string blogId, int page = 1, int count = 10)
    {
      var comments = await _blogPostCommentsService.GetBlogComments(blogId, page, count);
      return Json(new { comments });
    }
    /// <summary>
    /// 跳转发布博客
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> PublishedBlog()
    {
      return await Task.FromResult(View());
    }
    /// <summary>
    /// 图片上传
    /// </summary>
    /// <param name="env">托管环境</param>
    /// <returns></returns>
    public async Task<IActionResult> UploadImage([FromServices]IHostingEnvironment env)
    {
      UploadImage up = new UploadImage();
      var form = Request.Form;
      var img = form.Files[0];
      string fileName = img.FileName;
      var openReadStream = img.OpenReadStream();
      byte[] buff = new byte[openReadStream.Length];
      await openReadStream.ReadAsync(buff, 0, buff.Length);
      string filenameGuid = Guid.NewGuid().ToString();
      var bowerPath = PathUtils.GetSavePath(filenameGuid, true) + ".jpg";
      var savePath = Path.Combine(env.WebRootPath, bowerPath);
      using (FileStream fs = new FileStream(savePath, FileMode.Create))
      {
        await fs.WriteAsync(buff, 0, buff.Length);
        up.errno = 0;
        up.data = new[] { "/" + bowerPath };
        return Json(up);
      }
    }
    /// <summary>
    /// 图片上传  Ckeditor使用
    /// </summary>
    /// <param name="env">托管环境</param>
    /// <returns></returns>
    public async Task<IActionResult> UploadImageUrl([FromServices]IHostingEnvironment env)
    {

      // CKEditor提交的很重要的一个参数  
      string callback = Request.Query["CKEditorFuncNum"];
      UploadImage up = new UploadImage();
      var form = Request.Form;
      var img = form.Files[0];
      string fileName = img.FileName;
      var openReadStream = img.OpenReadStream();
      byte[] buff = new byte[openReadStream.Length];
      await openReadStream.ReadAsync(buff, 0, buff.Length);
      string filenameGuid = Guid.NewGuid().ToString();
      var bowerPath = PathUtils.GetSavePath(filenameGuid, true) + ".jpg";
      var savePath = Path.Combine(env.WebRootPath, bowerPath);
      using (FileStream fs = new FileStream(savePath, FileMode.Create))
      {
        await fs.WriteAsync(buff, 0, buff.Length);
        string result = $"<script type=\"text/javascript\">window.parent.CKEDITOR.tools.callFunction(\"{callback}\", \"{"/" + bowerPath}\", \"\");</script>";
        Response.ContentType = "text/html;charset=UTF-8";
        return Content(result);
      }
    }

    /// <summary>
    /// 添加博客
    /// </summary>
    /// <param name="blog"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddBlog(BlogView blog)
    {
      string userName = User.Identity.Name;
      var user = await _userRepository.GetEntity(u => u.UserName == userName);
      Blog blogModel = AutoMapperContainer.MapTo<Blog>(blog);
      var content = blogModel.Content.CleanHtml();
      var contentLen = content.Length;
      blogModel.Description = contentLen > 199 ? content.Substring(0, 199) + "..." : content;
      blogModel.UserId = user.Id;
      blogModel.PublishedTime = DateTime.Now;
      blogModel.Author = user.NickName;
      _unitOfWork.RegisterNew(blogModel);
      _unitOfWork.Commit();
      await _elasticsearchClient.CreateDocument<Blog>(blogModel);
      return Redirect("/Blogs/Page");
    }

    /// <summary>
    /// 添加博客内容到Elasticsearch中
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> TestDocument()
    {
      var blogs = await _blogRepository.GetEntitys(u => true);
      var response = await _elasticsearchClient.Bulk(blogs);
      return Content(response.IsValid.ToString());
    }
    /// <summary>
    /// 发表博客评论
    /// </summary>
    /// <param name="blogId">博客id</param>
    /// <param name="content">评论内容</param>
    /// <param name="commentId">评论ID</param>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> PostComment(string blogId, string content)
    {

      var startTime = DateTime.Now;
      HttpContext.Session.TryGetValue(MyDictionary.LoginKey, out byte[] buff);
      if (buff == null)
      {
        return Json(new { resultCode = ResultCode.Redirect, returnUrl = Request.Path });
      }
      string userId = Encoding.UTF8.GetString(buff);
      PostCommentResult result = await _blogPostCommentsService.PostComment(userId, blogId, content);
      var endTime = DateTime.Now;
      var duration = endTime - startTime;
      result.Duration = duration.TotalMilliseconds;
      if (result.ResultCode == ResultCode.Ok)
      {
        return Json(result);
      }
      else
      {
        return Json(result);
      }
    }
    /// <summary>
    /// 对某人的评论内容进行评论
    /// </summary>
    /// <param name="blogId">博客id</param>
    /// <param name="commentId">评论id</param>
    /// <param name="content">评论内容</param>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> Comment(string blogId, string commentId, string content)
    {

      HttpContext.Session.TryGetValue(MyDictionary.LoginKey, out byte[] buff);
      if (buff == null)
      {
        return Json(new { resultCode = ResultCode.Redirect, returnUrl = Request.Path });
      }
      string userId = Encoding.UTF8.GetString(buff);
      PostCommentResult result = await _blogPostCommentsService.Comment(userId, blogId, content, commentId);
      if (result.ResultCode == ResultCode.Ok)
      {
        return Json(result);
      }
      else
      {
        return Json(result);
      }
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="s">关键字</param>
    /// <param name="page">当前页</param>
    /// <returns></returns>
    public async Task<IActionResult> Search(string s, int page = 1)
    {
      var result = await _blogSearchService.SearchBlogs(s);
      var blogviews = result.Item1;
      int count = result.Item2;
      ViewData["CurrentKeyWord"] = s;
      ViewData["Blogs"] = blogviews;
      ViewData["CurrentPage"] = page;
      List<int> pages = PageUtils.GetPages(count).ToList();
      ViewData["pages"] = pages;
      return View();
    }

    public async Task<IActionResult> ReadRankings()
    {
      List<Blog> blogs = _blogRepository.GetPageEntitys(1, 10, blog => true, blog => blog.VisitsNumber, true);
      var hotBlogs = blogs.Select(b => new { b.Id, b.Title }).ToArray();        
      return await Task.FromResult(Json(hotBlogs));
    }
  }
}