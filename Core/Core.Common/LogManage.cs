using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Common
{
    public static class LogManage
    {
        public static void Error(this ILogger _log, Exception ex)
        {
            _log.LogError(ex, "");
            //throw ex;
        }

        public static void Info(this ILogger _log, string text)
        {
            _log.LogInformation(text);
        }
        
        public static void Debug(this ILogger _log, object text)
        {
            _log.LogInformation(JsonConvert.SerializeObject(text));
        }

        public static void Warning(this ILogger _log, string text)
        {
            _log.LogWarning(text);
        }
    }
}
