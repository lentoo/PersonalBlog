using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
  public class BlogDbContext : Microsoft.EntityFrameworkCore.DbContext,IDbContext,Domain.DI.IScopedDependency
  {
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options){}
    public virtual DbSet<Users> User { get; set; }
    public virtual DbSet<Blog> Blogs { get; set; }
    public virtual DbSet<News> News { get; set; }
    
    public virtual DbSet<BlogComment> BlogComment { get; set; }
    public virtual DbSet<LikeRecords> LikeRecords { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
      optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=PersonalBlog;uid=root;pwd=root;SslMode=None");
      
    }
  }
}
