using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Domain.IRepository;
using System.Text;
using Domain.Event;
using Domain.Model;
using Domain.ViewModel.Result;
using Domain.UnitOfWork;

namespace PersonalBlog.Controlles
{
  public class LikeController : Controller
  {
    private readonly ILikeRecordsRepository _likeRecordsRepository;
    private readonly IBlogCommentRepository _blogCommentRepository;
    private readonly IUnitOfWork unitOfWork;
    public LikeController(ILikeRecordsRepository _likeRecordsRepository, IBlogCommentRepository _blogComment, IUnitOfWork unitOfWork)
    {
      this._likeRecordsRepository = _likeRecordsRepository;
      _blogCommentRepository = _blogComment;
      this.unitOfWork = unitOfWork;
    }
    public IActionResult Like(string commentId)
    {
      if (User.Identity.IsAuthenticated)
      {
        HttpContext.Session.TryGetValue("user", out byte[] buff);
        string userId = Encoding.UTF8.GetString(buff);
        LikeRecords likeRecord = _likeRecordsRepository.GetEntity(u => u.UserId == userId && u.CommentId == commentId).Result;
        ResultModel result = new ResultModel();
        if (likeRecord != null) //点过赞了，不能再点
        {
          result.ResultCode = Domain.ViewModel.ResultCode.No;
          result.Message = "已经点过赞了";

        }
        else                    //可以点赞
        {
          likeRecord = new LikeRecords()
          {
            Id = Guid.NewGuid().ToString(),
            CommentId = commentId,
            UserId = userId
          };
          result.ResultCode = Domain.ViewModel.ResultCode.Ok;
          result.Message = "成功点赞";
          BlogComment blogComment = _blogCommentRepository.GetEntity(u => u.Id == commentId).Result;
          unitOfWork.RegisterNew(likeRecord);
          blogComment.Like += 1;
          unitOfWork.RegisterDirty(blogComment);
          unitOfWork.Commit();

        }
        return Json(result);
      }
      return null;
    }
  }
}