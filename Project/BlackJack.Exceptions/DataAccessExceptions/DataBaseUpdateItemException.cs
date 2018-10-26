using BlackJack.Exceptions.BaseException;
using BlackJack.Shared.Constants;

namespace BlackJack.Exceptions.DataAccessExceptions
{
    public class DataBaseUpdateItemException : BaseCustomException
    {
        public DataBaseUpdateItemException(string message = SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE)
            : base(message)
        {
        }
    }
}