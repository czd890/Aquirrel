using Aquirrel.FailureRetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel.Test
{
    [TestClass]
    public class FailureRetryTest
    {
        [TestMethod]
        public void FailureRetry_void()
        {
            FailureRetryBuilder.Bind(() => { }).Execute();
        }

        [TestMethod]
        public void FailureRetry_void_failure()
        {
            Assert.ThrowsException<RetryFaiureException>(() =>
            {
                FailureRetryBuilder.Bind(() =>
                {
                    var x = 0;
                    Console.WriteLine(1 / x);
                }).RetryFilter(ex =>
                {
                    Console.WriteLine("exception:" + ex.ToString());
                    return true;
                })
                .Execute();
            });
        }

        [TestMethod]
        public void FailureRetry_Func_T()
        {
            string str = FailureRetryBuilder.Bind(() => { return "aa"; }).Execute();
            Assert.AreEqual(str, "aa");
        }
        [TestMethod]
        public void FailureRetry_Func_T_result_failure()
        {
            Assert.ThrowsException<RetryFaiureException>(() =>
            {
                try
                {
                    FailureRetryBuilder.Bind(() => { return "aa"; }).ResultFilter(str =>
                            {
                                Console.WriteLine("执行结果:" + str);
                                return true;
                            }).Execute();
                }
                catch (Exception e)
                {

                    throw;
                }
            });
        }
        [TestMethod]
        public void FailureRetry_Func_T_exception_failure()
        {
            Assert.ThrowsException<RetryFaiureException>(() =>
            {
                FailureRetryBuilder.Bind(() =>
                {
                    this.ThrowEx();
                }).RetryFilter(ex =>
                {
                    Console.WriteLine(ex.ToString());
                    return true;
                }).Execute();
            });
        }


        [TestMethod]
        public void FailureRetry_Func_Task_T()
        {
            Task<int> taskInt = FailureRetryBuilder.Bind(() => { return Task.FromResult(33); }).ExecuteAsync();
            Task.WaitAll(taskInt);
            Assert.AreEqual(taskInt.Result, 33);
        }

        [TestMethod]
        public void FailureRetry_Func_Task_T_retry1()
        {
            var a = false;
            Task<int> taskInt = FailureRetryBuilder.Bind(() => { return Task.FromResult(33); })
                .ResultFilter(ii =>
                {
                    Console.WriteLine(ii);
                    a = !a;
                    return a;
                })
                .ExecuteAsync();

            Task.WaitAll(taskInt);
            Assert.AreEqual(taskInt.Result, 33);
        }


        [TestMethod]
        public void FailureRetry_Func_Task_T_result_failure()
        {
            Task<int> taskInt = FailureRetryBuilder.Bind(() => { return Task.FromResult(33); })
                .ResultFilter(ii => true)
                .ExecuteAsync();

            Assert.ThrowsExceptionAsync<RetryFaiureException>(() => taskInt);
        }
        [TestMethod]
        public void FailureRetry_Func_Task_T_exception_failure()
        {
            Task<int> taskInt = FailureRetryBuilder.Bind(() => { this.ThrowEx(); return Task.FromResult(33); })
                .ResultFilter(ii => true)
                .RetryFilter(ex =>
                {
                    Console.WriteLine(ex);
                    return true;
                })
                .ExecuteAsync();

            Assert.ThrowsExceptionAsync<RetryFaiureException>(() => taskInt);
        }

        [TestMethod]
        public void FailureRetry_Func_Task_T_exception_success()
        {
            Task<int> taskInt = FailureRetryBuilder.Bind(() => { this.ThrowEx(); return Task.FromResult(33); })
                .ResultFilter(ii => true)
                .RetryFilter(ex =>
                {
                    Console.WriteLine(ex);
                    return false;
                })
                .ExecuteAsync();

            Task.WaitAll(taskInt);
            Console.WriteLine(taskInt.Result);
            Assert.AreEqual(taskInt.Result, 0);
        }

        [TestMethod]
        public void FailureRetry_Func_Task()
        {
            Task task = FailureRetryBuilder.Bind(() => { return Task.Delay(99); }).ExecuteAsync();
            Task.WaitAll(task);
        }
        [TestMethod]
        public void FailureRetry_Func_Task_exception_failure()
        {
            Task task = FailureRetryBuilder.Bind(() => { this.ThrowEx(); return Task.Delay(99); }).ExecuteAsync();
            Assert.ThrowsExceptionAsync<RetryFaiureException>(() => task);
        }

        [TestMethod]
        public void FailureRetry_Func_Task_exception_success()
        {
            Task task = FailureRetryBuilder.Bind(() => { this.ThrowEx(); return Task.Delay(99); }).RetryFilter(ii=>false).ExecuteAsync();
            Task.WaitAll(task);
        }


        void ThrowEx() => throw new Exception("throw ex");
    }
}