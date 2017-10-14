using System;
using System.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using Domain.IRepository;
using Hangfire;
using Hangfire.MySql;
using Hangfire.MySql.Core;
using Infrastructure.DbContext;
using Infrastructure.Middleware;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PersonalBlog.Models.filters;
using PersonalBlog.Models.Jobs;
using Application.IService;
using Application.Service;

using Domain.UnitOfWork;
using Infrastructure.UnitOfWork;
using Infrastructure.AutoInjections;
using System.Reflection;
using System.Linq;
using Infrastructure.ES;
using Infrastructure.Common.Cached;

namespace PersonalBlog
{
  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      var builder = new ConfigurationBuilder();
      builder.SetBasePath(Directory.GetCurrentDirectory());
      builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
      var configuration = builder.Build();
      var mySqlConnection = configuration.GetConnectionString("MySqlConnection");
      //配置redis
      var section = configuration.GetSection("redisClientList");
      //var val = section.GetValue<string>("redisReadWriteHosts");
      services.Configure<RedisConfiguration>(section);


      #region 配置authorize
      var cookieScheme = configuration["LoginCookieName"];
      services.AddAuthentication(b =>
      {
        b.DefaultAuthenticateScheme = cookieScheme;
        b.DefaultChallengeScheme = cookieScheme;
        b.DefaultSignInScheme = cookieScheme;
      })
        .AddCookie(cookieScheme, b =>
        {
          b.LoginPath = "/Login/Index";
          b.Cookie.Name = "Login";
          b.Cookie.Path = "/";
          b.Cookie.HttpOnly = true;
          b.Cookie.Expiration = new TimeSpan(0, 20, 0);
          b.ExpireTimeSpan = new TimeSpan(0, 20, 0);
        });
      #endregion


      services.AddMvc(option =>
      {
        option.Filters.Add(new GlobalErrorFilter());
      });

      services.AddSession();
      services.AddOptions();


      #region Hangfire
      services.AddHangfire(o => o.UseStorage(new MySqlStorage(mySqlConnection, new MySqlStorageOptions()
      {
        TransactionIsolationLevel = IsolationLevel.ReadCommitted, //事务隔离级别
        QueuePollInterval = TimeSpan.FromSeconds(15),             //队列轮询间隔
        JobExpirationCheckInterval = TimeSpan.FromHours(1),       //作业到期检查间隔
        CountersAggregateInterval = TimeSpan.FromMinutes(5),      //计数器总计间隔
        PrepareSchemaIfNecessary = false,                         //必要准备模式
        DashboardJobListLimit = 50000,                            //作业列表限制
        TransactionTimeout = TimeSpan.FromMinutes(1),             //事务超时
      })));
      #endregion

      //services.AddSingleton<ICacheClient, RedisWriter>();
      services.AddScoped<VisitsRecordAttribute>();
      #region 注册容器

      services
        .AddDbContext<BlogDbContext>()
        .AddScoped<IDbContext, BlogDbContext>()
        .AddScoped<IUnitOfWork, UnitOfWork>();
      AutoInjection.LoadAssembly("Domain");
      AutoInjection.LoadAssembly("Infrastructure");
      AutoInjection.LoadAssembly("Application");
      //   types.AddRange(Assembly.GetExecutingAssembly()
      AutoInjection.Initial(services);
      //services.AddSingleton<IElasticsearchClient, ElasticsearchClient>((provider) =>
      //{
      //  return 
      //});
      //.AddTransient<IUserRepository, UserRepository>()
      //.AddTransient<IBlogRepository, BlogRepository>()
      //.AddTransient<INewsRepository, NewsRepository>()
      //.AddTransient<IBlogCommentRepository, BlogCommentRepository>()
      //.AddTransient<IBlogPostCommentsService, BlogPostCommentsService>()
      //.AddTransient<ILikeRecordsRepository, LikeRecordsRepository>()
      ;
      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      loggerFactory.AddNLog();
      loggerFactory.ConfigureNLog(env.ContentRootPath + "/nlog.config");
      app
      .UseStaticFiles()
      .UseAutoMapper()
      .UseAuthentication()
      .UseSession();

      #region Use Hangfire
      app
        .UseHangfireServer()
        .UseHangfireDashboard();
      #endregion

      //RecurringJob.AddOrUpdate(() => AutoGetNews.Enqueue(), Cron.HourInterval(2));
      RecurringJob.AddOrUpdate(() => SaveVisitsNumber.StartSave(), Cron.HourInterval(1)); //每小时执行保存博客访问数据记录
      app.UseMvc(router => router.MapRoute(
        name: "default",
        template: "{controller=Blogs}/{action=Page}/{id?}"
      ));
    }
  }
}
