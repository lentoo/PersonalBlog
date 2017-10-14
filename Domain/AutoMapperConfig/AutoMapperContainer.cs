using System;
using AutoMapper;
using Domain.Model;
using Domain.ViewModel;

namespace Domain.AutoMapperConfig
{
  public class AutoMapperContainer
  {
    /// <summary>
    /// 初始化映射关系
    /// </summary>
    public static void Initialize()
    {
      Mapper.Initialize(cfg =>
      {
        cfg.CreateMap<Users, UserViewModel>();
        cfg.CreateMap<UserViewModel, Users>();
        cfg.CreateMap<Blog, BlogView>();
        cfg.CreateMap<BlogView, Blog>();
        cfg.CreateMap<BlogComment, CommentViewModel>();
        cfg.CreateMap<CommentViewModel, Users>();
        cfg.CreateMap<CommentViewModel, BlogComment>();
        var userViewMap = cfg.CreateMap<UserViewModel, Users>();
        userViewMap.ForMember(v => v.Pwd, ops => ops.MapFrom(m => m.UserName));
      });
    }

    /// <summary>
    /// 合并两个对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="t"></param>
    /// <param name="t1"></param>
    /// <returns>T1</returns>
    public static T1 Map<T, T1>(T t, T1 t1)
    {
      return Mapper.Map(t, t1);
    }

    public static T MapTo<T>(object obj)
    {
      return Mapper.Map<T>(obj);
    }
    public static TResult MapTo<TSource,TResult>(TSource  self, TResult result)
    {
      if (self == null)
        throw new ArgumentNullException();
      return (TResult)Mapper.Map(self, result, self.GetType(), typeof(TResult));
    }
  }
}