using Aquirrel.FailureRetry.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.FailureRetry
{
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


    public class FailureRetryBuilder
    {
        protected FailureRetryBuilder()
        {
        }


        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        Action<RetryFaiureException> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        internal Action doAction;

        public static FailureRetryBuilder Bind(Action doAction)
        {
            var builder = new FailureRetryBuilder();
            builder.doAction = doAction;
            return builder;
        }
        //public static FailureRetryBuilder Bind(Func<Task> doAction) => BindInternal(doAction);

        public FailureRetryBuilder RetryFilter(Func<RetryFaiureException, bool> hasFailure)
        {
            this.hasExceptionFailure = hasFailure;
            return this;
        }
        public FailureRetryBuilder RetryCount(int retryCount)
        {
            this.retryCount = retryCount;
            return this;
        }
        public FailureRetryBuilder Failure(Action<RetryFaiureException> failure)
        {
            failureCallback = failure;
            return this;
        }
        public FailureRetryBuilder RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }

        public void Execute()
        {
            int i = 0;
            while (i++ <= this.retryCount)
            {
                try
                {
                    doAction();
                    return;
                }
                catch (Exception ex)
                {
                    if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, i)))
                        return;
                    if (i > this.retryCount)
                    {
                        var _ex = new RetryFaiureException(ex, this.retryCount);
                        if (failureCallback != null) failureCallback(_ex);
                        throw _ex;
                    }
                }
                if (this.retryInterval > 0)
                    Thread.Sleep(this.retryInterval);
            }
        }



        public static FailureRetryBuilder<T> Bind<T>(Func<T> doAction)
        {
            var builder = new FailureRetryBuilder<T>();
            builder.doAction = doAction;
            return builder;
        }
        public static FailureRetryTaskBuilder<T> Bind<T>(Func<Task<T>> doAction)
        {
            var builder = new FailureRetryTaskBuilder<T>();
            builder.doAction = doAction;
            return builder;
        }

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
    public class FailureRetryBuilder<T>
    {
        internal FailureRetryBuilder()
        {
        }


        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        Action<RetryFaiureException> failureCallback;
        internal Func<T, bool> hasResultFailure;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        internal Func<T> doAction;

        public FailureRetryBuilder<T> RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }
        public FailureRetryBuilder<T> ResultFilter(Func<T, bool> hasResultFailure)
        {
            this.hasResultFailure = hasResultFailure;
            return this;
        }

        public FailureRetryBuilder<T> RetryCount(int retryCount)
        {
            this.retryCount = Math.Max(1, retryCount);
            return this;
        }
        public FailureRetryBuilder<T> RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        public FailureRetryBuilder<T> Failure(Action<RetryFaiureException> failure)
        {
            failureCallback = failure;
            return this;
        }

        public T Execute()
        {
            T r;
            int i = 0;
            while (i++ <= this.retryCount)
            {
                try
                {
                    r = doAction();
                    if (this.hasResultFailure == null || !this.hasResultFailure(r))
                        return r;
                }
                catch (Exception ex)
                {
                    if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, i)))
                        return default(T);

                    if (i > this.retryCount)
                    {
                        var _ex = new RetryFaiureException(ex, this.retryCount);
                        if (failureCallback != null) failureCallback(_ex);
                        throw _ex;
                    }
                    if (this.retryInterval > 0)
                        Thread.Sleep(this.retryInterval);

                    continue;
                }


                if (i > this.retryCount)
                {
                    var ex = new RetryFaiureException("resutl filter error", i);
                    ex.Data["return result"] = r;

                    if (i > this.retryCount)
                    {
                        var _ex = new RetryFaiureException(ex, this.retryCount);
                        if (failureCallback != null) failureCallback(_ex);
                        throw _ex;
                    }
                }
                if (this.retryInterval > 0)
                    Thread.Sleep(this.retryInterval);
            }

            throw new RetryFaiureException("retry has max. but proccess is error", this.retryCount);
        }
    }

    public class FailureRetryTaskBuilder
    {
        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        Action<RetryFaiureException> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        internal Func<Task> doAction;

        public FailureRetryTaskBuilder RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }

        public FailureRetryTaskBuilder RetryCount(int retryCount)
        {
            this.retryCount = retryCount;
            return this;
        }
        public FailureRetryTaskBuilder RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        public FailureRetryTaskBuilder Failure(Action<RetryFaiureException> failure)
        {
            failureCallback = failure;
            return this;
        }
        public Task ExecuteAsync()
        {
            return Task.Run(async () =>
            {

                int i = 0;
                while (i++ <= this.retryCount)
                {
                    try
                    {
                        var t = doAction();
                        await t;
                    }
                    catch (Exception ex)
                    {
                        if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, i)))
                            return;
                        if (i > this.retryCount)
                        {
                            var _ex = new RetryFaiureException(ex, this.retryCount);
                            if (failureCallback != null) failureCallback(_ex);
                            throw _ex;
                        }
                    }
                    if (this.retryInterval > 0)
                        await Task.Delay(this.retryInterval);
                }
            });
        }

    }
    public class FailureRetryTaskBuilder<T>
    {
        internal FailureRetryTaskBuilder()
        {
        }


        internal Func<RetryFaiureException, bool> hasExceptionFailure;
        internal Func<T, bool> hasResultFailure;
        Action<RetryFaiureException> failureCallback;
        internal int retryCount = 2;
        internal int retryInterval = 0;
        internal Func<Task<T>> doAction;

        public FailureRetryTaskBuilder<T> RetryFilter(Func<RetryFaiureException, bool> hasExceptionFailure)
        {
            this.hasExceptionFailure = hasExceptionFailure;
            return this;
        }
        public FailureRetryTaskBuilder<T> ResultFilter(Func<T, bool> hasResultFailure)
        {
            this.hasResultFailure = hasResultFailure;
            return this;
        }

        public FailureRetryTaskBuilder<T> RetryCount(int retryCount)
        {
            this.retryCount = Math.Max(1, retryCount);
            return this;
        }
        public FailureRetryTaskBuilder<T> RetryInterval(int retryInterval)
        {
            this.retryInterval = retryInterval;
            return this;
        }
        public FailureRetryTaskBuilder<T> Failure(Action<RetryFaiureException> failure)
        {
            failureCallback = failure;
            return this;
        }

        public Task<T> ExecuteAsync()
        {
            return Task.Run(async () =>
            {
                T r;
                int i = 0;
                while (i++ <= this.retryCount)
                {
                    try
                    {
                        r = await doAction();
                        if (this.hasResultFailure == null || !this.hasResultFailure(r)) return r;
                    }
                    catch (Exception ex)
                    {
                        if (this.hasExceptionFailure != null && !this.hasExceptionFailure(new RetryFaiureException(ex, i)))
                            return default(T);

                        if (i > this.retryCount)
                        {
                            var _ex = new RetryFaiureException(ex, this.retryCount);
                            if (failureCallback != null) failureCallback(_ex);
                            throw _ex;
                        }

                        if (this.retryInterval > 0)
                            await Task.Delay(this.retryInterval);

                        continue;
                    }

                   

                    if (i > this.retryCount)
                    {
                        var ex = new RetryFaiureException("resutl filter error", i);
                        ex.Data["return result"] = r;

                        if (i > this.retryCount)
                        {
                            var _ex = new RetryFaiureException(ex, this.retryCount);
                            if (failureCallback != null) failureCallback(_ex);
                            throw _ex;
                        }
                    }
                    if (this.retryInterval > 0)
                        await Task.Delay(this.retryInterval);
                }

                throw new RetryFaiureException("retry has max. but proccess is error", this.retryCount);

            });
        }
    }
}