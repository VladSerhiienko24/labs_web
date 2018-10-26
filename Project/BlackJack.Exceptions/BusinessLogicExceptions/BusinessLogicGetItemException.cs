using BlackJack.Exceptions.BaseException;
using BlackJack.Shared.Constants;

namespace BlackJack.Exceptions.BusinessLogicExceptions
{
    public class BusinessLogicGetItemException : BaseCustomException
    {
        public BusinessLogicGetItemException(string message = SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE)
            : base(message)
        {
        }
    }
}