using BlackJack.Exceptions.Interfaces;
using BlackJack.Exceptions.Models;
using BlackJack.Shared.Constants;
using System;
using System.IO;
using System.Text;

namespace BlackJack.Exceptions.Loggers
{
    public class ExceptionFileLogger : IExceptionFileLogger
    {
        public void WriteLog(ExceptionDetail exceptionDetail)
        {
            var pathBuilder = new StringBuilder();

            pathBuilder.Append(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString());
            pathBuilder.Append(string.Format("\\{0}", SolutionConstants.ERRORS_FOLDER_NAME));

            string path = pathBuilder.ToString();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            pathBuilder.Append(string.Format("\\{0}", SolutionConstants.ERRORS_FILE_NAME));

            path = pathBuilder.ToString();

            if (!File.Exists(path))
            {
                FileStream fsc = new FileStream(path, FileMode.Create, FileAccess.Write);

                fsc.Close();
            }

            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sr = new StreamWriter(fs))
                {
                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine("/---------------------Error------------------------/");
                    stringBuilder.AppendLine(string.Format("{0}", exceptionDetail.ExceptionMessage));
                    stringBuilder.AppendLine(string.Format("Controller: {0}", exceptionDetail.ControllerName));
                    stringBuilder.AppendLine(string.Format("Action: {0}", exceptionDetail.ActionName));
                    stringBuilder.AppendLine(string.Format("Data: {0}", exceptionDetail.Date));

                    sr.WriteLine(stringBuilder.ToString());
                }
            }

        }
    }
}