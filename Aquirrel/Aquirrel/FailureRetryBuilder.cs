using Aquirrel.FailureRetry.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.FailureRetry
{
    /// <summary>
    /// 重试超过指定次数抛出错误
    /// </summary>
    public class RetryFaiureException : Exception
    {
        public int RetryCount { get; set; }
        public RetryFaiureException(string message, int retryCount) : base(message)
        {
            this.RetryCount = retryCount;
        }
        public RetryFaiureException(Exception exception, int retryCount) : base(exception.Message, exception)
        {
            this.RetryCount = --retryCount;
        }
        public override string ToString()
        {
            return $"retry {this.RetryCount}；{Environment.NewLine}{base.ToString()}";
        }
        public override string Message => $"retry {this.RetryCount}；{base.Message}";
    }

    /// <summary>
    /// 操作重试包装
    /// </summary>
    public class FailureRetryBuilder
    {
        protected FailureRetryBuilder()
        {
        }


        internal Action doAction;
        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        internal Func<RetryFaiureException, bool> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;

        private int invokeCount = 0;
        /// <summary>
        /// 设置执行的方法
        /// </summary>
        /// <param name="doAction"></param>
        /// <returns></returns>
        public static FailureRetryBuilder Bind(Action doAction)
        {
            var builder = new FailureRetryBuilder();
            builder.doAction = doAction;
            return builder;
        }
        //public static FailureRetryBuilder Bind(Func<Task> doAction) => BindInternal(doAction);
        /// <summary>
        /// 方法执行错误过滤，返回false表示忽略错误，真实错误试用RetryFaiureException.InnterException
        /// </summary>
        /// <param name="hasFailure"></param>
        /// <returns></returns>
        public FailureRetryBuilder RetryFilter(Func<RetryFaiureException, bool> hasFailure)
        {
            this.hasExceptionFailure = hasFailure;
            return this;
        }
        /// <summary>
        /// 设置最大的重试次数，不包含第一次执行计数，即重试2次，最高方法将执行3次
        /// </summary>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public FailureRetryBuilder RetryCount(int retryCount)
        {
            this.retryCount = retryCount;
            return this;
        }
        /// <summary>
        /// 重试依然错误后，抛出错误前的一次过滤通知.返回false则表示忽略错误，Execute将不抛出错误
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        public FailureRetryBuilder Failure(Func<RetryFaiureException, bool> failure)
        {
            failureCallback = failure;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public FailureRetryBuilder RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval">上一次的间隔时间，本次错误需要sleep时间；当前调用次数</param>
        /// <returns></returns>
        public FailureRetryBuilder RetryInterval(Func<int, int, int> retryInterval)
        {
            this.retryInterval = retryInterval(this.retryInterval, this.invokeCount);
            return this;
        }
        /// <summary>
        /// 执行包装方法
        /// </summary>
        public void Execute()
        {

            while (invokeCount++ <= this.retryCount)
            {
                try
                {
                    doAction();
                    return;
                }
                catch (Exception ex)
                {
                    if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, invokeCount)))
                        return;
                    if (invokeCount > this.retryCount)
                    {
                        var _ex = new RetryFaiureException(ex, this.retryCount);
                        if (failureCallback != null && !failureCallback(_ex)) return;
                        throw _ex;
                    }
                }
                if (this.retryInterval > 0)
                    Thread.Sleep(this.retryInterval);
            }
        }


        /// <summary>
        /// 设置执行的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doAction"></param>
        /// <returns></returns>
        public static FailureRetryBuilder<T> Bind<T>(Func<T> doAction)
        {
            var builder = new FailureRetryBuilder<T>();
            builder.doAction = doAction;
            return builder;
        }
        /// <summary>
        /// 设置执行的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doAction"></param>
        /// <returns></returns>
        public static FailureRetryTaskBuilder<T> Bind<T>(Func<Task<T>> doAction)
        {
            var builder = new FailureRetryTaskBuilder<T>();
            builder.doAction = doAction;
            return builder;
        }
        /// <summary>
        /// 设置执行的方法
        /// </summary>
        /// <param name="doAction"></param>
        /// <returns></returns>

        public static FailureRetryTaskBuilder Bind(Func<Task> doAction)
        {
            var builder = new FailureRetryTaskBuilder();
            builder.doAction = doAction;
            return builder;
        }

    }

}
namespace Aquirrel.FailureRetry.Internal
{
    /// <summary>
    /// 操作重试包装，同步带返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FailureRetryBuilder<T>
    {
        internal FailureRetryBuilder()
        {
        }


        internal Func<T> doAction;
        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        internal Func<RetryFaiureException, bool> failureCallback;
        internal Func<T, bool> hasResultFailure;

        internal int retryCount = 2;
        internal int retryInterval = 0;

        private int invokeCount = 0;
        /// <summary>
        /// 方法执行错误过滤，返回false表示忽略错误，真实错误试用RetryFaiureException.InnterException
        /// </summary>
        /// <param name="hasExceptionFailure"></param>
        /// <returns></returns>
        public FailureRetryBuilder<T> RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }
        /// <summary>
        /// 执行完成无错误对结果进行过滤，返回true表示当前执行未返回正确结果，将继续重试
        /// </summary>
        /// <param name="hasResultFailure"></param>
        /// <returns></returns>
        public FailureRetryBuilder<T> ResultFilter(Func<T, bool> hasResultFailure)
        {
            this.hasResultFailure = hasResultFailure;
            return this;
        }
        /// <summary>
        /// 设置最大的重试次数，不包含第一次执行计数，即重试2次，最高方法将执行3次
        /// </summary>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public FailureRetryBuilder<T> RetryCount(int retryCount)
        {
            this.retryCount = Math.Max(1, retryCount);
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public FailureRetryBuilder<T> RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval">上一次的间隔时间，本次错误需要sleep时间；当前调用次数</param>
        /// <returns></returns>
        public FailureRetryBuilder<T> RetryInterval(Func<int, int, int> retryInterval)
        {
            this.retryInterval = retryInterval(this.retryInterval, this.invokeCount);
            return this;
        }
        /// <summary>
        /// 重试依然错误后，抛出错误前的一次过滤通知.返回false则表示忽略错误，Execute将不抛出错误
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        public FailureRetryBuilder<T> Failure(Func<RetryFaiureException, bool> failure)
        {
            failureCallback = failure;
            return this;
        }
        /// <summary>
        /// 执行包装方法
        /// </summary>
        /// <returns></returns>
        public T Execute()
        {
            T r;
            while (invokeCount++ <= this.retryCount)
            {
                try
                {
                    r = doAction();
                    if (this.hasResultFailure == null || !this.hasResultFailure(r))
                        return r;
                }
                catch (Exception ex)
                {
                    if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, invokeCount)))
                        return default(T);

                    if (invokeCount > this.retryCount)
                    {
                        var _ex = new RetryFaiureException(ex, this.retryCount);
                        if (failureCallback != null && !failureCallback(_ex)) return default(T);
                        throw _ex;
                    }
                    if (this.retryInterval > 0)
                        Thread.Sleep(this.retryInterval);

                    continue;
                }

