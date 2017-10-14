using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
  [Table("blogs")]
  public class Blog : IEntity
  {
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Category { get; set; }
    
    public DateTime PublishedTime { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Keywords { get; set; }
    public int VisitsNumber { get; set; }
    public virtual Users User { get; set; }
  }
}