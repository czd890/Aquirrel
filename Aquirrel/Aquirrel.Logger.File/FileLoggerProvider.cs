using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Logger.File.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Aquirrel.Logger.File
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider, IDisposable
    {
        //Microsoft.Extensions.Logging.ILoggerProvider
        FileLoggerSettings _configuration;
        readonly ConcurrentDictionary<string, LoggerOptionsModel> _loggerOptionsCache = new ConcurrentDictionary<string, LoggerOptionsModel>();
        readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();

        IFileFormatProvider fileFormatProvider;
        public FileLoggerProvider(FileLoggerSettings configuration, IFileFormatProvider fileFormatProvider)
        {
            this.fileFormatProvider = fileFormatProvider;
            _configuration = configuration;
            _configuration.ChangeToken.RegisterChangeCallback(p =>
            {
                //update loggers settings form new settings
                foreach (var item in this._loggers.Values)
                {
                    LoggerOptionsModel model = new LoggerOptionsModel();
                    InitLoggerSettings(item.Name, model);
                    InitLogger(model, item);
                }

            }, null);
        }
        public ILogger CreateLogger(string categoryName)
        {
            var option = this._loggerOptionsCache.GetOrAdd(categoryName, p =>
             {
                 LoggerOptionsModel model = new LoggerOptionsModel();
                 InitLoggerSettings(categoryName, model);
                 return model;
             });
            return this._loggers.GetOrAdd(categoryName, p =>
            {
                var logger = new FileLogger(categoryName, option, this.fileFormatProvider);
                InitLogger(option, logger);
                return logger;
            });
        }

        private static void InitLogger(LoggerOptionsModel model, FileLogger logger)
        {
            logger.Options = model;
        }


        private void InitLoggerSettings(string categoryName, LoggerOptionsModel model)
        {
            model.CategoryName = categoryName;
            model.IncludeScopes = this._configuration.IncludeScopes;

            var keys = this.GetKeys(categoryName);

            model.MinLevel = LogLevel.Information;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetMinLevel(item);
                if (switchV.isMatch)
                {
                    model.MinLevel = switchV.logLevel;
                    break;
                }
            }
            model.FileDiretoryPath = this._configuration.DefaultPath;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetDiretoryPath(item);
                if (switchV.isMatch)
                {
                    model.FileDiretoryPath = switchV.path;
                    break;
                }
            }
            model.FileNameTemplate = this._configuration.DefaultFileName;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetFileName(item);
                if (switchV.isMatch)
                {
                    model.FileNameTemplate = switchV.fileName;
                    break;
                }
            }
            model.MaxSize_Bytes = this._configuration.DefaultMaxSize;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetMaxSize(item);
                if (switchV.isMatch)
                {
                    model.MaxSize_Bytes = switchV.maxSize;
                    break;
                }
            }
            model.MaxSize_Bytes = model.MaxSize_Bytes * 1024 * 1024;

            model.LogFormat = this._configuration.LogFormat;
            foreach (var item in keys)
            {
                var switchV = _configuration.GetLogFormat(item);
                if (switchV.isMatch)
                {
                    model.LogFormat = switchV.format;
                    break;
                }
            }
        }

        IEnumerable<string> GetKeys(string categoryName)
        {
            while (!String.IsNullOrEmpty(categoryName))
            {
                // a.b.c
                //--result
                // a.b.c，a.b，a，Default
                yield return categoryName;
                var last = categoryName.LastIndexOf('.');
                if (last <= 0)
                {
                    yield return "Default";
                    yield break;
                }
                categoryName = categoryName.Substring(0, last);
            }
            yield break;

        }
        public void Dispose()
        {
            _loggerOptionsCache.Clear();
            _loggers.Clear();
        }
    }
}