                if (this.retryInterval > 0)
                    Thread.Sleep(this.retryInterval);
            }

            throw new RetryFaiureException("retry has max. but proccess is error", this.retryCount);
        }
    }
    /// <summary>
    /// 操作重试包装，异步
    /// </summary>
    public class FailureRetryTaskBuilder
    {
        internal Func<Task> doAction;
        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        internal Func<RetryFaiureException, bool> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        private int invokeCount = 0;
        /// <summary>
        /// 方法执行错误过滤，返回false表示忽略错误，真实错误试用RetryFaiureException.InnterException
        /// </summary>
        /// <param name="hasExceptionFailure"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }
        /// <summary>
        /// 设置最大的重试次数，不包含第一次执行计数，即重试2次，最高方法将执行3次
        /// </summary>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder RetryCount(int retryCount)
        {
            this.retryCount = retryCount;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval">上一次的间隔时间，本次错误需要sleep时间；当前调用次数</param>
        /// <returns></returns>
        public FailureRetryTaskBuilder RetryInterval(Func<int, int, int> retryInterval)
        {
            this.retryInterval = retryInterval(this.retryInterval, this.invokeCount);
            return this;
        }
        /// <summary>
        /// 重试依然错误后，抛出错误前的一次过滤通知.返回false则表示忽略错误，Execute将不抛出错误
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder Failure(Func<RetryFaiureException, bool> failure)
        {
            failureCallback = failure;
            return this;
        }
        /// <summary>
        /// 执行包装方法
        /// </summary>
        /// <returns></returns>
        public Task ExecuteAsync()
        {
            return Task.Run(async () =>
            {

                while (invokeCount <= this.retryCount)
                {
                    invokeCount++;
                    try
                    {
                        var t = doAction();
                        await t;
                    }
                    catch (Exception ex)
                    {
                        if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, invokeCount)))
                            return;
                        if (invokeCount > this.retryCount)
                        {
                            var _ex = new RetryFaiureException(ex, this.retryCount);
                            if (failureCallback != null && !failureCallback(_ex)) return;
                            throw _ex;
                        }
                    }
                    if (this.retryInterval > 0)
                        await Task.Delay(this.retryInterval);
                }
            });
        }

    }
    /// <summary>
    /// 操作重试包装，异步带返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FailureRetryTaskBuilder<T>
    {
        internal FailureRetryTaskBuilder()
        {
        }


        internal Func<Task<T>> doAction;
        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        internal Func<T, bool> hasResultFailure;
        internal Func<RetryFaiureException, bool> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        private int invokeCount = 0;
        /// <summary>
        /// 方法执行错误过滤，返回false表示忽略错误，真实错误试用RetryFaiureException.InnterException
        /// </summary>
        /// <param name="hasExceptionFailure"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }
        /// <summary>
        /// 执行完成无错误对结果进行过滤，返回true表示当前执行未返回正确结果，将继续重试
        /// </summary>
        /// <param name="hasResultFailure"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> ResultFilter(Func<T, bool> hasResultFailure)
        {
            this.hasResultFailure = hasResultFailure;
            return this;
        }
        /// <summary>
        /// 设置最大的重试次数，不包含第一次执行计数，即重试2次，最高方法将执行3次
        /// </summary>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> RetryCount(int retryCount)
        {
            this.retryCount = Math.Max(1, retryCount);
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        /// <summary>
        /// 设置重试间隔时间，单位毫秒
        /// </summary>
        /// <param name="retryInterval">上一次的间隔时间，本次错误需要sleep时间；当前调用次数</param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> RetryInterval(Func<int, int, int> retryInterval)
        {
            this.retryInterval = retryInterval(this.retryInterval, this.invokeCount);
            return this;
        }
        /// <summary>
        /// 重试依然错误后，抛出错误前的一次过滤通知.返回false则表示忽略错误，Execute将不抛出错误
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        public FailureRetryTaskBuilder<T> Failure(Func<RetryFaiureException, bool> failure)
        {
            failureCallback = failure;
            return this;
        }
        /// <summary>
        /// 执行包装方法
        /// </summary>
        /// <returns></returns>
        public Task<T> ExecuteAsync()
        {
            return Task.Run(async () =>
            {
                T r;
                while (invokeCount <= this.retryCount)
                {
                    invokeCount++;
                    try
                    {
                        r = await doAction();
                        if (this.hasResultFailure == null || !this.hasResultFailure(r)) return r;
                    }
                    catch (Exception ex)
                    {
                        if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, invokeCount)))
                            return default(T);

                        if (invokeCount > this.retryCount)
                        {
                            var _ex = new RetryFaiureException(ex, this.retryCount);
                            if (failureCallback != null && !failureCallback(_ex)) return default(T);
                            throw _ex;
                        }

                        if (this.retryInterval > 0)
                            await Task.Delay(this.retryInterval);

                        continue;
                    }

                    if (this.retryInterval > 0)
                        await Task.Delay(this.retryInterval);
                }

                throw new RetryFaiureException("retry has max. but proccess is error", this.retryCount);

            });
        }
    }
}