using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Model
{
  [Table("Users")]
  public class Users : IEntity
  {
    public Users()
    {
      Blogs=new HashSet<Blog>();
    }
    public string UserName { get; set; }
    public string NickName { get; set; }
    public string Pwd { get; set; }
    public string Identity { get; set; }
    public string Description { get; set; }
    public string Career { get; set; }
    public string ImgUrl { get; set; }
    public string Sex { get; set; }
    public ICollection<Blog> Blogs { get; set; }
  }
}
