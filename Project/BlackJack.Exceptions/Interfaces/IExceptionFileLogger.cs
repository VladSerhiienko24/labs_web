using BlackJack.Exceptions.Models;

namespace BlackJack.Exceptions.Interfaces
{
    public interface IExceptionFileLogger
    {
        void WriteLog(ExceptionDetail exceptionDetail);
    }
}