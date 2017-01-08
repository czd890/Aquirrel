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

        public Tuple<bool, LogLevel> GetSwitch(string name)
        {
            var section = this._configuration.GetSection("LogLevel");
            if (section != null)
            {
                LogLevel level;
                if (Enum.TryParse(section[name], true, out level))
                    return new Tuple<bool, LogLevel>(true, level);
            }
            return new Tuple<bool, LogLevel>(false, LogLevel.Debug);
        }
        public Tuple<bool, string> GetDiretoryPath(string name)
        {
            var section = this._configuration.GetSection("Path");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return new Tuple<bool, string>(true, path);
                }
            }
            return new Tuple<bool, string>(false, this.DefaultPath);
        }
        public Tuple<bool, string> GetFileName(string name)
        {
            var section = this._configuration.GetSection("FileName");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return new Tuple<bool, string>(true, path);
                }
            }
            return new Tuple<bool, string>(false, this.DefaultFileName);
        }
        public Tuple<bool, int> GetMaxSize(string name)
        {
            var section = this._configuration.GetSection("MaxSize");
            if (section != null)
            {
                var path = section[name];
                if (!String.IsNullOrEmpty(path))
                {
                    return new Tuple<bool, int>(true, section[name].ToInt(this.DefaultMaxSize));
                }
            }
            return new Tuple<bool, int>(false, this.DefaultMaxSize);
        }
    }
}
