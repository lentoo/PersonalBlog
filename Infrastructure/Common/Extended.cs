using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Common
{
  public static class Extended
  {
    public static string GetMd5(this string str)
    {
      if (str == null) throw new ArgumentNullException(nameof(str));
      var md5 = MD5.Create();
      var buff = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
      StringBuilder stringBuilder=new StringBuilder();
      foreach (var b in buff)
      {
        stringBuilder.Append(b.ToString("x2"));
      }
      return stringBuilder.ToString();
    }
    /// <summary>
    /// 清除Html标签
    /// </summary>
    /// <param name="strHtml"></param>
    /// <returns></returns>
    public static string CleanHtml(this string strHtml)
    {
      if (string.IsNullOrEmpty(strHtml)) return strHtml;
      //删除脚本
      //Regex.Replace(strHtml, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase)
      strHtml = Regex.Replace(strHtml, @"(\<script(.+?)\</script\>)|(\<style(.+?)\</style\>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
      //删除标签
      var r = new Regex(@"</?[^>]*>", RegexOptions.IgnoreCase);
      Match m;
      for (m = r.Match(strHtml); m.Success; m = m.NextMatch())
      {
        strHtml = strHtml.Replace(m.Groups[0].ToString(), "");
      }
      return strHtml.Trim();
    }

   
  }
}