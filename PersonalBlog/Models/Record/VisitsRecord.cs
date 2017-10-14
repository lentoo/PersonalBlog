using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBlog.Models.Record
{
  public class VisitsRecord
  {
    public VisitsRecord()
    {
      VisitsRecords = new Dictionary<string, List<string>>();
    }
    public Dictionary<string, List<string>> VisitsRecords { get;}
  }
}
