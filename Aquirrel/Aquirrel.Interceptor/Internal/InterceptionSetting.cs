using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Aquirrel.Interceptor.Internal
{
    /// <summary>
    /// aop拦截配置
    /// </summary>
    public class InterceptionSetting
    {
        IConfiguration configuration;
        public IChangeToken ChangeToken { get; private set; }
        public InterceptionSetting(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.ChangeToken = configuration.GetReloadToken();
            this.ChangeToken.RegisterChangeCallback(_ => this.ReadyConf(), null);

            this.ReadyConf();
        }

        void ReadyConf()
        {
            this.Rules = this.configuration.GetSection("Rules").GetChildren().Select(conf =>
            {
                InterceptionRule rule = new InterceptionRule();
                rule.NameSpace = conf["NameSpace"];
                rule.MethodName = conf["MethodName"];
                if (conf["MethoMmodifiers"] == "" || conf["MethoMmodifiers"] == "*")
                    rule.MethoMmodifiers = new[] { "public", "private", "internal", "protected", "internal protected" };
                else
                    //rule.MethoMmodifiers = (MethoMmodifiers)Enum.Parse(typeof(MethoMmodifiers), conf["MethoMmodifiers"].Split('|').Where(p => p.IsNotNullOrEmpty()).Select(p => "@" + p).ConcatEx("|"));
                    rule.MethoMmodifiers = conf["MethoMmodifiers"].Split('|');
                rule.Ref = conf.GetSection("Ref").Get<string[]>();
                return rule;
            }).ToArray();

            List<InterceptorSettingInterceptor> _interceptors = new List<InterceptorSettingInterceptor>();
            ConfigurationBinder.Bind(configuration.GetSection("Interceptors"), _interceptors);
            this.Interceptors = _interceptors.ToArray();
        }
        /// <summary>
        /// 需要拦截的方法规则描述集合
        /// <para></para>
        /// </summary>
        public InterceptionRule[] Rules { get; set; }

        public InterceptorSettingInterceptor[] Interceptors { get; set; }

    }

    public class InterceptionRule
    {
        public string NameSpace { get; set; }
        public string[] MethoMmodifiers { get; set; }
        public string MethodName { get; set; }
        //public object Params { get; set; }
        public string[] Ref { get; set; }
    }

    [Flags]
    public enum MethoMmodifiers
    {
        @all = 0,
        @public = 1,
        @private = 2,
        @internal = 4,
        @protected = 8,
        @internalAndprotected = 16,

    }

    public class InterceptorSettingInterceptor
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
