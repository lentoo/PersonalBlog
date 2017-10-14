
using Domain.ViewModel;
using Domain.ViewModel.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
  public interface IBlogPostCommentsService: Domain.DI.ITransientDependency
  {
    Task<PostCommentResult> PostComment(string userId, string blogId, string content);
    Task<List<CommentViewModel>> GetBlogComments(string blogId, int page = 1, int count = 1);
    /// <summary>
    /// 对某条评论进行评论
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="blogId"></param>
    /// <param name="content"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    Task<PostCommentResult> Comment(string userId, string blogId, string content, string commentId);
  }
}