using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel.Logger.File.Internal
{
    public interface IFileLogWrite
    {
        void Write(params string[] logs);
    }

    public class FileLogWrite : IFileLogWrite, IDisposable
    {
        public static void AddWriteProvider(LoggerOptionsModel options, Func<LoggerOptionsModel, IFileLogWrite> factory)
        {
            AddProvider(options, factory);
        }
        static Dictionary<string, IFileLogWrite> _cache = new Dictionary<string, IFileLogWrite>();


        protected LoggerOptionsModel _options;
        public FileLogWrite(LoggerOptionsModel options)
        {
            _options = options;
        }

        internal static void AddToQueue(LoggerOptionsModel options, params string[] messages)
        {
            string key = AddProvider(options, op => new FileLogWrite(op));
            _cache[key].Write(messages);
        }

        private static string AddProvider(LoggerOptionsModel options, Func<LoggerOptionsModel, IFileLogWrite> factory)
        {
            var key = options.FileDiretoryPath + options.FileNameTemplate;
            if (!_cache.ContainsKey(key))
            {
                lock (_cache)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache[key] = factory(options);
                    }
                }
            }

            return key;
        }

        object workAsync = new object();
        public void Write(params string[] logs)
        {
            lock (workAsync)
            {
                workAdd.AddRange(logs);
                if (!_running)
                {
                    _running = true;
                    Task.Factory.StartNew(SaveToFile);
                }
            }
        }
        void SaveToFile()
        {

            while (true)
            {
                lock (workAsync)
                {
                    var data = workAdd;
                    workAdd = workRunning;
                    workRunning = data;
                    if (workRunning.Count == 0)
                    {
                        _running = false;
                        break;
                    }
                }
                WriteRunningQueueLog();
            }
        }

        List<string> workAdd = new List<string>();
        List<string> workRunning = new List<string>();
        bool _running = false;
        void WriteRunningQueueLog()
        {
            try
            {
                EnsureInitFile();

                foreach (var item in workRunning)
                {
                    var msg = item + Environment.NewLine;
                    var _s = Encoding.UTF8.GetByteCount(item);
                    _size += _s;
                    if (_size > this._options.MaxSize_Bytes)
                    {
                        _size = _s;
                        reNewFile = true;
                        EnsureInitFile();
                        reNewFile = false;
                    }
                    this._sw.Write(item);
                }
                workRunning.Clear();
            }
            catch
            {
            }
        }

        string _FileName;
        int _size = 0;
        bool reNewFile = false;
        protected StreamWriter _sw;
        void EnsureInitFile()
        {
            if (CheckNeedCreateNewFile())
                InitFile();
        }
        bool CheckNeedCreateNewFile()
        {
            if (reNewFile)
                return true;
            //TODO 使用 RollingType判断是否需要创建文件。提高效率！！！
            if (_FileName != DateTime.Now.ToString(_options.FileNameTemplate))
            {
                return true;
            }

            return false;
        }
        void InitFile()
        {
            if (!Directory.Exists(_options.FileDiretoryPath))
            {
                Directory.CreateDirectory(_options.FileDiretoryPath);
            }
            var path = "";
            int i = 0;
            do
            {
                _FileName = DateTime.Now.ToString(_options.FileNameTemplate);
                path = Path.Combine(_options.FileDiretoryPath, _FileName + "_" + i + ".log");
                i++;
            } while (System.IO.File.Exists(path));
            var oldsw = _sw;
            _sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);
            _sw.AutoFlush = true;
            if (oldsw != null)
            {
                try
                {
                    oldsw.Flush();
                    oldsw.Dispose();
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
            this.Write("END");
        }
    }
}
