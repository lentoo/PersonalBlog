using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Domain.ViewModel
{
  public class CommentViewModel
  {
    public CommentViewModel()
    {
      SubComments = new List<CommentViewModel>();
    }
    public string Id { get; set; }
    public string UserId { get; set; }
    public string NickName { get; set; }
    public string CommentTime { get; set; }
    public string Content { get; set; }
    public int Like { get; set; }
    public string CommentId { get; set; }
    /// <summary>
    /// 子评论
    /// </summary>
    public List<CommentViewModel> SubComments { get; set; }
  }
}