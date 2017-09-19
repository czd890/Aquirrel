using System;

namespace Aquirrel
{
    public class BusinessException : Exception
    {
        public int ErrCode { get; }
        public BusinessException() { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }

        public BusinessException(string message, int errCode) : base(message)
        {
            this.ErrCode = errCode;
        }
        public BusinessException(string message, Exception innerException, int errCode) : base(message, innerException)
        {
            this.ErrCode = errCode;
        }
    }
}