using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTest
{
  public class UnitTest1
  {
    [Fact]
    public void Test1()
    {
      IEnumerable<int> pages = PageUtils.GetPages(2,1);
      var list = pages.ToList();
    }
    [Fact]
    public void Test2()
    {
      PathUtils.GetSavePath("abc.jpg", false);
    }
  }
}
