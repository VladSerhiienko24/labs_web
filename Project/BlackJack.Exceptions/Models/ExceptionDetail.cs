using System;

namespace BlackJack.Exceptions.Models
{
    public class ExceptionDetail
    {
        public string ExceptionMessage { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public DateTime Date { get; set; }
    }
}