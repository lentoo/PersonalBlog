using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Middleware
{
  public static class AutoMapperMiddleware
  {
    public static IApplicationBuilder UseAutoMapper(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<AutoMapperMiddlewareConfig>();
    }
  }
}