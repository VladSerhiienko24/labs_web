using BlackJack.Exceptions.BaseException;
using BlackJack.Shared.Constants;

namespace BlackJack.Exceptions.DataAccessExceptions
{
    public class DataBaseGetNullFromCreateItemException : BaseCustomException
    {
        public DataBaseGetNullFromCreateItemException(string message = SolutionConstants.DB_CREATE_ITEM_EXCEPTION_MESSAGE)
            : base(message)
        {
        }
    }
}