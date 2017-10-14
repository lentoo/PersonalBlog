using Application.IService;
using Domain.IRepository;
using Domain.Model;
using Domain.ViewModel;
using Domain.ViewModel.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Domain.UnitOfWork;
using Domain.AutoMapperConfig;

namespace Application.Service
{
  public class BlogPostCommentsService : IBlogPostCommentsService, Domain.DI.ITransientDependency
  {
    private readonly IBlogRepository _blogRepository;
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    public BlogPostCommentsService(IBlogRepository _blogRepository, IBlogCommentRepository blogCommentRepository, IUserRepository _userRepository, IUnitOfWork unitOfWork)
    {
      this._blogRepository = _blogRepository;
      this._blogCommentRepository = blogCommentRepository;
      this._userRepository = _userRepository;
      this._unitOfWork = unitOfWork;
    }
    public async Task<PostCommentResult> PostComment(string userId, string blogId, string content)
    {
      PostCommentResult postCommentResult = new PostCommentResult();
      var user = await _userRepository.GetEntity(u => u.Id == userId);
      var blog = await _blogRepository.GetEntity(u => u.Id == blogId);
      if (user == null || blog == null)
      {
        postCommentResult.ResultCode = ResultCode.No;
        postCommentResult.Message = "用户或博客不存在";
        return postCommentResult;
      }
      BlogComment blogComment = new BlogComment();
      blogComment.Id = Guid.NewGuid().ToString();
      blogComment.UserId = userId;
      blogComment.BlogId = blogId;
      blogComment.Content = content;
      blogComment.CommentTime = DateTime.Now;
      _unitOfWork.RegisterNew(blogComment);
      postCommentResult.ResultCode = ResultCode.Ok;
      CommentViewModel commentViewModel = new CommentViewModel();
      commentViewModel.Content = content;
      commentViewModel.CommentTime = blogComment.CommentTime.ToString("yyyy.MM.dd HH:mm:ss");
      commentViewModel.NickName = user.NickName;
      commentViewModel.Id = blogComment.Id;
      postCommentResult.Comment = commentViewModel;
      _unitOfWork.Commit();
      return postCommentResult;
    }
    /// <summary>
    /// 对某条评论进行评论
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="blogId"></param>
    /// <param name="content"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    public async Task<PostCommentResult> Comment(string userId, string blogId, string content, string commentId)
    {
      PostCommentResult postCommentResult = new PostCommentResult();
      var user = await _userRepository.GetEntity(u => u.Id == userId);
      var blog = await _blogRepository.GetEntity(u => u.Id == blogId);
      if (user == null || blog == null)
      {
        postCommentResult.ResultCode = ResultCode.No;
        postCommentResult.Message = "用户或博客不存在";
        return postCommentResult;
      }
      BlogComment blogComment = new BlogComment();
      blogComment.Id = Guid.NewGuid().ToString();
      blogComment.UserId = userId;
      blogComment.BlogId = blogId;
      blogComment.Content = content;
      blogComment.CommentTime = DateTime.Now;
      blogComment.CommentId = commentId;
      _unitOfWork.RegisterNew(blogComment);
      postCommentResult.ResultCode = ResultCode.Ok;
      CommentViewModel commentViewModel = new CommentViewModel();
      commentViewModel.UserId = userId;
      commentViewModel.CommentId = commentId;
      commentViewModel.Content = content;
      commentViewModel.Content = content;
      commentViewModel.CommentTime = blogComment.CommentTime.ToString("yyyy.MM.dd HH:mm:ss");
      commentViewModel.NickName = user.NickName;
      commentViewModel.Id = blogComment.Id;
      postCommentResult.Comment = commentViewModel;
      _unitOfWork.Commit();
      return postCommentResult;
    }

    /// <summary>
    /// 获取博客评论
    /// </summary>
    /// <param name="blogId"></param>
    /// <param name="page"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<CommentViewModel>> GetBlogComments(string blogId, int page = 1, int count = 1)
    {
      var blogComments = (await _blogCommentRepository.GetEntitys(u => u.BlogId == blogId && u.CommentId == null)).OrderBy(u=>u.CommentTime); //获取一级评论
      var users = await _userRepository.GetEntitys(u => true);
      var data = from blogComment in blogComments
                 join user in users on blogComment.UserId equals user.Id
                 select new CommentViewModel()
                 {
                   Id = blogComment.Id,
                   CommentTime = blogComment.CommentTime.ToString("yyyy.MM.dd HH:mm:ss"),
                   Content = blogComment.Content,
                   NickName = user.NickName,
                   Like = blogComment.Like
                 };
      var commentList = data.ToList();
      var blogComments2 = (await _blogCommentRepository.GetEntitys(u => u.BlogId == blogId && u.CommentId != null)).OrderBy(u=>u.CommentTime); //获取二级评论
      var blogComments2List = (from blogComment in blogComments2
                               join user in users on blogComment.UserId equals user.Id
                               select new CommentViewModel()
                               {
                                 Id = blogComment.Id,
                                 UserId=user.Id,
                                 CommentTime = blogComment.CommentTime.ToString("yyyy.MM.dd HH:mm:ss"),
                                 Content = blogComment.Content,
                                 NickName = user.NickName,
                                 Like = blogComment.Like,
                                 CommentId = blogComment.CommentId
                               }).ToList();      
      commentList.ForEach(item =>
      {
        item.SubComments = blogComments2List.Where(a => a.CommentId == item.Id)
        //.Where(a => blogComments2List.Any(blog => blog.CommentId == a.Id))
        .ToList();
        //commentList.Where(a => item.Id == a.Id).Where(bc => blogComments2.Any(bc2 => bc2.CommentId == bc.Id)).ToList();
      });

      return commentList;
    }
  }
}
