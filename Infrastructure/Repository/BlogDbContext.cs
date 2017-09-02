using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository
{
  public class BlogDbContext : DbContext
  {
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
      
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
    }
  }
}
