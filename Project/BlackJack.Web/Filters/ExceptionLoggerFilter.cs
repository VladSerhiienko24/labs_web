using BlackJack.Exceptions.BaseException;
using BlackJack.Exceptions.Extentions;
using BlackJack.Exceptions.Interfaces;
using BlackJack.Exceptions.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace BlackJack.Web.Filters
{
    public class ExceptionLoggerFilter : ExceptionFilterAttribute
    {
        public Type ExceptionType { get; set; }

        private IExceptionFileLogger _exceptionFileLogger;

        public ExceptionLoggerFilter(IExceptionFileLogger exceptionFileLogger) : base()
        {
            ExceptionType = typeof(BaseCustomException);
            _exceptionFileLogger = exceptionFileLogger;
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            var exceptionDetail = new ExceptionDetail();

            exceptionDetail.ControllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            exceptionDetail.ActionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            exceptionDetail.ExceptionMessage = actionExecutedContext.Exception.ConvertMessageFromStackTrace();
            exceptionDetail.Date = DateTime.Now;

            _exceptionFileLogger.WriteLog(exceptionDetail);

            if (actionExecutedContext.Exception != null && actionExecutedContext.Exception.GetType().BaseType == ExceptionType)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exceptionDetail.ExceptionMessage);
            }

            return Task.FromResult<object>(null);
        }
    }
}