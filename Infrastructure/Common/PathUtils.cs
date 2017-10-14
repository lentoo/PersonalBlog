using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Common
{
  public class PathUtils
  {
    /// <summary>
    /// 得到文件保存路径
    /// </summary>
    /// <param name="filaname"></param>
    /// <returns></returns>
    public static string GetSavePath(string filaname, bool isGuid)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder bowerPath = new StringBuilder();
      string path = string.Empty;
      if (isGuid)
      {
        path = filaname.Substring(8, 8);
      }
      else
      {
        path = path.GetHashCode().ToString();
        if (path.Length >= 8)
        {
          path = path.Substring(0, 8);
        }
      }
      stringBuilder.Append("wwwroot/UploadImages");
      bowerPath.Append("UploadImages");
      foreach (var b in path)
      {
        var str = b.ToString();
        stringBuilder.Append("/" + str);
        bowerPath.Append("/" + str);
        if (!Directory.Exists(stringBuilder.ToString()))
        {
          Directory.CreateDirectory(stringBuilder.ToString());
        }
      }
      return bowerPath.Append("/" + filaname).ToString();
    }
  }
}
