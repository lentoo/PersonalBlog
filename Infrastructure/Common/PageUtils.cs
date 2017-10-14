using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
  public class PageUtils
  {
    public static IEnumerable<int> GetPages(int entityCount, int page = 1, int count = 10)
    {
      IEnumerable<int> pages = Enumerable.Range(1, 10).ToList();
      int totalPages = Convert.ToInt32(Math.Ceiling((entityCount / (count*1.0))));//总页数
      if (totalPages <= 10)
      {
        pages = Enumerable.Range(1, totalPages);
      }
      else
      {
        if (page - 5 <= 1)
        {
          pages = Enumerable.Range(1, 10);
          return pages;
        }
        else
        {
          if (page + 4 > totalPages)
          {
            pages = Enumerable.Range(totalPages -10, 10).ToList();
          }
          else
          {
            pages = Enumerable.Range(page - 5, 10).ToList();
          }
        }
      }
      return pages;
    }
  }
}
