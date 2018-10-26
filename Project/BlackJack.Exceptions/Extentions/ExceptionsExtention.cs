using System;
using System.Diagnostics;
using System.Text;

namespace BlackJack.Exceptions.Extentions
{
    public static class ExceptionsExtention
    {
        public static string ConvertMessageFromStackTrace(this Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);

            var frame = stackTrace.GetFrame(0);

            var traceString = new StringBuilder();

            traceString.AppendLine(string.Format("{0}", exception.Message));
            traceString.AppendLine(string.Format("File: {0}", frame.GetFileName()));
            traceString.AppendLine(string.Format("Method: {0}", frame.GetMethod().ReflectedType.Name));
            traceString.AppendLine(string.Format("LineNumber: {0}", frame.GetFileLineNumber()));

            return traceString.ToString();
        }
    }
}