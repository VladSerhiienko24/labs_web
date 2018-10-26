using System.Net;

namespace BlackJack.ViewModels.GameViewModels
{
    public class DefaultResponse
    {
        public string ResponseMessage { get; set; }
        public string Detail { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
    }
}