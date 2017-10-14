using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Domain.ViewModel
{
  public enum ResultCode
  {
    /// <summary>
    /// 正常
    /// </summary>
    Ok = 0,
    /// <summary>
    /// 异常
    /// </summary>
    No = 1,
    /// <summary>
    /// 重定向
    /// </summary>
    Redirect = 2 
  }
}
