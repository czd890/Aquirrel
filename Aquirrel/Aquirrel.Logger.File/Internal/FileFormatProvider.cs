using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aquirrel.Logger.File.Internal
{
    public interface IFileFormatProvider
    {
        string Log<TState>(LoggerOptionsModel options, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
    }

    public class FileFormatProvider : IFileFormatProvider
    {
        public string Log<TState>(LoggerOptionsModel options, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            if (options.LogFormat.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                return BuildLogByJson(logLevel, eventId, msg, exception, options);
            }
            var log = BuildLog(logLevel, eventId, msg, exception, options);
            return log;
        }
        [ThreadStatic]
        private static StringBuilder _logBuilder;
        protected static string _messagePadding = "      ";
        protected string BuildLog(LogLevel logLevel, EventId eventId, string message, Exception ex, LoggerOptionsModel options)
        {
            var builder = _logBuilder;
            if (builder == null)
                builder = new StringBuilder();

            builder.Append(GetLogLevelString(logLevel));
            builder.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
            builder.Append("[");
            builder.Append(Thread.CurrentThread.ManagedThreadId.ToString());
            builder.Append("]");
            builder.Append("[");
            builder.Append(eventId);
            builder.Append("]");
            builder.Append("[");
            builder.Append(options.CategoryName);
            builder.Append("]");
            builder.AppendLine(message);
            if (ex != null)
            {
                builder.Append(_messagePadding);
                builder.AppendLine(ex.ToString());
            }
            GetScopeInformation(builder);
            var msg = builder.ToString();

            builder.Clear();
            if (builder.Capacity > 1024)
                builder.Capacity = 1024;
            _logBuilder = builder;

            return msg;
        }
        protected static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "[trce]";
                case LogLevel.Debug:
                    return "[dbug]";
                case LogLevel.Information:
                    return "[info]";
                case LogLevel.Warning:
                    return "[warn]";
                case LogLevel.Error:
                    return "[fail]";
                case LogLevel.Critical:
                    return "[crit]";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
        protected void GetScopeInformation(StringBuilder builder)
        {
            var current = FileLogScope.Current;
            string scopeLog = string.Empty;
            var length = builder.Length;

            while (current != null)
            {
                if (length == builder.Length)
                {
                    scopeLog = $"=> {current}";
                }
                else
                {
                    scopeLog = $"=> {current} ";
                }

                builder.Insert(length, scopeLog);
                current = current.Parent;
            }
            if (builder.Length > length)
            {
                builder.Insert(length, _messagePadding);
                builder.AppendLine();
            }
        }

        protected string BuildLogByJson(LogLevel logLevel, EventId eventId, string message, Exception ex, LoggerOptionsModel options)
        {

            var builder = _logBuilder;
            if (builder == null)
                builder = new StringBuilder();

            GetScopeInformation(builder);

            var info = new {
                logLevel=logLevel.ToInt(),
                logLevelDesc=GetLogLevelString(logLevel),
                timestamp= DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"),
                threadId= Thread.CurrentThread.ManagedThreadId.ToString(),
                eventId=eventId,
                categoryName=options.CategoryName,
                message=message,
                exception=ex?.ToString(),
                scope= builder.ToString()
            };


            return info.ToJson();
        }

    }
}
