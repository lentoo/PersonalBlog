using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Model
{
  [Table("news")]
  public class News 
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Introduction { get; set; }
    public string ImgUrl { get; set; }
    public string Category { get; set; }
    public DateTime ReleaseTime { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
  }
}
