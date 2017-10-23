using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Aquirrel.Logger.File
{
    public class FileLoggerSettings
    {
        IConfiguration _configuration;
        public IChangeToken ChangeToken { get; private set; }
        public FileLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            this.ChangeToken = _configuration.GetReloadToken();
        }
        public string DefaultPath
        {
            get
            {
                return this._configuration["DefaultPath"] ?? "log";
            }
        }
        public bool IncludeScopes
        {
            get
            {
                bool b;
                if (!bool.TryParse(this._configuration["IncludeScopes"], out b))
                    b = true;
                return b;
            }
        }
        public int DefaultMaxSize
        {
            get
            {
                return this._configuration["DefaultMaxSize"].ToInt(100);
            }
        }
        public string DefaultFileName
        {
            get { return this._configuration["DefaultFileName"] ?? "[yyyyMMdd]"; }
        }

        public string LogFormat => this._configuration["LogFormat"] ?? "";

        public (bool isMatch, LogLevel logLevel) GetMinLevel(string name)
        {
            var fileSection = this._configuration.GetSection("File");
            if (fileSection != null)
            {
                var logLevelSection = fileSection.GetSection("LogLevel");
                if (logLevelSection != null)
                {
                    var level = logLevelSection[name];
                    if (level.IsNotNullOrEmpty() && Enum.TryParse(level, true, out LogLevel lev))
                        return (true, lev);
                    else if ((level = logLevelSection["Default"]).IsNotNullOrEmpty() && Enum.TryParse(level, true, out LogLevel lev2))
                        return (false, lev2);
                }
            }

            var section = this._configuration.GetSection("LogLevel");
            if (section != null)
            {
                LogLevel level;
                if (Enum.TryParse(section[name], true, out level))
                    return (true, level);
            }

            return (false, LogLevel.Information);
        }
        public (bool isMatch, string path) GetDiretoryPath(string name)
        {
            var config = this._configuration.GetSection("Config");
            if (config != null)
            {
                var sec = config.GetSection(name);
                if (sec != null && sec["Path"].IsNotNullOrEmpty())
                    return (true, sec["Path"]);
            }

            var section = this._configuration.GetSection("Path");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return (true, path);
                }
            }
            return (false, this.DefaultPath);
        }
        public (bool isMatch, string fileName) GetFileName(string name)
        {
            var config = this._configuration.GetSection("Config");
            if (config != null)
            {
                var sec = config.GetSection(name);
                if (sec != null && sec["FileName"].IsNotNullOrEmpty())
                    return (true, sec["FileName"]);
            }

            var section = this._configuration.GetSection("FileName");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return (true, path);
                }
            }
            return (false, this.DefaultFileName);
        }
        public (bool isMatch, int maxSize) GetMaxSize(string name)
        {
            var config = this._configuration.GetSection("Config");
            if (config != null)
            {
                var sec = config.GetSection(name);
                if (sec != null && sec["MaxSize"].IsNotNullOrEmpty())
                    return (true, sec["MaxSize"].ToInt(this.DefaultMaxSize));
            }


            var section = this._configuration.GetSection("MaxSize");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return (true, section[name].ToInt(this.DefaultMaxSize));
                }
            }
            return (false, this.DefaultMaxSize);
        }

        public (bool isMatch, string format) GetLogFormat(string name)
        {
            var config = this._configuration.GetSection("Config");
            if (config != null)
            {
                var sec = config.GetSection(name);
                if (sec != null && sec["LogFormat"].IsNotNullOrEmpty())
                    return (true, sec["LogFormat"]);
            }

            return (false, this.LogFormat);
        }
    }
}
