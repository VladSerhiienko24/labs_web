using System;

namespace BlackJack.Exceptions.BaseException
{
    public class BaseCustomException : Exception
    {
        public BaseCustomException(string message)
            : base(message)
        {
        }
    }
}