using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Domain.Event;

namespace Domain.Model
{
  [Table("BlogComment")]
  public class BlogComment : IEntity,IEvent
  {
    public string UserId { get; set; }
    public string BlogId { get; set; }
    public string Content { get; set; }
    public DateTime CommentTime { get; set; }
    public int Like { get; set; }
    public string CommentId { get; set; }
  }
}
