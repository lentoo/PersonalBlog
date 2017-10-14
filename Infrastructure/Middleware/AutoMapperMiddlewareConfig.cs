using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.AutoMapperConfig;

namespace Infrastructure.Middleware
{
  public class AutoMapperMiddlewareConfig
  {
    private readonly RequestDelegate _next;

    public AutoMapperMiddlewareConfig(RequestDelegate _next)
    {
      this._next = _next;
    }

    public async Task Invoke(HttpContext context)
    {
      AutoMapperContainer.Initialize();
      await _next.Invoke(context);
    }
  }
}
