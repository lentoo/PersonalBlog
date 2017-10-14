using Domain.IRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Model
{
  /// <summary>
  /// 点赞记录类
  /// </summary>
  [Table("likeRecords")]
  public class LikeRecords:IEntity
  {
    public string UserId { get; set; }
    public string CommentId { get; set; }
  }
}
