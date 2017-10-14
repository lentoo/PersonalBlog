using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModel
{
  public class BlogView
  {
    public string Id { get; set; }
    public string UserId { get; set; }
    //public string UserImgUrl { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Category { get; set; }
    public DateTime PublishedTime { get; set; }
    public string Content { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string ImgUrl { get; set; }
    public int VisitsNumber { get; set; }
    public int CommentCount { get; set; }
  }
}
