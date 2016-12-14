using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading;
using Aquirrel.Logger.File.Internal;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Aquirrel.Logger.File
{
    public class FileLogger : ILogger
    {
        public static IFileFormatProvider FileFormatProvider { get; set; } = new FileFormatProvider();


        public FileLogger(string categoryName, LoggerOptionsModel options)
        {
            this.Name = categoryName;
            this.Options = options;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return FileLogScope.Push(this.Name, state);
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.Options.MinLevel <= logLevel;
        }
        public string Name { get; private set; }
        public LoggerOptionsModel Options { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;
            var log = FileFormatProvider.Log(this.Options, logLevel, eventId, state, exception, formatter);
            FileLogWrite.AddToQueue(this.Options, log);
        }
    }




}
