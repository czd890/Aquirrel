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

namespace Aquirrel.Logger.File
{
    /// <summary>
    /// 文件日志logger
    /// </summary>
    public class FileLogger : ILogger
    {
        IFileFormatProvider fileFormatProvider;

        public FileLogger(string categoryName, LoggerOptionsModel options, IFileFormatProvider fileFormatProvider)
        {
            this.Name = categoryName;
            this.Options = options;
            this.fileFormatProvider = fileFormatProvider;
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
        public IFileFormatProvider FileFormatProvider { get => fileFormatProvider; set => fileFormatProvider = value; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;
            var log = this.fileFormatProvider.Log(this.Options, logLevel, eventId, state, exception, formatter);
            FileLogWrite.AddToQueue(this.Options, log);
        }
    }




}
