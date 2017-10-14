using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using Infrastructure.Common;

namespace PersonalBlog.Models.filters
{
  public class GlobalErrorFilter : ExceptionFilterAttribute
  {

    public override Task OnExceptionAsync(ExceptionContext context)
    {
      LoggerHelper.Debug(context.Exception);
      return base.OnExceptionAsync(context);
    }
  }
}
