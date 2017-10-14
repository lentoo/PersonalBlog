using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModel.Result
{
  public class PostCommentResult:ResultModel
  {   
    public CommentViewModel Comment { get; set; }
    public double Duration { get; set; }
  }
}
