using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel
{
  public class UserViewModel
  {
    [Required]
    [MinLength(4)]
    public string UserName { get; set; }
    public string NickName { get; set; }
    [Required]
    [MinLength(8)]
    public string PassWord { get; set; }
    public string Sex { get; set; }

    public string ImgUrl { get; set; }
    public string Career { get; set; }
    public string Description { get; set; }
  }
}