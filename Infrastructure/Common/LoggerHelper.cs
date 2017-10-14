using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common
{
  public class LoggerHelper
  {
    public static Logger logger = LogManager.GetCurrentClassLogger();
    public static void Debug(Exception exception)
    {
      logger.Debug(exception);
    }
  }
}
