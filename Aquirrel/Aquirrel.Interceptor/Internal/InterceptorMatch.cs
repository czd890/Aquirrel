using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Aquirrel.Interceptor.Internal
{
    /// <summary>
    /// aop拦截匹配接口
    /// </summary>
    public interface IInterceptorMatch
    {
        IChangeToken ChangeToken { get; }

        bool IsMatchType(Type impType);
        bool IsMatchMethod(MethodInfo methodInfo);
        IEnumerable<Type> GetMatchedInterceptor(Type impType);
    }
    /// <summary>
    /// aop拦截匹配默认实现
    /// </summary>
    public class DefaultInterceptorMatch : IInterceptorMatch
    {
        public InterceptionSetting Setting { get; }
        public IChangeToken ChangeToken { get; }
        public DefaultInterceptorMatch(InterceptionSetting setting)
        {
            this.Setting = setting;
            this.ChangeToken = setting.ChangeToken;
        }

        public bool IsMatchType(Type impType)
        {
            foreach (var rule in this.Setting.Rules)
            {
                if (this.IsMatchType(rule, impType))
                    return true;
            }
            return false;
        }
        bool IsMatchType(InterceptionRule rule, Type type)
        {
            var namespaceses = this.GetNameSpaceMatchs(type.FullName);
            foreach (var key in namespaceses)
            {
                //Aquirrel.* Aquirrel.#

                //Aquirrel==Aquirrel true
                if (rule.NameSpace == key)
                {
                    return true;
                }
                //Aquirrel.# ==Aquirrel.aa.bb true
                if (rule.NameSpace.Last() == '#' && rule.NameSpace.TrimEnd('.', '#') == key)
                {
                    return true;
                }
                //Aquirrel.* ==Aquirrel.aa true
                if (rule.NameSpace.Last() == '*' && rule.NameSpace.TrimEnd('.', '*') == key)
                {
                    var ruleD = rule.NameSpace.Split('.').Length;
                    var keyD = type.FullName.Split('.').Length;
                    if (ruleD == keyD)
                        return true;
                }
            }
            return false;
        }

        IEnumerable<string> GetNameSpaceMatchs(string fullName)
        {
            while (fullName.IsNotNullOrEmpty())
            {
                yield return fullName;
                var last = fullName.LastIndexOf('.');
                if (last <= 0)
                    yield break;
                fullName = fullName.Substring(0, last);
            }
            yield break;
        }

        public bool IsMatchMethod(MethodInfo methodInfo)
        {


            foreach (var rule in this.Setting.Rules)
            {
                if (this.IsMatchMethod(rule, methodInfo))
                    return true;
            }

            return false;
        }
        bool IsMatchMethod(InterceptionRule rule, MethodInfo methodInfo)
        {
            if (methodInfo.IsDefined(typeof(UnacceptableAttribute)))
                return false;
            if (!this.IsMatchType(rule, methodInfo.DeclaringType))
                return false;

            if (rule.MethodName == "*")
                return true;
            Regex reg = new Regex(rule.MethodName);
            if (!reg.IsMatch(methodInfo.Name))
                return false;

            //{ "public", "private", "internal", "protected", "internal protected", };
            if (methodInfo.IsPublic && rule.MethoMmodifiers.Contains("public"))
                return true;
            if (methodInfo.IsPrivate && rule.MethoMmodifiers.Contains("private"))
                return true;
            if (methodInfo.IsAssembly && rule.MethoMmodifiers.Contains("internal"))
                return true;
            if (methodInfo.IsFamily && rule.MethoMmodifiers.Contains("protected"))
                return true;
            if (methodInfo.IsFamilyOrAssembly && rule.MethoMmodifiers.Contains("internal protected"))
                return true;
            return false;
        }

        public IEnumerable<Type> GetMatchedInterceptor(Type impType)
        {
            var rules = this.Setting.Rules.Where(rule => this.IsMatchType(rule, impType)).ToList();
            var refs = rules.SelectMany(p => p.Ref);
            var interceptorTypes = refs
                .Select(p => this.Setting.Interceptors.FirstOrDefault(x => x.Id == p).Type)
                .Distinct();
            return interceptorTypes.Select(p => Type.GetType(p));
        }
    }
}
